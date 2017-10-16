using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using Seges.IdentityModel.WebApi;
using Serilog;

namespace Seges.IdentityModel.Tests
{

    [TestFixture]
    public class WebApiClientTests
    {
        public WebApiClientTests()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();
        }

        private class LdkDeflatedSamlWebApiConfiguration : DeflatedSamlWebApiConfiguration
        {
            public LdkDeflatedSamlWebApiConfiguration(WsTrustTokenProvider tokenProvider) : base(tokenProvider)
            {
            }
        }

        [Test]
        public void CanCallApi()
        {
            var configuration = new LdkDeflatedSamlWebApiConfiguration(new WsTrustTokenProvider(
                new WsTrustConfiguration(
                    username: "cvruser1",
                    password: "Pass1word",
                    audience: "https://devtest-www-ldk3.vfltest.dk",
                    adfsDns: "si-idp.vfltest.dk",
                    tokenCacheTime: TimeSpan.FromMinutes(15)
                )))
            {
                Endpoint = null//new Uri("https://devtest-www-ldk3.vfltest.dk/")
            };


            var sut = new WebApiClient<LdkDeflatedSamlWebApiConfiguration>(
                configuration,
                new HttpClient());
            var response = sut.Get<LdkCurrentUser>("/Profile/CurrentUser").Result;
            Assert.IsNotNull(response);
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("cvruser1@PROD.DLI", response.Typed.nameidentifier.SingleOrDefault());

            response = sut.Get<LdkCurrentUser>("/Profile/CurrentUser").Result;
            Assert.IsNotNull(response);
            Assert.AreEqual("cvruser1@PROD.DLI", response.Typed.nameidentifier.SingleOrDefault());
        }
    }

    public class LdkCurrentUser
    {
        public List<string> nameidentifier { get; set; }
        public List<string> role { get; set; }
        public List<string> givenname { get; set; }
        public List<string> surname { get; set; }
        public List<string> upn { get; set; }
        public List<string> streetname { get; set; }
        public List<string> streetnumber { get; set; }
        public List<string> stairway { get; set; }
        public List<string> floor { get; set; }
        public List<string> door { get; set; }
        public List<string> postalcode { get; set; }
        public List<string> locality { get; set; }
        public List<string> homephone { get; set; }
        public List<string> mobilephone { get; set; }
        public List<string> cvrnumber { get; set; }
        public List<string> unvalidatedcvrnumber { get; set; }
        public List<string> windowsaccountname { get; set; }
        public List<string> isfulltimefarmer { get; set; }
        public List<string> hasfulltimeemployees { get; set; }
        public List<string> conventional { get; set; }
        public List<string> lmonemloen { get; set; }
        public List<string> authenticationmethod { get; set; }
        public List<DateTime> authenticationinstant { get; set; }
    }
}
