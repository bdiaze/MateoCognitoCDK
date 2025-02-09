using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Cognito;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Constructs;

namespace Cdk
{
    public class CdkStack : Stack
    {
        internal CdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            string appName = System.Environment.GetEnvironmentVariable("APP_NAME")!;
            string emailSubject = System.Environment.GetEnvironmentVariable("VERIFICATION_SUBJECT")!;
            string emailBody = System.Environment.GetEnvironmentVariable("VERIFICATION_BODY")!;
            string domainName = System.Environment.GetEnvironmentVariable("DOMAIN_NAME")!;
            string subdomainName = System.Environment.GetEnvironmentVariable("SUBDOMAIN_NAME")!;
            string certificateArn = System.Environment.GetEnvironmentVariable("CERTIFICATE_ARN")!;


            UserPool userPool = new UserPool(this, $"{appName}UserPool", new UserPoolProps {
                UserPoolName = $"{appName}UserPool",
                SelfSignUpEnabled = true,
                UserVerification = new UserVerificationConfig {
                    EmailSubject = emailSubject,
                    EmailBody = emailBody,
                    EmailStyle = VerificationEmailStyle.CODE,
                },
                SignInAliases = new SignInAliases {
                    Username = true,
                    Email = true
                },
                AutoVerify = new AutoVerifiedAttrs {
                    Email = true
                },
                KeepOriginal = new KeepOriginalAttrs { 
                    Email = true
                },
                Mfa = Mfa.OPTIONAL,
                MfaSecondFactor = new MfaSecondFactor {
                    Otp = true,
                },
                AccountRecovery = AccountRecovery.EMAIL_ONLY,
                StandardAttributes = new StandardAttributes {
                    Email = new StandardAttribute {
                        Required = true,
                        Mutable = true
                    },
                    Nickname = new StandardAttribute {
                        Required = false,
                        Mutable = true
                    },
                    Birthdate = new StandardAttribute {
                        Required = false,
                        Mutable = true,
                        
                    },
                    Gender = new StandardAttribute {
                        Required = false,
                        Mutable = true
                    },
                },
                PasswordPolicy = new PasswordPolicy {
                    MinLength = 8,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequireDigits = true,
                    RequireSymbols = true
                }
            });

            // Create client for login/logout page, any other permissions required should be added in another client...
            UserPoolClient userPoolClient = new UserPoolClient(this, $"{appName}FrontendUserPoolClient", new UserPoolClientProps {
                UserPoolClientName = $"{appName}FrontendUserPoolClient",
                UserPool = userPool,
                GenerateSecret = false,
                PreventUserExistenceErrors = true,
                ReadAttributes = new ClientAttributes().WithStandardAttributes(new StandardAttributesMask { 
                    Email = true,
                }),
                WriteAttributes = new ClientAttributes().WithStandardAttributes(new StandardAttributesMask {
                    Email = true,
                }),
                AuthFlows = new AuthFlow {
                    UserSrp = true,
                },
                DisableOAuth = true,
            });

            // Create client for api/backend applications...
            UserPoolClient apiUserPoolClient = new UserPoolClient(this, $"{appName}ApiUserPoolClient", new UserPoolClientProps {
                UserPoolClientName = $"{appName}ApiUserPoolClient",
                UserPool = userPool,
                GenerateSecret = true,
                AuthFlows = new AuthFlow {
                    UserSrp = true,
                },
                OAuth = new OAuthSettings {
                    Flows = new OAuthFlows {
                        AuthorizationCodeGrant = true,
                        ImplicitCodeGrant = true,
                    },
                    Scopes = new[] {
                        OAuthScope.EMAIL,
                        OAuthScope.PHONE,
                        OAuthScope.OPENID,
                        OAuthScope.PROFILE,
                        OAuthScope.COGNITO_ADMIN,
                    },
                },
            });

            ICertificate certificate = Certificate.FromCertificateArn(this, $"{appName}Certificate", certificateArn);
            IHostedZone hostedZone = HostedZone.FromLookup(this, $"{appName}HostedZone", new HostedZoneProviderProps {
                DomainName = domainName
            });

            UserPoolDomain domain = new UserPoolDomain(this, $"{appName}UserPoolDomain", new UserPoolDomainProps {
                UserPool = userPool,
                CustomDomain = new CustomDomainOptions {
                    DomainName = subdomainName,
                    Certificate = certificate,
                },
            });

            ARecord record = new ARecord(this, $"{appName}ARecord", new ARecordProps {
                Zone = hostedZone,
                RecordName = subdomainName,
                Target = RecordTarget.FromAlias(new UserPoolDomainTarget(domain)),
            });
        }
    }
}
