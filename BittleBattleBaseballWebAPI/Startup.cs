using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BittleBattleBaseballWebAPI
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
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddHealthChecks();

            //services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(policy =>
            //    {
            //        policy.WithOrigins("http://localhost:4200", "https://bittlebattlebaseball.onrender.com", "http://bittlebattlebaseball.onrender.com").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            //    });
            //});

            services.AddResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

           // app.UseHttpsRedirection();

            app.UseRouting();         
             
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapHealthChecks("/health"));

            app.UseCors(x => x
                   .AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
