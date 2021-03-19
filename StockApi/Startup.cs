using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StockApi.Data;
using StockApi.Hubs;
using StockApi.Services;
using StockApp.Client;

namespace StockApi
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
            services.AddDbContext<IContext, StockAppContext>(builder =>
            {
                builder.UseSqlServer(Configuration["connectionString"]);
            });

            services.AddHostedService<HostedService>();
            services.AddSignalR();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StockApi", Version = "v1" });
            });

            services.AddTransient<IGetStockInfoService, GetStockInfoService>()
                .AddListenerClientService(new ClientConfiguration() // listen for all changes to stock prices
                {
                    Host = Configuration["rabbitMq:host"],
                    Port = Configuration.GetValue<int>("rabbitMq:port"),
                    User = Configuration["rabbitMq:user"],
                    Password = Configuration["rabbitMq:password"],
                    ChannelName = Configuration["changeChannelName"],
                    QueueName = $"{Configuration["changeQueueName"]}-{Configuration["POD_NAME"]}"
                })
                .AddTransient<ILargePriceChangeListenerService>(provider => new LargePriceChangeListenerService(
                    new ClientConfiguration
                    {
                        Host = Configuration["rabbitMq:host"],
                        Port = Configuration.GetValue<int>("rabbitMq:port"),
                        User = Configuration["rabbitMq:user"],
                        Password = Configuration["rabbitMq:password"],
                        ChannelName = Configuration["largeChangeChannelName"],
                        QueueName = $"{Configuration["largeChangeQueueName"]}-{Configuration["POD_NAME"]}"
                    }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StockApi v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<PriceChangeHub>("api/pricechange/live");
            });
        }
    }
}
