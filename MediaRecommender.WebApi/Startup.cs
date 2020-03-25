using Autofac;
using MediaRecommender.WebApi.App_Start;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MediaRecommender.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        ///     ConfigureServices is where you register dependencies. This gets
        ///     called by the runtime before the ConfigureContainer method, below.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services
                .RegisterApplicationLayer(Configuration)
                .RegisterDomainLayer(Configuration)
                .RegisterDataLayer(Configuration);
        }

        /// <summary>
        ///     ConfigureContainer is where you can register things directly with Autofac.
        ///     This runs after ConfigureServices so the things here will override
        ///     registrations made in ConfigureServices.
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder
                .RegisterApplicationLayer(Configuration)
                .RegisterDomainLayer(Configuration)
                .RegisterDataLayer(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
