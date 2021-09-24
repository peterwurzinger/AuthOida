using Azure.Identity;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace AuthOida.Microsoft.Identity.Groups.Tests
{
    public class TokenCredentialConversionTests
    {
        [Fact]
        public void FromIdentityOptionsThrowsIfIdentityOptionsNull()
        {
            Assert.Throws<ArgumentNullException>("identityOptions", () => TokenCredentialConversion.FromIdentityOptions(null!));
        }

        [Fact]
        public void FromIdentityOptionsShouldReturnClientCertificateCredentialIfClientCertificatesNotEmpty()
        {
            using var fakeCert = new X509Certificate2();
            var identityOptions = new MicrosoftIdentityOptions
            {
                TenantId = "Tenant1234",
                ClientId = "ClientId1234",
                ClientCertificates = new List<CertificateDescription>()
                {
                    CertificateDescription.FromCertificate(fakeCert)
                }
            };

            var result = TokenCredentialConversion.FromIdentityOptions(identityOptions);

            Assert.IsType<ClientCertificateCredential>(result);
        }

        [Fact]
        public void FromIdentityOptionsShouldReturnClientSecretCredentialIfClientSecretNotNull()
        {
            var identityOptions = new MicrosoftIdentityOptions
            {
                TenantId = "Tenant1234",
                ClientId = "ClientId1234",
                ClientSecret = "Pssst,Secret"
            };

            var result = TokenCredentialConversion.FromIdentityOptions(identityOptions);

            Assert.IsType<ClientSecretCredential>(result);
        }

        [Fact]
        public void FromIdentityOptionsShouldPreferClientCertificateOverClientSecret()
        {
            using var fakeCert = new X509Certificate2();
            var identityOptions = new MicrosoftIdentityOptions
            {
                TenantId = "Tenant1234",
                ClientId = "ClientId1234",
                ClientSecret = "Pssst,Secret",
                ClientCertificates = new List<CertificateDescription>()
                {
                    CertificateDescription.FromCertificate(fakeCert)
                }
            };

            var result = TokenCredentialConversion.FromIdentityOptions(identityOptions);

            Assert.IsType<ClientCertificateCredential>(result);
        }

        [Fact]
        public void FromIdentityOptionsThrowsIfNeitherClientSecretNorClientCertificateIsSet()
        {
            var identityOptions = new MicrosoftIdentityOptions
            {
                TenantId = "Tenant1234",
                ClientId = "ClientId1234"
            };

            Assert.Throws<NotImplementedException>(() => TokenCredentialConversion.FromIdentityOptions(identityOptions));
        }
    }
}
