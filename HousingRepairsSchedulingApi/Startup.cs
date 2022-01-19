using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingRepairsOnline.Authentication.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace HousingRepairsSchedulingApi
{
    using System.ServiceModel;
    using Gateways;
    using Microsoft.Extensions.Options;
    using Services.Drs;
    using UseCases;

    public class Startup
    {
        private const string HousingRepairsSchedulingApiIssuerId = "Housing Management System Api";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHousingRepairsOnlineAuthentication(HousingRepairsSchedulingApiIssuerId);

            services.AddControllers();
            services.AddTransient<IRetrieveAvailableAppointmentsUseCase, RetrieveAvailableAppointmentsUseCase>();

            this.ConfigureOptions(services);

            services.AddScoped<SOAP>(sp =>
            {
                var drsOptions = sp.GetRequiredService<IOptions<DrsOptions>>();
                return new SOAPClient(new BasicHttpsBinding(), new EndpointAddress(drsOptions.Value.ApiAddress));
            });

            services.AddTransient<IDrsService, DrsService>();

            services.AddTransient<IAppointmentsGateway, DrsAppointmentGateway>(sp =>
                {
                    var drsOptions = sp.GetRequiredService<IOptions<DrsOptions>>();
                    var appointmentSearchTimeSpanInDays = drsOptions.Value.SearchTimeSpanInDays;
                    return new DrsAppointmentGateway(sp.GetService<IDrsService>(),
                        5, appointmentSearchTimeSpanInDays, 2);
                }
            );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HousingRepairsSchedulingApi", Version = "v1" });
                c.AddJwtSecurityScheme();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HousingRepairsSchedulingApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });
        }

        private void ConfigureOptions(IServiceCollection services)
        {
            var drsOptionsConfiguration = this.Configuration.GetSection(nameof(DrsOptions));

            if (string.IsNullOrEmpty(drsOptionsConfiguration["ApiAddress"]))
            {
                throw new InvalidOperationException($"Incorrect configuration: {nameof(DrsOptions)}.{nameof(DrsOptions.ApiAddress)} is a required configuration.");
            }
            if (string.IsNullOrEmpty(drsOptionsConfiguration["Login"]))
            {
                throw new InvalidOperationException($"Incorrect configuration: {nameof(DrsOptions)}.{nameof(DrsOptions.Login)} is a required configuration.");
            }
            if (string.IsNullOrEmpty(drsOptionsConfiguration["Password"]))
            {
                throw new InvalidOperationException($"Incorrect configuration: {nameof(DrsOptions)}.{nameof(DrsOptions.Password)} is a required configuration.");
            }

            services.Configure<DrsOptions>(drsOptionsConfiguration);
        }
    }
}
