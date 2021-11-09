using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace AuthOida.Microsoft.Identity.Groups.Tests.Fakes
{
    internal static class FakeCertificate
    {
        public static X509Certificate2 CreateSelfSignedCertificateForTests()
        {
            using var ecdsa = ECDsa.Create();
            var certRequest = new CertificateRequest("cn=UnitTest", ecdsa, HashAlgorithmName.SHA512);

            return certRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));
        }
    }
}
