using System;
using System.Collections.Generic;

namespace Module.Core
{
    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T service, bool replace = false) where T : class
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            var key = typeof(T);

            if (!replace && _services.ContainsKey(key))
            {
                throw new InvalidOperationException($"Service {key} already registered");
            }
            _services[key] = service;
        }

        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out object service))
            {
                return (T)service;
            }
            throw new InvalidOperationException($"Service {typeof(T)} not registered");
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out object s))
            {
                service = (T)s;
                return true;
            }
            service = null;
            return false;
        }

        public static void Clear() => _services.Clear();
    }
}