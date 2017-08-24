//using System;
//using System.Collections.Specialized;
//using System.IdentityModel.Services;
//using System.Web;
//using System.Web.Util;
//using Dlbr.CommonLogin.IdentityModel.Web;
//using NUnit.Framework;
//using TypeMock.ArrangeActAssert;

//namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.IdentityModel.Web
//{
//    [TestFixture]
//    class WsFederationAwareRequestValidatorTests
//    {
//        [Test]
//        [Isolated][Category("TypeMock")]
//        public void IsValidRequestString_SigninMessageProvided_ReturnsTrue()
//        {
//            // Arrange
//            TestableWsFederationAwareRequestValidator validator = new TestableWsFederationAwareRequestValidator();
//            HttpContext context = Isolate.Fake.Instance<HttpContext>();

//            Isolate.WhenCalled(() => WSFederationMessage.CreateFromNameValueCollection(null, null)).WillReturn(Isolate.Fake.Instance<SignInResponseMessage>());
//            Uri uri = new Uri("http://localhost.vfltest.dk/test/");
//            Isolate.WhenCalled(() => WSFederationMessage.GetBaseUrl(uri)).WillReturn(uri);

//            // Act
//            int validationFailureIndex;
//            bool result = validator.TestIsValidRequestString(context, null, RequestValidationSource.Form, "wresult", out validationFailureIndex);

//            // Assert
//            Assert.IsTrue(result);
//        }

//        [Test]
//        [Isolated][Category("TypeMock")]
//        public void IsValidRequestString_NotRequestValidationForm_CallsBase()
//        {
//            // Arrange
//            TestableWsFederationAwareRequestValidator validator = new TestableWsFederationAwareRequestValidator();
//            HttpContext context = Isolate.Fake.Instance<HttpContext>();

//            // Act
//            int validationFailureIndex;
//            bool result = validator.TestIsValidRequestString(context, null, RequestValidationSource.Headers, "wresult", out validationFailureIndex);

//            // Assert
//            Assert.IsTrue(result);
//        }

//    }

//    internal class TestableWsFederationAwareRequestValidator : WsFederationAwareRequestValidator
//    {
//        public bool GetUnvalidatedFormValuesHasBeenCalled = false;

//        public bool TestIsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
//        {
//            bool result = base.IsValidRequestString(context, value, requestValidationSource, collectionKey, out validationFailureIndex);
//            return result;
//        }

//        protected override NameValueCollection GetUnvalidatedFormValues(HttpContext context)
//        {
//            GetUnvalidatedFormValuesHasBeenCalled = true;
//            return new NameValueCollection();
//        }
//    }

//}
