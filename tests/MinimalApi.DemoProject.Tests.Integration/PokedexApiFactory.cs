using Microsoft.Extensions.DependencyInjection.Extensions;
using MinimalApi.DemoProject.Data;

namespace MinimalApi.DemoProject.Tests.Integration;

public class PokedexApiFactory : WebApplicationFactory<IApiMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IDbConnectionFactory));
            services.AddSingleton<IDbConnectionFactory>(_ =>
                new SqliteConnectionFactory(
                    "DataSource=file:inmem?mode=memory&cache=shared"));
        });
    }
}
