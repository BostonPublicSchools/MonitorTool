using System;
using System.Collections.Generic;
using Wolfpack.Core.Containers;
using System.Linq;

namespace Wolfpack.Core
{
    public class Container
    {
        private static IContainer _container;

        public static IContainer Initialise()
        {
            _container = new WindsorContainer();
            return _container;
        }

        public static IContainer RegisterAsTransient(Type implType)
        {
            return _container.RegisterAsTransient(implType);
        }

        public static IContainer RegisterAsTransient<T>(Type implType, string name = null)
            where T : class
        {
            return _container.RegisterAsTransient<T>(implType, name);
        }

        public static IContainer RegisterAsSingleton(Type implType)
        {
            return _container.RegisterAsSingleton(implType);
        }

        public static IContainer RegisterAsSingleton<T>(Type implType, string name = null,
            params Tuple<Type, string>[] hints) 
            where T : class
        {
            return _container.RegisterAsSingleton<T>(implType, name, hints);
        }
        
        public static IContainer RegisterAsSingletonWithInterception<TPlugin, TIntercept>(Type type, 
            string name = null,
            params Tuple<Type, string>[] hints) 
            where TPlugin : class
        {
            return _container.RegisterAsSingletonWithInterception<TPlugin, TIntercept>(type, name, hints);
        }

        public static IContainer RegisterInstance<T>(T instance, string name) 
            where T : class
        {
            return _container.RegisterInstance(instance, name);
        }

        public static IContainer RegisterInstance(Type implType, object instance, string name = null)
        {
            return _container.RegisterInstance(implType, instance, name);
        }

        public static IContainer RegisterInstance<T>(T instance, bool overwrite = false)
            where T : class
        {
            return _container.RegisterInstance(instance);
        }

        public static IContainer RegisterInstance<T>(T instance, Func<bool> shouldRegister)
            where T : class
        {
            return shouldRegister() 
                ? _container.RegisterInstance(instance) 
                : _container;
        }

        public static IContainer RegisterRegisterUsingFactory<T>(Func<T> providerFunc)
            where T : class
        {
            return _container.RegisterUsingFactory(providerFunc);
        }

        public static IContainer RegisterAll<T>()
            where T : class
        {
            return _container.RegisterAll<T>();
        }

        public static IContainer RegisterAllWithInterception<T, TI>()
        {
            return _container.RegisterAllWithInterception<T, TI>();
        }

        public static object Resolve(string componentName)
        {
            return _container.Resolve(componentName);
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public static TI Resolve<TI, TC>()
            where TC : TI
        {
            var targetType = typeof(TC).Name;

            return Find<TI>(implementations => implementations
                .Single(impl => string.Equals(impl.GetType().Name, targetType, StringComparison.OrdinalIgnoreCase)));
        }

        public static T[] ResolveAll<T>()
        {
            return _container.ResolveAll<T>();
        }

        /// <summary>
        /// This will resolve all implementations of the interface
        /// specified but then filters these through the delegate
        /// supplied to pick a specific implementation to use
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static T Find<T>(Func<IEnumerable<T>, T> filter)
        {
            return _container.Find(filter);
        }

        public static void ResolveAll<T>(Action<T> action)
        {
            _container.ResolveAll(action);
        }

        public static bool IsRegistered<T>()
        {
            return _container.IsRegistered<T>();
        }

        public static bool IsRegistered(Type type)
        {
            return _container.IsRegistered(type);
        }
    }
}