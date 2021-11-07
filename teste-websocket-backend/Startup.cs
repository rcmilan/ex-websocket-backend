using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teste_websocket_backend.Hubs;
using teste_websocket_backend.Models;

namespace teste_websocket_backend
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR(); // middleware do websocket

            services.AddCors(options => {
                options.AddDefaultPolicy(builder => {
                    builder
                    .WithOrigins("http://localhost:3000") // frontend react
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            // singleton para guardar as conexões e usuários/salas
            services.AddSingleton<IDictionary<string, UserRoom>>(opts => new Dictionary<string, UserRoom>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors(); // aplica a policy de CORS

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat"); // registra o hub nos endpoints
            });
        }
    }
}
