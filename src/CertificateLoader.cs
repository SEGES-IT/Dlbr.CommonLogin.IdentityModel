using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Dlbr.CommonLogin.IdentityModel
{
    public class CertificateLoader
    {
        public static X509Certificate2 GetCertificateByThumbprint(StoreName name, StoreLocation location, string thumbPrint)
        {
            Func<X509Certificate2, bool> predicate =
                certificate => NormalizeThumbprint(certificate.Thumbprint) == NormalizeThumbprint(thumbPrint);
            return GetCertificate(name, location, predicate);
        }

        public static X509Certificate2 GetCertificate(StoreName name, StoreLocation location, Func<X509Certificate2,bool> predicate )
        {
            X509Store store = null;
            IEnumerable<X509Certificate2> matchingCerts = new X509Certificate2[0];
            var certificates = new List<X509Certificate2>();
            try
            {
                store = new X509Store(name, location);
                store.Open(OpenFlags.ReadOnly);

                // Every time we call store.Certificates property, a new collection will be returned.
                certificates = store.Certificates.Cast<X509Certificate2>().ToList();

                matchingCerts = certificates.Where(predicate);

                if (!matchingCerts.Any())
                {
                    throw new ApplicationException("No certificate was found for criteria");
                }
                
                if (matchingCerts.Count() > 1)
                {
                    throw new ApplicationException("Multiple certificates match criteria");
                }
                return matchingCerts.Single();
            }
            finally
            {
                foreach (var x509Certificate2 in certificates.Except(matchingCerts))
                {
                    x509Certificate2.Reset();
                }
                if (store != null)
                {
                    store.Close();
                }
            }
        }

        private static string NormalizeThumbprint(string thumbPrint)
        {
            return Regex.Replace(thumbPrint.ToUpperInvariant(), @"[\W]", "");
        }
    }
}