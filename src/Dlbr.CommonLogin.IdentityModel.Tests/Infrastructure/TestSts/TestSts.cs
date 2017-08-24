using System;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Infrastructure.TestSts
{
    public class TestSts : SecurityTokenService
    {
        private TestSts(SecurityTokenServiceConfiguration securityTokenServiceConfiguration)
            : base(securityTokenServiceConfiguration)
        {}

        private const string Issuer = "TestSts";

        public static TestSts Create()
        {
            var signingCredentials = new X509SigningCredentials(GetCertificate());
            var configuration = new SecurityTokenServiceConfiguration(Issuer, signingCredentials);
            

            return new TestSts(configuration);
        }
        
        public SamlSecurityToken IssueToken(ClaimsPrincipal principal, string appliesTo)
        {
            var rst = new RequestSecurityToken
            {
                AppliesTo = new EndpointReference(appliesTo),
                KeyType = KeyTypes.Bearer,
                
            };
            var response = Issue(principal, rst);
            return (SamlSecurityToken)response.RequestedSecurityToken.SecurityToken;
        }

        public string IssueTokenAsXml(ClaimsPrincipal principal, string appliesTo)
        {
            var token = IssueToken(principal, appliesTo);
            var buffer = new StringBuilder();

            var handler = new SamlSecurityTokenHandler
                              {
                                  Configuration = new SecurityTokenHandlerConfiguration()
                              };
            var writer = XmlWriter.Create(new StringWriter(buffer));
            handler.WriteToken(writer, token);
            var tokenXml = buffer.ToString();
            return tokenXml;
        }

        public string Thumbprint 
        {
            get { return GetCertificate().Thumbprint; }
        }

        /// <summary>
        /// Returns an IssuerNameRegistry able to validate tokens issued by this test STS instance
        /// </summary>
        public SimpleValidateByThumbprintOnlyIssuerNameRegistry IssuerNameRegistry
        {
            get
            {
                return new SimpleValidateByThumbprintOnlyIssuerNameRegistry(this.Thumbprint, this.IssuerName);
            }
        }


        public string IssuerName
        {
            get { return Issuer; }
        }

        protected override Scope GetScope(ClaimsPrincipal principal, RequestSecurityToken request)
        {
            var scope = new Scope(request.AppliesTo.Uri.OriginalString, SecurityTokenServiceConfiguration.SigningCredentials);
            scope.TokenEncryptionRequired = false;
            scope.SymmetricKeyEncryptionRequired = false;
            scope.ReplyToAddress = scope.AppliesToAddress;
            return scope;
        }


        protected override ClaimsIdentity GetOutputClaimsIdentity(ClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            if (null == principal)
            {
                throw new ArgumentNullException("principal");
            }

            return principal.Identities.Single();
        }

        protected static X509Certificate2 GetCertificate()
        {
            // Base 64 encoded pfx file
            const string certString =   "MIIK0QIBAzCCCpcGCSqGSIb3DQEHAaCCCogEggqEMIIKgDCCBTcGCSqGSIb3DQEHBqCCBSgwggUk" +
                                        "AgEAMIIFHQYJKoZIhvcNAQcBMBwGCiqGSIb3DQEMAQYwDgQII2vdw5u1l/kCAggAgIIE8C5aoUqR" +
                                        "szoSvLzDuveLWKRVtexqQpVvvM8Ecuh4DvycPze0thbgTSq9dshkcs+1+NwwA21W2aqnsUHDowt3" +
                                        "8/M02zez18ZAgiW/5Q+s8X5SLILITXL/Ni9nAW7ytjXfE+vGhcowNMAWabcC02XCXPsj2jtxQ0xY" +
                                        "kTyz7AV7/sHT/U/sxNujjULbKKe7tjtUFW6r5SWY/34/OT4BXF6DIs3DX45hXpEbwyoO/69L/Ge4" +
                                        "gn/vpDb+wdG8mdro0eQ5fIqb0xEl1C3tS2bHcUOQBvNns/vh/O5Gv4i6wM8M3uCoBl0eRFRNxaqd" +
                                        "piPnzD51Ld6cH0bNfrM4dQ8xW8KwmbPz9yFsf8W0loq/VIL0Oi264F/OCZ9wX3rXWxASpENQjgbL" +
                                        "jVph9hTxAQEAwpyuUZGc/NnTClmhGoIQHluOas6Y6PuK9xhh7e/n+JbfmREAzTDjvTTzCSxm8Zkk" +
                                        "sT0JxdVJpZrvC7pQKxzhhBUc7TdwPVVzuszFekuxr8UhVtRueaSROCc1frOfiaJMgO/gyXqaoWGR" +
                                        "/pPF4rJtnRLVW1T9TLmcY4MeplYZHPNA0NVZ5BYR2Eo9VzCV9aaX1Ng1qLigB35aRmjXP8roAg9p" +
                                        "y0xFMtFZx+o9OKRp5g7+/iM1PKcQ7kojzXhOGThPV9zBeLSmU4+JAGIQa34XxsjQkTI1erTcgRAL" +
                                        "iYWmxlspDfMVmWYaoBny7bwTJYiSlxyzVnaxJXopvDYeOahYDJ6DlAHOHnODbEfeQhd3evTNdBTR" +
                                        "AndQWAddBDsu+qjFYZsAmc6zB9jwoHfw5ReQubKj1RyglLaiy+Jzg1o1Srpjle3VgczZ7NSBRcmK" +
                                        "t4vNKReZCCM3JSYFrvf5rLc9mfpG/UxwCjPc06AeuXhredMc4d0qccNo3odsy6J015MKJKaSba7a" +
                                        "2XIcInl5xoDHuY+MUAG/+cMz9Es39dc8UqVM37Rj3OXAehafIXvHprVSYrY0mJwvToTvyBtadnYH" +
                                        "WareQ+mk1wWE81C+wI2xNUcAAzPbZQ9LQX6/nT7WOgzjYOTWoZak/PAsQ+Vn22qwlwfRMmyhSpSq" +
                                        "Bem4PfIl88ehaigCFx57l/Ip+M774Mv8bFk7si7freJ3PryNI1p/w2jtXBqgTifCZysqa/NERRTp" +
                                        "hIKzZJ9iBuD7AQB12ldyUJ9oNFBhFr2Et7of1ph5CP++2SSmRpV13ufvBboFKXOil3oCYtCCS6my" +
                                        "gj0TWgTvYJ0oJ54vhsuQrmGT1aQEs8lnTpID2jwkvcFTQq5TqW0+QBDFa0YJNtnoMCdidJeGXPiJ" +
                                        "YI9uUNpO9QF+kG6NlzRnGLv8/ErEPhasDjUdTUAvrkXdW7MYd93QtRm9ktbMVF1ygstBED9dyUD7" +
                                        "WILZpoLG++wWZOsUbCX9iFiuHH8fCilIf28mS8OABMwEyqc5ax3n8KhR4EMgIt80HHluQW7YM8WK" +
                                        "8BaNyVADL8stzXiqzDjZS1GmyV2MrxDJCjrAOInBu8PhkqLsrsLQ5CxPw4q46JW8qXxE+rxTpQN8" +
                                        "sRdCesevFlpAJ+c0W8kNqh9sS4qqFvwoQCEdH6BR4Z5QGkaVRkcdA+42zJZvGFRk9WxD9UVbUAzT" +
                                        "Z26OBHF8CCuuvLc8JDcctCi0mng9zNGg1iCdWde9daKyZRL8zgMatwVqhzBnNC25zDaJhDMpjsyk" +
                                        "hy5aUmQwggVBBgkqhkiG9w0BBwGgggUyBIIFLjCCBSowggUmBgsqhkiG9w0BDAoBAqCCBO4wggTq" +
                                        "MBwGCiqGSIb3DQEMAQMwDgQI2hkVTUNpP9wCAggABIIEyIWBy3CyrF/RyO79UeHmTHZj8xj9kc4y" +
                                        "hNE07Vy6XZhygX0Dit4FCoh44AeqLO8ht8kgyQK7hxPp2udchFNAT6aJHAk7PEfXaWpVGxXpwjmQ" +
                                        "nrUvRLGoSh4d/5+nqJkxBKwmpQQL3lxTpRumbtndWgO0V+F45FdiGKLCZ4gI2xpgBfFmJMm5SHwJ" +
                                        "eIpZjq8/f7+C+H3LiEKDdHplA6k2UbmKxjbKxw2DKvv1BcPFZpAWcQUkkHehHOfb22R2rkNJ7RuS" +
                                        "lx9g+yDOTVP3N0Vq8zFFF2otRgh1tDv2WBxs1eRE4ixtSCIKn3xKsYEQQtZ5pStXejqy2t8OPMOs" +
                                        "pv1ViK+15RRBVK06Cmotq0roRIuoqDBM5lj6dHWbAp2htlDCPhfRwK/9Oy74bLpjlLZB5mfgJ/ho" +
                                        "t23pmVzvZ9xK0kmPWbC1EniNEKdBeVAeIyQDhsmtDMWDvjTD3G1tCxWia7U8sv+T2Sogc6e5Ayt3" +
                                        "JoW/+vAObBWC0F8gtpHDZz5VmwTfB3HN0/DYT2vBznaRZ2OXfKMxrIrEYXpB+T1V6KG+WJRxiHvl" +
                                        "EGB2quCqZ20DjN79kQcmn5z15tyjb4y2uYUKL3kVDXt3qx33QPfxibXUVJ/KTbtRffLaYdcD4hpo" +
                                        "7ZImXF0/pjF/qSpguI8VJINB7uRyTWKtnWHWT+xIPd1SDK3VCXX0EiBQUtxAlzLjjicX1yTk5O58" +
                                        "TrC21p1p9J/pWFzASXelRrM/z4TnqmZQrK/iEovL3z5Gtr4oDQ/CBRWxp8CJ094KyRCk41u1sSOf" +
                                        "4EN+VBca/6z7+rg0XEWXVDbn2U9IPeN9nMi6Q9v90wmiPSgAEdtLogwsSdQhKk2zaRL/7nZZft/8" +
                                        "PoWLLhWIfY0N9OhSFLb/ZhZAlwxtA9T/wWNrbbhO3RUPnXvm6ZRSpiqRd9GuX6ZHJQS482ClwWZH" +
                                        "Lr57fSzw9MzLDKbK5peLemVeZ6snx0wjuRHTp17nM8CIM+0kJSpFnBco5lOkJaqiTXJHkREUpq+g" +
                                        "NqsZh3JC4dUxkvoN8t1KnOjEJvYVajMtLnvWPnpI99+Wntj+/0CUB1hcnlDlqgHOJszuQZWCv4OU" +
                                        "sZ3cevnJAqvX6sXp7WWi+F0uwgPuznMkhMw92ru1gMLD+FqZA7ek96cDOE6Xjsx3qgwofOGFrKWM" +
                                        "EI8r5oAEfHdPhqDbobOLQ2Q/MCkjo25L19OKR8LI3rfG6ZNloiNMa2FWM0RIjnX4lzDKB74kYWWu" +
                                        "k6GB6M0WLtxfjME3fkFwomt+AEfM/bYBDDmYTLLVzd/OiTUlLQ5S42QuVwhbNSCxS1ur8m/38JXg" +
                                        "VqAeg0fvDVr4JtKv8tTFoV79n0nDWnQK0TRy2Qpm7a1WgqeiRRYSu4BBbeR8/3wdtBmko2bwep+q" +
                                        "K1dEFPU2t1hnLKQ1KHufSpPd/ETep75tgH1qxfvop8AaQwMpAAhJyhIlGv0Mf9oUOCY4tWW0hkP1" +
                                        "FMdoIpeelS+OPGJzzM8fwmMsRqqPQUA7AbNtRSBIAsO8+FJsZThRNVcABvkwiwcYo0iznHXq6CJG" +
                                        "bCZiVT1BOp9WHFx6jCiGdAv9aL72t/uls3SO4alE5cKNvjP68s2DZUDjpetB6+gaxeiH0fe9VVWu" +
                                        "eVjnTTElMCMGCSqGSIb3DQEJFTEWBBRHYpYZ4elJH0ojLEvDdwdph2liajAxMCEwCQYFKw4DAhoF" +
                                        "AAQUULaF/YolvJl3cpJJ+crmG0FBDCQECLzungy6LZAVAgIIAA==";
            return new X509Certificate2(Convert.FromBase64String(certString), "teststs", X509KeyStorageFlags.PersistKeySet);
        }

    }
}

