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
                SignInAliases = new SignInAliases {
                    Email = true
                },
                AutoVerify = new AutoVerifiedAttrs {
                    Email = true
                },
                StandardAttributes = new StandardAttributes {
                    Email = new StandardAttribute {
                        Required = true,
                        Mutable = false
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

        }
    }
}
