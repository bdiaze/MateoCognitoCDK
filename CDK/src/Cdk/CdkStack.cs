using Amazon.CDK;
using Amazon.CDK.AWS.Cognito;
using Constructs;

namespace Cdk
{
    public class CdkStack : Stack
    {
        internal CdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {

            UserPool userPool = new UserPool(this, "MateoUserPool", new UserPoolProps {
                UserPoolName = "MateoUserPool",
                SelfSignUpEnabled = true,
                UserVerification = new UserVerificationConfig {
                    EmailSubject = "¡Verifica tu correo electrónico! - Mateo",
                    EmailBody = "¡Hola {username}!, Para verificar tu correo electrónico solo debes hacer click {##¡Aquí!##}",
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
                        Mutable = true
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

            UserPoolClient userPoolClient = new UserPoolClient(this, "MateoUserPoolClient", new UserPoolClientProps {
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
                        "http://localhost/signin-oidc"
                    },
                    LogoutUrls = new string[] {
                        "http://localhost/logout-oidc"
                    }
                }
            });
        }
    }
}
