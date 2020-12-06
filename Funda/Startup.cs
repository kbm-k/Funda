using Funda.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Funda.Startup))]
namespace Funda
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IApiService, ApiService>();
            builder.Services.AddHttpClient<ApiService>();
        }
    }
}