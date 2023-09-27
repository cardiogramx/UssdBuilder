using Microsoft.Extensions.DependencyInjection;
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
            return services.AddSingleton<IUssdServer, UssdServer>();
        }
    }
}