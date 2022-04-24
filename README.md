AuthOida
==========

[![Latest package](https://img.shields.io/nuget/v/AuthOida.Microsoft.Identity.Groups.svg)](https://www.nuget.org/packages/AuthOida.Microsoft.Identity.Groups)
[![Download tracker](https://img.shields.io/nuget/dt/AuthOida.Microsoft.Identity.Groups.svg)](https://www.nuget.org/packages/AuthOida.Microsoft.Identity.Groups)
[![GitHub status](https://github.com/peterwurzinger/AuthOida/workflows/everything/badge.svg)](https://github.com/peterwurzinger/AuthOida/actions)
[![Code coverage](https://codecov.io/gh/peterwurzinger/AuthOida/branch/main/graph/badge.svg)](https://codecov.io/gh/peterwurzinger/AuthOida)
[![Mutation score](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Fpeterwurzinger%2FAuthOida%2Frefs%2Fheads%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/peterwurzinger/AuthOida/refs/heads/main)

*AuthOida* closes a gap in *Microsoft.Identity.Web*, where Azure Active Directory group
assignments for an identity are only appended to the token using the groups `ObjectID` instead of
their display name. Refer to [StackOverflow](https://stackoverflow.com/questions/65146210/azure-ad-show-group-name-in-id-token-instead-of-group-id)
and a [GitHub issue](https://github.com/MicrosoftDocs/azure-docs/issues/59766) for some details.

# Usage
*AuthOida* supports WebApps and API scenarios likewise, as long as `Microsoft.Identity.Web` is
used for authentication. For both use cases *AuthOida* provides an extension method
`AddMappedGroups()` as primary hook.

***Important:*** *AuthOida* makes use of [Microsoft Graph](https://docs.microsoft.com/en-us/graph/overview) to query
the groups display names after the the JWT/OIDC token has been validated. Therefore the respective App
registration needs to configure
[Group.Read.All](https://docs.microsoft.com/en-us/graph/permissions-reference#group-permissions)
**Application permissions** for the Microsoft Graph API, so that the application can read group details.
Please note, that Delegated permissions are not yet supported, and that there are not plans for it
to be implemented.

Since the application itself also acts as a confidential application when querying the Graph API, you
need to either specify a `ClientSecret` or provide at least one `ClientCertificate` within the
`MicrosoftIdentityOptions` configuration. *AuthOida* will reuse it to authenticate with Microsoft
Graph. (Please note, that if both are specified, *AuthOida* will authenticate using `ClientCertificate` in favor of
`ClientSecret`.)

## For WebApps
Kindly refer to `samples/AuthOida.Sample.App` for a runnable sample of a WebApp using `AuthOida`.
```csharp
services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
        .AddMappedGroups();
```

## For APIs
Kindly refer to `samples/AuthOida.Sample.API` for a runnable sample of an API using `AuthOida`.
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"))
        .AddMappedGroups();
```

# Considerations

## Performance
*AuthOida* makes at least one API call to MS Graph querying all groups of an AD instance
for their display names during the lifetime of an application instance. Therefore the first request
to a new instance *could potentially* experience a significant response delay. I only tested it with
an AD instance containing a rather small number of groups, so any experience with bigger AD instances
would be highly appreciated.

*Consider* prewarming instances, so that subsequent requests will fallback to the cache, if you
experience significant delays on the first request.

## Caching
*AuthOida* builds up a lookup dictionary, that is bound to the application lifetime. As long as the
application is loaded, *AuthOida* will reuse the previously queried lookup pairs.

*Note:* *AuthOida* does not attempt to refresh this cache, as groups display names are considered to be
rather static.

## Thread Safety
*AuthOida* is not thread safe in that sense, that for `n` simultaneously arriving first requests there
could be a race, where the application groups would be queried `n` times instead of only once.
This does not cause any harm, as the caches would simply be replaced with one another.

## On-premise Groups
Active Directory groups, that are synced from an on-premise instance most likely do not suffer from
the issue described above. As of now, the groups display names are only omitted for Azure AD security
group instances.