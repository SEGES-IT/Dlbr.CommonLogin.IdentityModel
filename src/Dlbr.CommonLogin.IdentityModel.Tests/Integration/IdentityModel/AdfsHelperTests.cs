using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Integration.IdentityModel
{
    [TestFixture]
    public class AdfsHelperTests
    {
        #region GetActAsToken
        [Test]
        public void GetActAsToken_AllOkay_SecurityTokenReturned()
        {
            // Arrange
            SecurityToken bootstrap = AdfsHelper.GetSecurityToken("https://devtest-idp.vfltest.dk/adfs/services/trust/13/usernamemixed",
                "https://dev.dcf.ws.dlbr.dk/DCFServices/", "DCFIntegrationstest", "dcftest");
            ClaimsIdentity identity = new ClaimsIdentity();
            identity.BootstrapContext = bootstrap;
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            Thread.CurrentPrincipal = principal;

            // Act
            SecurityToken actAsToken =
                AdfsHelper.GetActAsToken("https://devtest-idp.vfltest.dk/adfs/services/trust/13/usernamemixed",
                                         "https://dev.dcf.ws.dlbr.dk/DCFServices/", "DCFKVKService", "dcfkvk898");
            
            // Assert
            actAsToken.Should().NotBeNull();
        }

        #endregion GetActAsToken

        #region GetSecurityToken

        [Test]
        public void GetSecurityToken_AllOkay_SecurityTokenReturned()
        {
            // Act
            SecurityToken actAsToken = AdfsHelper.GetSecurityToken("https://devtest-idp.vfltest.dk/adfs/services/trust/13/usernamemixed",
                "https://dev.dcf.ws.dlbr.dk/DCFServices/", "DCFIntegrationstest", "dcftest");

            // Assert
            actAsToken.Should().NotBeNull();
        }

        #endregion GetSecurityToken

        #region HelperMethods
        private static X509Certificate2 GetMacClientAuthCertificate()
        {
            // Base 64 encoded pfx file
            var sb = new StringBuilder();
            sb.AppendLine(@"MIIKgQIBAzCCCkcGCSqGSIb3DQEHAaCCCjgEggo0MIIKMDCCBOcGCSqGSIb3DQEHBqCCBNgwggTU");
            sb.AppendLine(@"AgEAMIIEzQYJKoZIhvcNAQcBMBwGCiqGSIb3DQEMAQYwDgQIR6NQ6xSV6HcCAggAgIIEoAf3R6Qr");
            sb.AppendLine(@"g6/enLh4c0jO0GfukqZnQRowdZK1iK1RghKtAGCOXs2ofQVeUW6oQ8s9qMe0xoxEawy3xtINAkQ9");
            sb.AppendLine(@"dno0G4bK6Mjl25B/gf/7lJvapoi1NLlsVOYokrhdmvDEygSOW5BfajzMHVwieC4+8T/47FX1Y9ZA");
            sb.AppendLine(@"Zqkem9ZRBqwdccedK+zCd0hwH64W+MzbE23NA0YVkPPd9Sud8neoCqSq+YEtQU78+weMaRJ4W8Kf");
            sb.AppendLine(@"oogJgXDWC+IJhdC6BP931enagzha7aoU9K9NmEKb9oKQqGnNvr1FjvmCq7Zs8R16hhQ2rY8sHHnC");
            sb.AppendLine(@"rGzArwlET3qWoXTdXCfsb5+zIDtmGFuFyA329YF2vaePs3uP9LTDNMJ4zyNBEyQjdnETthigyNVm");
            sb.AppendLine(@"2v7YyKLXg+fGG+uQQUsiRcn9vdckGHmK8w2adnt23QXkooahcBwd7cvF+aONKQc0DfEuB+mJs/o1");
            sb.AppendLine(@"WrWe2eEpY9nnsoEcAMaW+002tUZWFzqstroLG/P00qtUYpKddFIeR0UPBsJOqFPXseyRRJtEtltp");
            sb.AppendLine(@"2JOvkZWUHbTmZRDYbKw1PXyq9N+AGzRlCUMtl40m973IuiUtV5zTfZHWtdM2PT8bmvJVMfttO+XV");
            sb.AppendLine(@"xRi2pMjoOa6LyKRarHuS/eidnHBicYtFp5E75uKPnnsYz6NbM1wLajGqrlCXa1kvDJpXqArDlx0+");
            sb.AppendLine(@"0rrDrleAIfoTg70PXrgAJ7VKIS26DXFjtiv21MfZsZ9RGt9JWZeYE/USDm9Aj1J/4hQTayuzBySj");
            sb.AppendLine(@"cmUUnaWaTNWECo+JuNv4QBlz6IQbX/3+eKeWQEYdqhKXBr+pH9Ak5CNWZU9ZZrcb5Spha8LtCIYP");
            sb.AppendLine(@"orZw+ZHEq4/Iid1Ctd2mcPAF1BOfuGRJ4rLiByH97ksmpcKs1j5l6xGIN/LvKSXqrnhqWenQeYyw");
            sb.AppendLine(@"U2dXCaGK/WNkO8qLV9HOHF4D8g7n5IRGulG8SqCyDnL3dDiqS6lufqvvWeQ/wsxMNSLkMfLF7ZRQ");
            sb.AppendLine(@"3zL/b5yixpt9c3e29lcS1mm1GKqxuSIObyb2ZfQ617G2hhrs/qF0/fUA130gVIxQITTaFbGxRD3J");
            sb.AppendLine(@"7p4TIixAvHcggAfIGw2VcEXpw9fTGMQBlvzNjRpH4mzrFnORciUQaUN3YJ+M6SD4jflTLeTu0V6Q");
            sb.AppendLine(@"HX82R6Mm8WaW5QspUdupu/cE92R1QtVuGXRca21E2tdfP2I/dfc3hwZO+GxMcATEAOqi1Vy18YvD");
            sb.AppendLine(@"4TfoQTYMM370zu5jCo2nlphNQzwNhqi4FWr73cBQbJ7IfGNKwwUIEXNLvgHXiQ6PUxnGn1EwGPlB");
            sb.AppendLine(@"TNUdooNC8P4ELm8U2+YYcYQI6y8iHpuMq9EiGWhKMtV459m0RbSiv4PEuSRfFyfpLIR8GbuZwquF");
            sb.AppendLine(@"DhIDQqcWNheA6f0b7HrRpJEQkM3CHUu344LaBi8FR1sbgtg434gWDrFEFG+R8z9tpqWrl7LcABRe");
            sb.AppendLine(@"Hm2sOwZaRFLuzHK+pJpP4Pr8LHrP5ethpZ0WrCsbLOXlz0eSeWQ0vWVWgzo5e8fGXdef4pbb17e3");
            sb.AppendLine(@"RsBzNjSzKyQXKrSxsIg7M9Gm1Ys287/Uq8g9eA0fi64SMiwPr1GfMIIFQQYJKoZIhvcNAQcBoIIF");
            sb.AppendLine(@"MgSCBS4wggUqMIIFJgYLKoZIhvcNAQwKAQKgggTuMIIE6jAcBgoqhkiG9w0BDAEDMA4ECDNUGA8v");
            sb.AppendLine(@"PQrHAgIIAASCBMhx8Hkq7+9aq9tb37P+qTOyvIk/+5b8u4G3jeAp+1sKdEtEHJdyN6tLK6Azu4dy");
            sb.AppendLine(@"5phuzR3XDoJ+Tjzd1xeUE+UCAAUuwRD1UpWIJLYd6tL+D0up2e6SfuoRG6hroYVM6K31D75TUOKt");
            sb.AppendLine(@"gFKvng46rvKHRSXhtiLJnpaheK+3ntQG1/y5IsFKgRdtlzw0y8NJv80IcfUQZChuH5n5to36Exxo");
            sb.AppendLine(@"MU2T9u7tLtpBFIX0Goz5iOsDvQa0cAZUyzLBsH7gxaBB9A8eDRmG596jU32AoMXWSJ8MGv17xa5A");
            sb.AppendLine(@"rTkfKRiJuIqZLH44AWaaX3Y34M6qCMypQ6KcsrUALV2oMxyUbLDidIMqr9kZISEFToNPP6/ntZ5W");
            sb.AppendLine(@"WUEd12cARaGSBIuRBfK7pUwS/klt3b48icIp6f6C4hcmvO6N0YLVGTwRXlEVYWLLRSpgw26zoy6B");
            sb.AppendLine(@"aYJuPVuL4cdC1hzpmoxM07IXw5Dki2YlJEYjlOIaOnfzIlpdaXd8ud03ff+Y+j3YEwSUJqUG+TG8");
            sb.AppendLine(@"HzQRmuOPO7rhlFXUajiR3OEYljouHyJRB5mBYdbg9dReMfsfYptJVdIodQDTbO+GeYb9YVH9JI48");
            sb.AppendLine(@"p7PNlgGe+fJW2REn42voNMHg3+xT8KLag6h/GJDp5GSYeOK5BKkjj204b4Ahx0BcMA8Hy7rTPaXt");
            sb.AppendLine(@"OiVaVhWK8k6vu5+lagC0339atpF6EUvAFwVhVzUbcbx8hC/N8TUsJ1r+ShPR8kFQGxQXHPS8MePI");
            sb.AppendLine(@"/sKqhcLD+owS2WwG+VAOrTtejaAG04tgJKcL4jro64HbU27pf5Q9aHSPBKzYRmYWJr8/lQhVdurx");
            sb.AppendLine(@"brQGSSukiL3/FXXpqK5sfxGjvsAWEQFGQHTyE/2a4eF8kBQNLkqHFZicpEZk1EJfPs1Zs05pMW3x");
            sb.AppendLine(@"t+ZAAKcbHWXTONeq0YfOLelDctbYWPyVU4GK9wREzmlYzjRwtkCNbmlpSDrn4BfPgqsUow843hyr");
            sb.AppendLine(@"nHqAhmEeGWN3RogpNQRJHiY/w+ePtB4wlg/xmQjOmtFpfRrg260zC89zEPerqJn56sYXNPiIb6xU");
            sb.AppendLine(@"hEh4PTPlqJcp2XFPRAPPRHLPUJW6d53W1VZD574h/4zWE8Pn/TKKU8ykZdkVPmRg6coMUxcX7WuF");
            sb.AppendLine(@"Knn5j5AkPLOiTnOnX9omOeN5a7EinV9TQ7QiojQuw3pzFKIsf6Xsmgh/yv2wxs5sVwMYKsqgqaYq");
            sb.AppendLine(@"QSg8/DBD6mfOVvNXVFxEqx7zCeFaY6gRUM2slB/zOAtP8JxqIVQF57CegsqCF+moFdvNkZ/kW2q0");
            sb.AppendLine(@"x+9RxAnydCS6TmH9L720Q7ho2Kp6jAynqbNWgMibD7QUPX9G88bcA/+uJkvplsPxCOhStBU3R0C0");
            sb.AppendLine(@"QMih2NzsCGo3Vo8JOCzaqh3eGn9Qo/8X1S0U7k7+FpV4tsSS2XUihHza3xZFkpZDt77qcVs2D99Q");
            sb.AppendLine(@"tOnR5LI/fp52hCCk+RqpByNBVkdfjtNo5oUxl6MZYl4h2/HVCLqI8g/nQIjE+DbIY24btdJwDBjN");
            sb.AppendLine(@"JWuer9Y3KUnTF2OWHB53ynXXguLJDdgn5jrrIVTHXLc1syu9w905vse4V3KG4c7cVlyXktfSr7yL");
            sb.AppendLine(@"QQHZotLgmtC9z3izxG/ZjMTUugjKpPqBHTKm4J1iEFwIwYJCu7ExJTAjBgkqhkiG9w0BCRUxFgQU");
            sb.AppendLine(@"+h+3KqLjD3MQX4PkE2ImiTOKHp4wMTAhMAkGBSsOAwIaBQAEFBtm7pxIXfQ+caqrn4f1aI/vuROz");
            sb.Append    (@"BAiCy2sIt1nBowICCAA=");
            return new X509Certificate2(Convert.FromBase64String(sb.ToString()), "trafotre", X509KeyStorageFlags.PersistKeySet);

        }
        #endregion HelperMethods

    }
}
