using System;
using System.Collections.Generic;

namespace Wolfpack.Core.Containers
{
    public interface IContainer
    {
        IContainer RegisterAsTransient(Type implType);
        IContainer RegisterAsTransient<T>(Type implType, string name = null) where T : class;
        IContainer RegisterAsSingleton(Type implType);
        IContainer RegisterAsSingleton<T>(Type implType,
            string name = null, 
            params Tuple<Type, string>[] hints) 
            where T : class;
        IContainer RegisterAsSingletonWithInterception<TPlugin, TIntercept>(Type type, 
            string name = null, 
            params Tuple<Type, string>[] hints) 
            where TPlugin : class;
        IContainer RegisterInstance<T>(T instance, bool overwrite = false) where T : class;
        IContainer RegisterInstance<T>(T instance, string name) where T : class;
        IContainer RegisterInstance(Type implType, object instance, string name = null);
        IContainer RegisterAll<T>() where T : class;
        IContainer RegisterAllWithInterception<T, TI>();
        IContainer RegisterUsingFactory<T>(Func<T> providerFunc) where T : class;
        object Resolve(string componentName);
        T Resolve<T>();        
        T[] ResolveAll<T>();
        T Find<T>(Func<IEnumerable<T>, T> filter);
        void ResolveAll<T>(Action<T> action);
        bool IsRegistered<T>();
        bool IsRegistered(Type type);
    }
}