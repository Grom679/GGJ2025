
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRSim.Utils
{
    public class ServiceLocator<TContext>
    {
        public static Type Context = typeof(TContext);

        private static Dictionary<Type, object> _services;

        static ServiceLocator()
        {
            _services = new Dictionary<Type, object>();
        }

        public static void Dispose()
        {
            _services.Clear();
        }

        public static void Register<T>(T service) where T : class
        {
            Type type = typeof(T);

            if (_services.ContainsKey(type))
            {
                Debug.LogError($"Service of type \"{type.Name}\" is already registered");
                return;
            }

            _services.Add(type, service);
        }

        public static void Unregister<T>(T service) where T : class
        {
            Type type = typeof(T);

            if (!_services.ContainsKey(type))
            {
                Debug.LogWarning($"Service of type \"{type.Name}\" is not registered");
                return;
            }

            _services.Remove(type);
        }

        public static T Get<T>() where T : class
        {
            Type type = typeof(T);

            if (!_services.TryGetValue(type, out object foundService))
            {
                Debug.LogError($"Unable to find service of type \"{type.Name}\"");
                return default(T);
            }

            return foundService as T;
        }

        public static void Clear()
        {
            _services.Clear();
        }
    }
}