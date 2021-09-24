using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System;
using System.Security.Claims;

namespace AuthOida.Microsoft.Identity.Groups
{
    /// <summary>
    /// Extensions for authentication builders of Microsoft.Identity.Web (<see href="https://github.com/AzureAD/microsoft-identity-web"/>)
    /// </summary>
    public static class AuthenticationBuilderExtensions
    {
        /// <summary>
        /// Maps group Ids contained in a JWT token issued by Azure Active Directory to their display name by
        /// performing a lookup with Microsoft Graph and creating an additional <see cref="ClaimsIdentity"/>.
        /// </summary>
        /// <param name="authenticationBuilder">The authentication builder provided by Microsoft.Identity.Web</param>
        /// <param name="configureOptions">The action used to configure the <see cref="GroupsMappingOptions"/></param>
        /// <param name="jwtBearerScheme">The authentication scheme. Defaults to <see cref="JwtBearerDefaults.AuthenticationScheme"/></param>
        /// <returns>The <paramref name="authenticationBuilder"/></returns>
        /// <exception cref="ArgumentNullException">When <paramref name="authenticationBuilder"/> is null</exception>
        /// <exception cref="ArgumentException">When <paramref name="jwtBearerScheme"/> is null or empty</exception>
        public static MicrosoftIdentityWebApiAuthenticationBuilder AddMappedGroups(this MicrosoftIdentityWebApiAuthenticationBuilder authenticationBuilder, Action<GroupsMappingOptions>? configureOptions = null, string jwtBearerScheme = JwtBearerDefaults.AuthenticationScheme)
        {
            if (authenticationBuilder is null)
                throw new ArgumentNullException(nameof(authenticationBuilder));

            if (string.IsNullOrEmpty(jwtBearerScheme))
                throw new ArgumentException($"'{nameof(jwtBearerScheme)}' cannot be null or empty.", nameof(jwtBearerScheme));

            AddMappedGroupsServices(authenticationBuilder.Services, jwtBearerScheme, configureOptions);

            authenticationBuilder.Services.Configure<JwtBearerOptions>(jwtBearerScheme, options =>
            {
                var onTokenValidated = options.Events.OnTokenValidated;
                options.Events.OnTokenValidated = async (ctx) =>
                {
                    await onTokenValidated(ctx).ConfigureAwait(false);
                    var groupsMapper = ctx.HttpContext.RequestServices.GetRequiredService<GroupsMapper>();
                    await groupsMapper.AugmentPrincipalWithMappedRoles(jwtBearerScheme, ctx.Principal).ConfigureAwait(false);
                };
            });
            return authenticationBuilder;
        }

        /// <summary>
        /// Maps group Ids contained in an OpenID Connect token issued by Azure Active Directory to their display name by
        /// performing a lookup with Microsoft Graph and creating an additional <see cref="ClaimsIdentity"/>.
        /// </summary>
        /// <param name="authenticationBuilder">The authentication builder provided by Microsoft.Identity.Web</param>
        /// <param name="configureOptions">The action used to configure the <see cref="GroupsMappingOptions"/></param>
        /// <param name="openIdConnectScheme">The authentication scheme. Defaults to <see cref="OpenIdConnectDefaults.AuthenticationScheme"/></param>
        /// <returns>The <paramref name="authenticationBuilder"/></returns>
        /// <exception cref="ArgumentNullException">When <paramref name="authenticationBuilder"/> is null</exception>
        /// <exception cref="ArgumentException">When <paramref name="openIdConnectScheme"/> is null or empty</exception>
        public static MicrosoftIdentityWebAppAuthenticationBuilder AddMappedGroups(this MicrosoftIdentityWebAppAuthenticationBuilder authenticationBuilder, Action<GroupsMappingOptions>? configureOptions = null, string openIdConnectScheme = OpenIdConnectDefaults.AuthenticationScheme)
        {
            if (authenticationBuilder is null)
                throw new ArgumentNullException(nameof(authenticationBuilder));

            if (string.IsNullOrEmpty(openIdConnectScheme))
                throw new ArgumentException($"'{nameof(openIdConnectScheme)}' cannot be null or empty.", nameof(openIdConnectScheme));

            AddMappedGroupsServices(authenticationBuilder.Services, openIdConnectScheme, configureOptions);

            authenticationBuilder.Services.Configure<OpenIdConnectOptions>(openIdConnectScheme, options =>
            {
                var onTokenValidated = options.Events.OnTokenValidated;
                options.Events.OnTokenValidated = async (ctx) =>
                {
                    await onTokenValidated(ctx).ConfigureAwait(false);
                    var groupsMapper = ctx.HttpContext.RequestServices.GetRequiredService<GroupsMapper>();
                    await groupsMapper.AugmentPrincipalWithMappedRoles(openIdConnectScheme, ctx.Principal).ConfigureAwait(false);
                };
            });

            return authenticationBuilder;
        }

        private static void AddMappedGroupsServices(IServiceCollection services, string authenticationScheme, Action<GroupsMappingOptions>? configureOptions)
        {
            if (configureOptions is null)
                configureOptions = o => { };

            services.Configure(authenticationScheme, configureOptions);
            services.AddScoped<GroupsMapper>();

            services.AddSingleton<IGroupsMapFactory, GraphGroupsMapFactory>();
            services.AddSingleton<GroupsMapObtainer>();
        }
    }
}
