using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Cognito;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.SSM;
using Constructs;

namespace Cdk
{
    public class CdkStack : Stack
    {
        internal CdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            string appName = System.Environment.GetEnvironmentVariable("APP_NAME")!;
            string region = System.Environment.GetEnvironmentVariable("REGION_AWS")!;
            string emailSubject = System.Environment.GetEnvironmentVariable("VERIFICATION_SUBJECT")!;
            string emailBody = System.Environment.GetEnvironmentVariable("VERIFICATION_BODY")!;


            UserPool userPool = new(this, $"{appName}UserPool", new UserPoolProps {
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
            UserPoolClient userPoolClient = new(this, $"{appName}FrontendUserPoolClient", new UserPoolClientProps {
                UserPoolClientName = $"{appName}FrontendUserPoolClient",
                UserPool = userPool,
                GenerateSecret = false,
                PreventUserExistenceErrors = true,
                AuthFlows = new AuthFlow {
                    UserSrp = true,
                },
                DisableOAuth = true,
            });

            _ = new StringParameter(this, $"{appName}StringParameterCognitoUserPoolId", new StringParameterProps {
                ParameterName = $"/{appName}/Cognito/UserPoolId",
                Description = $"Cognito UserPoolId de la aplicación {appName}",
                StringValue = userPool.UserPoolId,
                Tier = ParameterTier.STANDARD,
            });

            _ = new StringParameter(this, $"{appName}StringParameterCognitoUserPoolClientId", new StringParameterProps {
                ParameterName = $"/{appName}/Cognito/UserPoolClientId",
                Description = $"Cognito UserPoolClientId de la aplicación {appName}",
                StringValue = userPoolClient.UserPoolClientId,
                Tier = ParameterTier.STANDARD,
            });

            _ = new StringParameter(this, $"{appName}StringParameterCognitoRegion", new StringParameterProps {
                ParameterName = $"/{appName}/Cognito/Region",
                Description = $"Cognito Region de la aplicación {appName}",
                StringValue = region,
                Tier = ParameterTier.STANDARD,
            });
        }
    }
}
