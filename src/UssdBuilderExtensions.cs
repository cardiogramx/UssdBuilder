using Microsoft.Extensions.DependencyInjection;
using UssdBuilder.Models;
using UssdBuilder.Services;

namespace UssdBuilder.Extensions.AspNetCore
{
    public static class UssdBuilderExtensions
    {
        /// <summary>
        /// Registers a singleton instance of <see cref="UssdServer"/> as an implementation of <seealso cref="IUssdServer"/>. 
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

            return services.AddSingleton<IUssdServer, UssdServer>();
        }

        /// <summary>
        /// Registers a singleton instance of <see cref="UssdServer"/> as an implementation of <seealso cref="IUssdServer"/>. 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction">Ussd server configuration</param>
        /// <returns></returns>
        public static IServiceCollection AddUssdServer(this IServiceCollection services, Action<UssdServerOption> setupAction)
        {
            return services.Configure(setupAction).AddSingleton<IUssdServer, UssdServer>();
        }
    }
}