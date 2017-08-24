using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Dlbr.CommonLogin.IdentityModel.WebApi;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;

namespace Dlbr.CommonLogin.Owin
{
    public class DeflatedSamlStrippingTokenFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly Func<string, ClaimsPrincipal> _tokenValidator;
        private readonly ILogger _logger;
        private readonly DeflatedSamlTokenHeaderEncoder _decoder;

        public DeflatedSamlStrippingTokenFormat(Func<string,ClaimsPrincipal> tokenValidator, ILogger logger)
        {
            if (tokenValidator == null) throw new ArgumentNullException("tokenValidator");
            if (logger == null) throw new ArgumentNullException("logger");
            _tokenValidator = tokenValidator;
            _logger = logger;
            _decoder = new DeflatedSamlTokenHeaderEncoder();
        }

        public string Protect(AuthenticationTicket data)
        {
            throw new NotImplementedException("DeflatedSamlStrippingTokenFormat only supports decoding tokens. Use DeflatedSamlTokenHeaderEncoder for encoding.");
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            _logger.WriteVerbose("Enter AuthenticationTicket.Unprotect");
            _logger.WriteVerbose(string.Format("protectedText: {0}", protectedText));
            if (string.IsNullOrWhiteSpace(protectedText))
            {
                throw new ArgumentNullException("protectedText");
            }

            _logger.WriteVerbose("Decoding protectedText");
            var tokenString = _decoder.Decode(protectedText);
            if (_logger.IsEnabled(TraceEventType.Verbose))
            {
                _logger.WriteVerbose("Raw token:");
                _logger.WriteVerbose(tokenString);
            }

            _logger.WriteVerbose("Reading and validating token string");
            var principal = _tokenValidator(tokenString);
            _logger.WriteVerbose("ClaimsPrincipal validated.");
            if (_logger.IsEnabled(TraceEventType.Verbose))
            {
                foreach (var id in principal.Identities)
                {
                    int nestingLevel = 1;
                    var current = id;
                    do
                    {
                        _logger.WriteVerbose(string.Format("Identity at nesting level {0}", nestingLevel));
                        foreach (var claim in id.Claims)
                        {
                            _logger.WriteVerbose(string.Format("{0} {1}", claim.Type, claim.Value));
                        }
                        nestingLevel++;
                        current = current.Actor;
                    } while (current != null);
                }
            }

            _logger.WriteVerbose("Issuing ticket");
            var ticket = new AuthenticationTicket(principal.Identities.First(), new AuthenticationProperties());
            _logger.WriteVerbose("Exit AuthenticationTicket.Unprotect");
            return ticket;
        }
    }
}