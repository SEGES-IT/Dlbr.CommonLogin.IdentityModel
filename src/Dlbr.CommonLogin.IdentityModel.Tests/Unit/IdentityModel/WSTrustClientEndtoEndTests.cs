using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Dlbr.CommonLogin.IdentityModel.Windows;
using NUnit.Framework;


namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.IdentityModel
{
    [TestFixture]
    public class WSTrustClientEndtoEndTests
    {
        private const string DcfRealm = "https://dev.dcf.ws.dlbr.dk/DCFServices/";
        private const string DcfUser = "DCFKVKService";
        private const string DcfPassword = "dcfkvk898";
        private const string CustomerSampleRealm = "https://localhost.vfltest.dk/CustomerSampleTier1Service/";
        private const string CustomerSampleEndUserUsername = "cvr01";
        private const string CustomerSampleEndUserPassword = "pass1word";
        private const string CustomerSampleActAsUserUsername = "customersampleactas";
        private const string CustomerSampleActAsUserPassword = "pass1word";


        [Test]
        public void GetSecurityTokenWithUserName_DevTestIdpAndDcf_TokenReturned()
        {
            var wsTrustClient = new WsTrustClient("devtest-idp.vfltest.dk");
            var token = wsTrustClient.GetSecurityToken(DcfRealm, DcfUser, DcfPassword);
            Assert.IsNotNull(token);
            Console.WriteLine(token.Id);
        }

        [Test]
        public void GetSecurityTokenAsyncWithUserName_DevTestIdpAndDcf_TokenReturnedWhenBlockingOnResult()
        {
            var wsTrustClient = new WsTrustClient("devtest-idp.vfltest.dk");
            var token = wsTrustClient.GetSecurityTokenAsync(DcfRealm, DcfUser, DcfPassword).Result;
            Assert.IsNotNull(token);
            Console.WriteLine(token.Id);
        }

        [Test]
        public async Task GetSecurityTokenAsyncWithUserName_DevTestIdpAndDcf_TokenReturnedToAsyncCalled()
        {
            var wsTrustClient = new WsTrustClient("devtest-idp.vfltest.dk");
            var token = await wsTrustClient.GetSecurityTokenAsync(DcfRealm, DcfUser, DcfPassword);
            Assert.IsNotNull(token);
            Console.WriteLine(token.Id);
        }

        [Test]
        public async Task GetSecurityTokenAsyncWithUserName_DevTestIdpAndDcf_TokenResponseReturnedToAsyncCalled()
        {
            var wsTrustClient = new WsTrustClient("devtest-idp.vfltest.dk");
            var tokenResponse = await wsTrustClient.GetSecurityTokenResponseAsync(DcfRealm, DcfUser, DcfPassword);
            Assert.IsNotNull(tokenResponse);
            Assert.IsNotNull(tokenResponse.Token);
            Assert.IsNotNull(tokenResponse.Rstr);
            Console.WriteLine(tokenResponse.Token.Id);
            Console.WriteLine(tokenResponse.Rstr);
        }

        [Test]
        public async Task GetActAsSecurityTokenAsyncWithUserName_SiIdpAndRpTesterCredentials_TokenResponseReturnedToAsyncCalled()
        {
            var wsTrustClient = new WsTrustClient("si-idp.vfltest.dk");
            var enduserTokenResponse =
                await wsTrustClient.GetSecurityTokenResponseAsync(
                        CustomerSampleRealm, 
                        CustomerSampleEndUserUsername,
                        CustomerSampleEndUserPassword);
            var actAsTokenResponse = await wsTrustClient.GetActAsTokenResponseAsync(
                CustomerSampleRealm,
                CustomerSampleActAsUserUsername,
                CustomerSampleActAsUserPassword,
                enduserTokenResponse.Token);
            Assert.IsNotNull(actAsTokenResponse);
            Assert.IsNotNull(actAsTokenResponse.Token);
            Assert.IsNotNull(actAsTokenResponse.Rstr);
            Console.WriteLine(actAsTokenResponse.Token.Id);
            Console.WriteLine(actAsTokenResponse.Rstr);
        }
    }
}