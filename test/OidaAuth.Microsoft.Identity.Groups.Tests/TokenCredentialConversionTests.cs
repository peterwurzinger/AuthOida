using Azure.Identity;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using Xunit;

namespace OidaAuth.Microsoft.Identity.Groups.Tests
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
            var identityOptions = new MicrosoftIdentityOptions
            {
                TenantId = "Tenant1234",
                ClientId = "ClientId1234",
                ClientCertificates = new List<CertificateDescription>()
                {
                    CertificateDescription.FromCertificate(new())
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
            var identityOptions = new MicrosoftIdentityOptions
            {
                TenantId = "Tenant1234",
                ClientId = "ClientId1234",
                ClientSecret = "Pssst,Secret",
                ClientCertificates = new List<CertificateDescription>()
                {
                    CertificateDescription.FromCertificate(new())
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
