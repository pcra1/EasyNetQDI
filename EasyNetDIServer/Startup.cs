using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using EasyNetQ;
using EasyNetQBus;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EasyNetDIServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IConfiguration>(Configuration);
            services.AddRabbitMqConfiguration(Configuration);
            services.AddSingleton<IBus>(provider =>
                BusBuilder.CreateMessageBus(provider.GetRequiredService<RabbitMqConfiguration>()));

            services.AddCors();
            services.AddHealthChecks()
                .AddRabbitMQ(
                    serviceProvider =>
                    {
                        var rabbitConfig = serviceProvider.GetService<RabbitMqConfiguration>();
                        var factory = new ConnectionFactory()
                        {
                            Uri = new Uri(rabbitConfig.HealthCheckConnectionString),
                            AutomaticRecoveryEnabled = true
                        };

                        return factory.CreateConnection();
                    },
                    name: "RabbitMQ");

            services.AddHostedService<Worker>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var corsOrigins = Configuration.GetSection("CorsOrigins").Get<List<string>>();
            app.UseCors(builder =>
            {
                foreach (var corsOrigin in corsOrigins)
                {
                    builder.WithOrigins(corsOrigin);
                }

                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            app.UseRouting();

            app.UseHealthChecks("/healthcheck", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse //nuget: AspNetCore.HealthChecks.UI.Client
            });
        }
    }
    
}