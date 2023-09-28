using Microsoft.Extensions.DependencyInjection;
using UssdBuilder.Models;
using UssdBuilder.Services;

namespace UssdBuilder.Extensions.AspNetCore
{
    public static class UssdBuilderExtensions
    {
        /// <summary>
        /// Registers a singleton instance of <see cref="UssdServer"/> using <seealso cref="UssdRequest"/>. 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUssdServer(this IServiceCollection services)
        {
            services.Configure<UssdServerOption>(opt =>
            {
                opt.EnableInputSplit = true;
                opt.InputSplitSeparators = new char[] { '*', '#' };
            });

            return services.AddSingleton<IUssdServer<UssdRequest>, UssdServer<UssdRequest>>();
        }

        /// <summary>
        /// Registers a singleton instance of <see cref="UssdServer"/> using <seealso cref="UssdRequest"/>. 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction">Ussd server configuration</param>
        /// <returns></returns>
        public static IServiceCollection AddUssdServer(this IServiceCollection services, Action<UssdServerOption> setupAction)
        {
            return services.Configure(setupAction).AddSingleton<IUssdServer<UssdRequest>, UssdServer<UssdRequest>>();
        }



        /// <summary>
        /// Registers a singleton instance of <see cref="UssdServer"/> for a custom implementation of <seealso cref="IUssdRequest"/>. 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUssdServerFor<TRequest>(this IServiceCollection services) where TRequest : IUssdRequest
        {
            services.Configure<UssdServerOption>(opt =>
            {
                opt.EnableInputSplit = true;
                opt.InputSplitSeparators = new char[] { '*', '#' };
            });

            return services.AddSingleton<IUssdServer<TRequest>, UssdServer<TRequest>>();
        }

        /// <summary>
        /// Registers a singleton instance of <see cref="UssdServer"/> for a custom implementation of <seealso cref="IUssdRequest"/>. 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction">Ussd server configuration</param>
        /// <returns></returns>
        public static IServiceCollection AddUssdServerFor<TRequest>(this IServiceCollection services, Action<UssdServerOption> setupAction) where TRequest : IUssdRequest
        {
            return services.Configure(setupAction).AddSingleton<IUssdServer<TRequest>, UssdServer<TRequest>>();
        }
    }
}