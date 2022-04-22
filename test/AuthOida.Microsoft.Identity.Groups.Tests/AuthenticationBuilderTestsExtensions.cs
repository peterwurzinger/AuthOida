using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Xunit;

namespace AuthOida.Microsoft.Identity.Groups.Tests;

public static class AuthenticationBuilderTestsExtensions
{
    internal static GroupsMappingOptions GetGroupMappingOptions(this MicrosoftIdentityBaseAuthenticationBuilder authBuilder, string authenticationType)
    {
        return GetOptionsInstance<GroupsMappingOptions>(authBuilder, authenticationType);
    }

    internal static TOptions GetOptionsInstance<TOptions>(this MicrosoftIdentityBaseAuthenticationBuilder authBuilder, string authenticationType)
        where TOptions : class, new()
    {
        var services = authBuilder.Services.BuildServiceProvider();
        var optionsSnapshot = services.GetRequiredService<IOptionsSnapshot<TOptions>>();
        return optionsSnapshot.Get(authenticationType);
    }

    internal static void AssertRegistered(this IServiceCollection services, Type serviceType, Type? implementationType = null, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        implementationType ??= serviceType;
        var serviceDescriptor = services.SingleOrDefault(s => s.ServiceType == serviceType && s.ImplementationType == implementationType && s.Lifetime == serviceLifetime);

        Assert.NotNull(serviceDescriptor);
    }
}
