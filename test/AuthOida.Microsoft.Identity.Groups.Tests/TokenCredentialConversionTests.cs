using System;
using System.Collections.Generic;
using AuthOida.Microsoft.Identity.Groups.Tests.Fakes;
using Azure.Identity;
using Microsoft.Identity.Web;
using Xunit;

namespace AuthOida.Microsoft.Identity.Groups.Tests;

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
        using var fakeCert = FakeCertificate.CreateSelfSignedCertificateForTests();
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
        using var fakeCert = FakeCertificate.CreateSelfSignedCertificateForTests();
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

        var exception = Assert.Throws<NotImplementedException>(() => TokenCredentialConversion.FromIdentityOptions(identityOptions));
        Assert.Equal("Conversion to TokenCredential is only implemented for ClientSecret and ClientCertificates.", exception.Message);
    }
}
