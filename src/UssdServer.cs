using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using UssdBuilder.Models;

namespace UssdBuilder.Services
{
    /// <summary>
    /// Default implementation of <see cref="IUssdServer"/>.
    /// </summary>
    public class UssdServer<TRequest> : IUssdServer<TRequest> where TRequest : IUssdRequest
    {
        private readonly IDistributedCache _cache;
        private readonly Dictionary<string, Dictionary<string, Func<UssdScreen, TRequest, Task<UssdResponse>>>> _handlers = new();
        private readonly List<UssdRoute> _routes = new();

        private readonly bool _enableInputSplit = true;
        private readonly char[] _inputSplitSeparators = new char[] { '*', '#' };
        private readonly string _backButton = "0";
        private readonly string _homeButton = "00";

        private readonly DistributedCacheEntryOptions _cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(40)
        };

        public UssdServer(IDistributedCache cache, IOptions<UssdServerOption> serverOptions)
        {
            _cache = cache;
            _cacheOptions = serverOptions.Value.CacheEntryOptions ?? _cacheOptions;
            _enableInputSplit = serverOptions.Value.EnableInputSplit ?? true;
            _inputSplitSeparators = serverOptions.Value.InputSplitSeparators ?? _inputSplitSeparators;
            _backButton = serverOptions.Value.BackButton ?? _backButton;
            _homeButton = serverOptions.Value.HomeButton ?? _homeButton;
         }

        public IEnumerable<string> GetCodes()
        {
            return _handlers.Keys.AsEnumerable();
        }

        public void AddRoute(UssdRoute route)
        {
            if(route.Regx == null)
            {
                throw new ArgumentNullException(nameof(route.Regx));
            }
            if (!_routes.Any(r => r.Code == route.Code && r.Prev == route.Prev && r.Goto == route.Goto && r.Regx == route.Regx))
            {
                _routes.Add(route);
            }
        }

        public void AddRoutes(IEnumerable<UssdRoute> routes)
        {
            foreach (var route in routes)
            {
                AddRoute(route);
            }
        }

        public void AddHandlers(string code, Dictionary<string, Func<UssdScreen, TRequest, Task<UssdResponse>>> runners)
        {
            _handlers.TryAdd(code, runners);
        }
    
        private async Task<UssdResponse> ProcessAsync(UssdScreen screen, TRequest request)
        {
            var routes = _routes.Where(m => m.Code == screen.Code && m.Prev == screen.Prev?.Task);
            if(!routes.Any()) return new UssdResponse { Status = false, Message = "END Invalid code or route."};

            var withRegx = routes.FirstOrDefault(r => r.Regx != null && r.Regx.Invoke(screen, request));
            var withoutRegx = routes.FirstOrDefault(r => r.Regx == null);

            screen.Task = withRegx?.Goto ?? withoutRegx?.Goto ?? screen.Prev.Task;

            return await Execute(screen, request);
        }

        private async Task<UssdResponse> Execute(UssdScreen screen, TRequest request)
        {
            return await _handlers[screen.Code][screen.Task].Invoke(screen, request);
        }

        public async Task<string> HandleAsync(TRequest request)
        {
            var result = string.Empty;

            if (_enableInputSplit && _inputSplitSeparators?.Length > 0)
            {
                var chunks = request.Text.Split(_inputSplitSeparators).Where(s => !string.IsNullOrWhiteSpace(s));

                if (chunks.Any())
                {
                    foreach (var item in chunks)
                    {
                        request.Text = item;
                        result = await HandleScreenAsync(request);
                    }
                }
                else
                {
                    result = await HandleScreenAsync(request);
                }
            }
            else
            {
                result = await HandleScreenAsync(request);
            }

            return result;
        }
    
        private async Task<string> HandleScreenAsync(TRequest request)
        {
            UssdScreen current = new()
            {
                Input = request.Text,
                Code = request.ServiceCode
            };

            var cacheString = await _cache.GetStringAsync(request.SessionId);

            if(cacheString is not null && string.IsNullOrWhiteSpace(request.Text))
            {
                await _cache.RemoveAsync(request.SessionId);
                return "END Invalid input";
            }

            var cache = cacheString is not null ? JsonSerializer.Deserialize<UssdScreen>(cacheString) : null;

            current.Prev = cache is not null ? cache : new UssdScreen { Task = null, Code = request.ServiceCode };

            if(request.Text.Equals(_backButton))
            {
                current = current.Prev?.Prev ?? new UssdScreen { Code = request.ServiceCode };
            }
            else if(request.Text.Equals(_homeButton))
            {
                current = new UssdScreen { Code = request.ServiceCode };
            }

            var res = await ProcessAsync(current, request);
            current.Prompt = res.Message;

            if(res.Message.StartsWith("END"))
            {
                await DeleteAsync(request.SessionId);
            }
            else
            {
                await _cache.SetStringAsync(request.SessionId, JsonSerializer.Serialize(current), _cacheOptions, default);
            }
            
            return current.Prompt;
        }

        public async Task DeleteAsync(string sessionId)
        {
            await _cache.RemoveAsync(sessionId);
        }
    }
}