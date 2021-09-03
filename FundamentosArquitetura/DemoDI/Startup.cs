using System;
using DemoDI.Cases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoDI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Lifecycle

            services.AddTransient<IOperacaoTransient, Operacao>();
            services.AddScoped<IOperacaoScoped, Operacao>();
            services.AddSingleton<IOperacaoSingleton, Operacao>();
            services.AddSingleton<IOperacaoSingletonInstance>(new Operacao(Guid.Empty));
            services.AddTransient<OperacaoService>();

            #endregion

            #region VidaReal

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IClienteServices, ClienteServices>();

            #endregion

            #region Generics

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            #endregion

            #region MultiplasClasses

            services.AddTransient<ServiceA>();
            services.AddTransient<ServiceB>();
            services.AddTransient<ServiceC>();
            services.AddTransient<Func<string, IService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "A":
                        return serviceProvider.GetService<ServiceA>();
                    case "B":
                        return serviceProvider.GetService<ServiceB>();
                    case "C":
                        return serviceProvider.GetService<ServiceC>();
                    default:
                        return null;
                }
            });

            #endregion

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
                // Middleware that run before routing. Usually the following appear here:
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

            }

            app.UseStaticFiles();

            // Runs matching. An endpoint is selected and set on the HttpContext if a match is found.
            app.UseRouting(); 

            // Middleware that run after routing occurs. Usually the following appear here:
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();
            // These middleware can take different actions based on the endpoint.

            // Executes the endpoint that was selected by routing.
            app.UseEndpoints(endpoints =>
            {
                // Mapping of endpoints goes here:
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                //endpoints.MapHub<MyChatHub>();
                //endpoints.MapGrpcService<MyCalculatorService>();
            });

            // Middleware here will only run if nothing was matched.
        }
    }
}
