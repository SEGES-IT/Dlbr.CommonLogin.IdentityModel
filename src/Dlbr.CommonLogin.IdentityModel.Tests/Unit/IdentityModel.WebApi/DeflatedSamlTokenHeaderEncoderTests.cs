using Dlbr.CommonLogin.IdentityModel.WebApi;
using FluentAssertions;
using NUnit.Framework;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.IdentityModel.WebApi
{
    
    [TestFixture]
    [Category("BuildVerification")]
    public class DeflatedSamlTokenHeaderEncoderTests
    {
        private const string ArbitraryStringWithI8NChars = "a b c d e f æ ø å Æ Ø Å !";

        [Test]
        public void Encode_StringWithI8NChars_RoundtripsCorrectly()
        {
            var tokenHeaderEncoder = new DeflatedSamlTokenHeaderEncoder();

            var encodedTokenHeader = tokenHeaderEncoder.Encode(ArbitraryStringWithI8NChars);

            var decodedTokenHeader = tokenHeaderEncoder.Decode(encodedTokenHeader);

            decodedTokenHeader.Should().Be(ArbitraryStringWithI8NChars);
        }
    }
}

        
