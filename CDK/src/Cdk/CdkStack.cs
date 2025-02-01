using Amazon.CDK;
using Amazon.CDK.AWS.Cognito;
using Constructs;

namespace Cdk
{
    public class CdkStack : Stack
    {
        internal CdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            string error = System.Environment.GetEnvironmentVariable("variable_no_existe")!;
            string appName = System.Environment.GetEnvironmentVariable("APP_NAME")!;
            string emailSubject = System.Environment.GetEnvironmentVariable("VERIFICATION_SUBJECT")!;
            string emailBody = System.Environment.GetEnvironmentVariable("VERIFICATION_BODY")!;
            string urlCallback = System.Environment.GetEnvironmentVariable("URL_CALLBACK")!;
            string urlLogout = System.Environment.GetEnvironmentVariable("URL_LOGOUT")!;

            UserPool userPool = new UserPool(this, $"{appName}UserPool", new UserPoolProps {
                UserPoolName = $"{appName}UserPool",
                SelfSignUpEnabled = true,
                UserVerification = new UserVerificationConfig {
                    EmailSubject = emailSubject,
                    EmailBody = emailBody,
                    EmailStyle = VerificationEmailStyle.LINK,
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
                        Required = true,
                        Mutable = true
                    },
                    Birthdate = new StandardAttribute {
                        Required = true,
                        Mutable = true,
                        
                    },
                    Gender = new StandardAttribute {
                        Required = true,
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

            UserPoolClient userPoolClient = new UserPoolClient(this, $"{appName}UserPoolClient", new UserPoolClientProps {
                UserPool = userPool,
                GenerateSecret = false,
                PreventUserExistenceErrors = true,
                ReadAttributes = new ClientAttributes().WithStandardAttributes(new StandardAttributesMask { 
                    Email = true,
                    Nickname = true,
                    Birthdate = true,
                    Gender = true
                }),
                SupportedIdentityProviders = new UserPoolClientIdentityProvider[] {
                    UserPoolClientIdentityProvider.COGNITO
                },
                AuthFlows = new AuthFlow {
                    UserPassword = true
                },
                OAuth = new OAuthSettings { 
                    Flows = new OAuthFlows {
                        AuthorizationCodeGrant = true
                    },
                    Scopes = new OAuthScope[] {
                        OAuthScope.EMAIL,
                        OAuthScope.OPENID,
                        OAuthScope.PROFILE,
                        OAuthScope.COGNITO_ADMIN
                    },
                    CallbackUrls = new string[] {
                        urlCallback
                    },
                    LogoutUrls = new string[] {
                        urlLogout
                    }
                }
            });
        }
    }
}
