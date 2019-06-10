using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace TrackEverything.View.Ninject
{
    public sealed class RequestScopingStartupFilter : IStartupFilter
    {
        private readonly Func<IDisposable> requestScopeProvider;

        public RequestScopingStartupFilter(Func<IDisposable> requestScopeProvider)
        {
            this.requestScopeProvider =
                requestScopeProvider ?? throw new ArgumentNullException(nameof(requestScopeProvider));
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> nextFilter)
        {
            return builder =>
            {
                ConfigureRequestScoping(builder);
                nextFilter(builder);
            };
        }

        private void ConfigureRequestScoping(IApplicationBuilder builder)
        {
            builder.Use(async (context, next) =>
            {
                using (var scope = requestScopeProvider())
                {
                    await next();
                }
            });
        }
    }
}

