using AspNetSecurity_NoSecurity.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetSecurity_NoSecurity
{
    public class Startup
    {
        private readonly IHostingEnvironment env;

        public Startup(IHostingEnvironment environment)
        {
            this.env = environment;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            //data protection api
            services.AddDataProtection();
            // makes all calls https in produciton enviroment.
            if (!env.IsDevelopment())
            {
                services.Configure<MvcOptions>(o => o.Filters.Add(new RequireHttpsAttribute()));
            }

            services.AddCors(options =>
            {

                options.AddPolicy("AllowBankCom", c => c.WithOrigins("https://bank.com"));
            });

            services.AddSingleton<ConferenceRepo>();
            services.AddSingleton<ProposalRepo>();
            services.AddSingleton<AttendeeRepo>();
            services.AddSingleton<PurposeStringConstants>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            //creates a header for secure communication for 365 days. if website or browser does not support HTTPS this website can then not be contacted.
            //you can register website for preloads 
            app.UseHsts(c => c.MaxAge(days: 365));
            // Content security policy(CSP)
            app.UseCsp(options => options.DefaultSources(s => s.Self()).StyleSources(c => c.Self().CustomSources("maxcdn.bootstrapcdn.com")).ReportUris(r => r.Uris("/report")));
            // this is for website cross scription 
            app.UseXfo(o => o.Deny());
            app.UseStaticFiles();
            //this is for api cross site scripting.
            //app.UseCors(c => c.AllowAnyOrigin());
            // this is using a CORS app secuirty created in configure.
            app.UseCors("AllowBankCom");
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Conference}/{action=Index}/{id?}");
            });
        }
    }
}
