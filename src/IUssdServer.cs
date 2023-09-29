using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UssdBuilder.Models;

namespace UssdBuilder.Services
{
    /// <summary>
    /// Ussd server interface.
    /// </summary>
    public interface IUssdServer<TRequest> where TRequest : IUssdRequest
    {
        /// <summary>
        /// Adds a route map for a ussd server.
        /// </summary>
        /// <param name="route"></param>
        void AddRoute(UssdRoute route);

        /// <summary>
        /// Add route maps for a ussd server.
        /// </summary>
        /// <param name="routes"></param>
        void AddRoutes(IEnumerable<UssdRoute> routes);

        /// <summary>
        /// Registers route handlers for a ussd server.
        /// </summary>
        /// <param name="code">Ussd code</param>
        /// <param name="handlers">Runnable handler functions</param>
        void AddHandlers(string code, Dictionary<string, Func<UssdScreen, TRequest, Task<UssdResponse>>> handlers);

        /// <summary>
        /// Handles a ussd operation and returns the string response.
        /// </summary>
        /// <param name="request">Ussd request</param>
        /// <returns></returns>
        Task<string> HandleAsync(TRequest request);

        /// <summary>
        /// Returns all ussd codes in use by the server.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetCodes();

        /// <summary>
        /// Deletes a ussd session.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task DeleteAsync(string sessionId);
    }
}