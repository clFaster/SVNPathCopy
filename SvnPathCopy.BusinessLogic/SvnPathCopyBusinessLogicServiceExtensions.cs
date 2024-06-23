using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpSvn;

namespace SvnPathCopy.BusinessLogic;

public static class SvnPathCopyBusinessLogicServiceExtensions
{
    public static IServiceCollection AddSvnPathCopyBusinessLogic(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<SvnClient>();
        return services;
    }
}