using Castle.DynamicProxy;

namespace DotNetUnknown.Aop;

public static class AopExtensions
{
    extension(IServiceCollection services)
    {
        /**
         * Add Interface-Based Proxy Extension Method.
         */
        public IServiceCollection AddProxiedScoped<TService, TImplementation, TInterceptor>()
            where TService : class
            where TImplementation : class, TService
            where TInterceptor : class, IInterceptor
        {
            services
                .AddScoped<TImplementation>()
                .AddScoped<TInterceptor>()
                .AddScoped<TService>(provider =>
                {
                    var implementation = provider.GetRequiredService<TImplementation>();
                    var proxyGenerator = new ProxyGenerator();
                    var interceptor = provider.GetRequiredService<TInterceptor>();
                    return proxyGenerator.CreateInterfaceProxyWithTarget<TService>(implementation, interceptor);
                });
            return services;
        }
    }
}