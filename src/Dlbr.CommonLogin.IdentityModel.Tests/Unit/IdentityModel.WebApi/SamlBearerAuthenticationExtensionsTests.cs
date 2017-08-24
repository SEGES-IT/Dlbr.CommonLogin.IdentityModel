using System;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Dlbr.CommonLogin.IdentityModel.Owin;
using Dlbr.CommonLogin.IdentityModel.Tests.Infrastructure.TestSts;
using Dlbr.CommonLogin.IdentityModel.WebApi;
using Dlbr.CommonLogin.Owin;
using FluentAssertions;
using Microsoft.Owin;
using Microsoft.Owin.Testing;
using NUnit.Framework;
using Owin;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.IdentityModel.WebApi
{

    [TestFixture]
    [Category("BuildVerification")]
    public class SamlBearerAuthenticationExtensionsTests
    {
        private const string User = "SomeUserWithI8NCharsInNameæøåÆØÅ";
        private const string SimulatedAuthorizingControllerActionOutput = "Response requiring a correctly authenticated user";
        private const string Audience = "https://example.com/someaudience/";

        [Test]
        public void UseDeflatedSaml11BearerAuthentication_CalledWithValidToken_CanGenerateResponse()
        {
            var sts = Infrastructure.TestSts.TestSts.Create();
            
            var audience = new Uri(Audience);
            var principal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, User)})});
            var issuerNameRegistry = sts.IssuerNameRegistry;
            SetIssuerNameRegistryAndAudience(issuerNameRegistry, audience);

            using (var server = TestServer.Create(app =>
            {
                app.UseDeflatedSamlBearerAuthentication(WifTokenValidatorFactory.CreateWindowsIdentityFoundationTokenValidator());
                app.Run(SimulatedAuthorizingControllerAction);
            }))
            {
                var httpClient = server.HttpClient;
                var tokenXml = sts.IssueTokenAsXml(principal, audience.AbsoluteUri);
                AddAuthorizationHeader(httpClient, tokenXml);

                var response = httpClient.GetAsync("/").Result;
                var result = response.Content.ReadAsStringAsync().Result;
                result.Should().Be(SimulatedAuthorizingControllerActionOutput);
            }
        }

        [Test]
        public void UseDeflatedSaml11BearerAuthentication_CalledWithUnrecognizedToken_ThrowsUnauthorizedException()
        {
            var sts = Infrastructure.TestSts.TestSts.Create();

            var audience = new Uri(Audience);
            var principal = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, User) }) });

            var issuerNameRegistry = new SimpleValidateByThumbprintOnlyIssuerNameRegistry(
                new string('F', sts.IssuerNameRegistry.AcceptedThumbprint.Length),
                "http://nonexistingissuer");
            
            SetIssuerNameRegistryAndAudience(issuerNameRegistry, audience);

            using (var server = TestServer.Create(app =>
            {
                app.UseDeflatedSamlBearerAuthentication(WifTokenValidatorFactory.CreateWindowsIdentityFoundationTokenValidator());
                app.Run(SimulatedAuthorizingControllerAction);
            }))
            {
                var httpClient = server.HttpClient;
                var tokenXml = sts.IssueTokenAsXml(principal, audience.AbsoluteUri);
                AddAuthorizationHeader(httpClient, tokenXml);

                var exception = Assert.Throws<AggregateException>(
                    () =>
                    {
                        var response = httpClient.GetAsync("/").Result;
                    }
                );
                Console.WriteLine(exception.Message);
                exception.InnerException.Should().BeOfType<UnauthorizedAccessException>();
                Console.WriteLine(exception.InnerException.Message);
            }
        }

        [Test]
        public void UseDeflatedSaml11BearerAuthentication_CalledWithInvalidToken_ThrowsUnauthorizedException()
        {
            var sts = Infrastructure.TestSts.TestSts.Create();

            var audience = new Uri(Audience);
            var principal = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, User) }) });

            var issuerNameRegistry = new SimpleValidateByThumbprintOnlyIssuerNameRegistry(
                new string('F', sts.IssuerNameRegistry.AcceptedThumbprint.Length),
                "http://nonexistingissuer");

            SetIssuerNameRegistryAndAudience(issuerNameRegistry, audience);

            using (var server = TestServer.Create(app =>
            {
                app.UseDeflatedSamlBearerAuthentication(WifTokenValidatorFactory.CreateWindowsIdentityFoundationTokenValidator());
                app.Run(SimulatedAuthorizingControllerAction);
            }))
            {
                var httpClient = server.HttpClient;
                AddAuthorizationHeader(httpClient, "<xml>Jægerbogen på arabisk: كتاب صياد</xml>");

                var exception = Assert.Throws<AggregateException>(
                    () =>
                    {
                        var response = httpClient.GetAsync("/").Result;
                    }
                );
                Console.WriteLine(exception.Message);
                exception.InnerException.Should().BeOfType<UnauthorizedAccessException>();
                Console.WriteLine(exception.InnerException.Message);
            }
        }


        [Test]
        public void UseDeflatedSaml11BearerAuthentication_CalledWithNoToken_ThrowsUnauthorizedException()
        {
            var sts = Infrastructure.TestSts.TestSts.Create();

            var audience = new Uri(Audience);

            var issuerNameRegistry = sts.IssuerNameRegistry;

            using (var server = TestServer.Create(app =>
            {
                //app.UseDeflatedSaml11BearerAuthentication(audience, issuerNameRegistry);
                app.Run(SimulatedAuthorizingControllerAction);
            }))
            {
                var httpClient = server.HttpClient;

                var exception = Assert.Throws<AggregateException>(
                    () =>
                    {
                        var response = httpClient.GetAsync("/").Result;
                    }
                );
                Console.WriteLine(exception.Message);
                exception.InnerException.Should().BeOfType<UnauthorizedAccessException>();
                Console.WriteLine(exception.InnerException.Message);
            }
        }

        private static void AddAuthorizationHeader(HttpClient httpClient, string tokenXml)
        {
            var tokenEncoder = new DeflatedSamlTokenHeaderEncoder();
            var encodedTokenXml = tokenEncoder.Encode(tokenXml);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", encodedTokenXml);
        }

        private static Task SimulatedAuthorizingControllerAction(IOwinContext context)
        {
            ClaimsPrincipal currentUser = context.Authentication.User;
            if (currentUser == null || !currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Null or unauthenticated principal");
            }
            var name = currentUser.Identity.Name;
            if (name != User)
            {
                throw new UnauthorizedAccessException(string.Format("Wrong identity, expected {0}, got {1}", User, name));
            }
            return context.Response.WriteAsync(SimulatedAuthorizingControllerActionOutput);
        }

        private static void SetIssuerNameRegistryAndAudience(
            SimpleValidateByThumbprintOnlyIssuerNameRegistry issuerNameRegistry, Uri audience)
        {
            FederatedAuthentication.FederationConfiguration.IdentityConfiguration.IssuerNameRegistry = issuerNameRegistry;
            FederatedAuthentication.FederationConfiguration.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris.Clear();
            FederatedAuthentication.FederationConfiguration.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris.Add(
                audience);
        }
    }
}


