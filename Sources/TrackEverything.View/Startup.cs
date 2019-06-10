using System;
using System.Threading;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ninject;
using Ninject.Activation;
using Ninject.Infrastructure.Disposal;
using TrackEverything.BusinessLogic.AutomapperProfiles;
using TrackEverything.BusinessLogic.Infrastructure;
using TrackEverything.EFStorage.Context;
using TrackEverything.Tools.Logger;
using TrackEverything.View.Ninject;

namespace TrackEverything.View
{
    public class Startup
    {
        private readonly AsyncLocal<Scope> scopeProvider = new AsyncLocal<Scope>();


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private IKernel Kernel { get; set; }

        private object Resolve(Type type)
        {
            return Kernel.Get(type);
        }

        private object RequestScope(IContext context)
        {
            return scopeProvider.Value;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            services.AddAutoMapper(
                typeof(ProjectProfile),
                typeof(TaskProfile),
                typeof(WorkerProfile)
            );

            services.AddLogging(
                builder =>
                {
                    builder.AddFilter("Microsoft", LogLevel.Critical)
                        .AddFilter("System", LogLevel.Critical)
                        .AddFilter("NToastNotify", LogLevel.Critical)
                        .AddConsole();
                });

            services.AddDbContext<DatabaseContext>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddRequestScopingMiddleware(() => scopeProvider.Value = new Scope());
            services.AddCustomControllerActivation(Resolve);
            services.AddCustomViewComponentActivation(Resolve);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Kernel = RegisterApplicationComponents(app);

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            loggerFactory.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
            {
                LogLevel = LogLevel.Information
            }));

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }

        private IKernel RegisterApplicationComponents(IApplicationBuilder app)
        {
            var ViewServices = new ViewServiceModule();
            var BusinessServices = new BLServiceModule();

            var kernel = new StandardKernel(ViewServices, BusinessServices);

            // Register application services
            foreach (var ctrlType in app.GetControllerTypes()) kernel.Bind(ctrlType).ToSelf().InScope(RequestScope);

            kernel.BindToMethod(app.GetRequestService<IViewBufferScope>);

            return kernel;
        }

        private sealed class Scope : DisposableObject
        {
        }
    }
}