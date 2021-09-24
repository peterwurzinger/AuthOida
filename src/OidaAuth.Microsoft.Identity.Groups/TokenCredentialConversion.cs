﻿using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Web;
using System;
using System.Linq;

namespace OidaAuth.Microsoft.Identity.Groups
{
    public static class TokenCredentialConversion
    {
        public static TokenCredential FromIdentityOptions(MicrosoftIdentityOptions identityOptions)
        {
            if (identityOptions is null)
                throw new ArgumentNullException(nameof(identityOptions));

            if (identityOptions.ClientCertificates?.Any() ?? false)
                return new ClientCertificateCredential(identityOptions.TenantId, identityOptions.ClientId, identityOptions.ClientCertificates.First().Certificate);
            
            if (identityOptions.ClientSecret is not null)
                return new ClientSecretCredential(identityOptions.TenantId, identityOptions.ClientId, identityOptions.ClientSecret);

            throw new NotImplementedException("Conversion to TokenCredential is only implemented for ClientSecret and ClientCertificates.");
        }
    }
}
