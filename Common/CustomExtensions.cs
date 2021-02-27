using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common
{
    public static class CustomExtensions
    {
        public static IServiceCollection AddRabbitMqConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            var rabbitSettings = configuration.GetSection("rabbitMqConfig");
            services.Configure<RabbitMqConfiguration>(c => rabbitSettings.Bind(c));
            services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value);

            return services;
        }
    }
}