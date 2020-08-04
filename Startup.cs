using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CookieAndSession
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.ConsentCookie.Name = "ConsentCookie";
            });
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = "Session";
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCookiePolicy();
            app.UseMiddleware<ConsentMiddleware>();
            app.UseSession();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("cookie", async context =>
                {
                    //var counter1 = int.Parse(context.Request.Cookies["counter1"] ?? "0") + 1;
                    //var counter2 = int.Parse(context.Request.Cookies["counter2"] ?? "0") + 1;

                    var counter1 = (context.Session.GetInt32("counter1") ?? 0) + 1;
                    var counter2 = (context.Session.GetInt32("counter2") ?? 0) + 1;


                    //context.Response.Cookies.Append("counter1", counter1.ToString(),new CookieOptions
                    //{
                    //    MaxAge = TimeSpan.FromMinutes(30),
                    //    IsEssential = true
                    //});

                    //context.Response.Cookies.Append("counter2", counter2.ToString(),new CookieOptions
                    //{
                    //    MaxAge = TimeSpan.FromMinutes(30),
                    //});

                    context.Session.SetInt32("counter1", counter1);
                    context.Session.SetInt32("counter2", counter2);

                    await context.Session.CommitAsync();

                    await context.Response.WriteAsync($"counter1:{counter1}\ncounter2:{counter2}");
                });
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("clear", context =>
            //    {
            //        context.Response.Cookies.Delete("counter1");
            //        context.Response.Cookies.Delete("counter2");
            //        context.Response.Redirect("/");
            //        return Task.CompletedTask;
            //    });
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallback( async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
