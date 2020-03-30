using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

namespace Wolfpack.Core.Containers
{
    public class WindsorContainer : IContainer
    {
        protected Castle.Windsor.WindsorContainer Instance;

        public WindsorContainer()
        {
            Instance = new Castle.Windsor.WindsorContainer(new ZeroAppConfigXmlInterpreter());
            Instance.Kernel.Resolver.AddSubResolver(new CollectionResolver(Instance.Kernel, true));
        }

        public IContainer RegisterAsTransient(Type implType)
        {
            Instance.Register(Component.For(implType)
                .ImplementedBy(implType)
                .LifeStyle.Transient);
            return this;
        }

        public IContainer RegisterAsTransient<T>(Type implType, string name = null)
            where T: class
        {
            var component = Component.For<T>()
                .ImplementedBy(implType)
                .LifeStyle.Transient;

            if (!string.IsNullOrWhiteSpace(name))
                component.Named(name);

            Instance.Register(component);
            return this;
        }

        /// <summary>
        /// Registers an interface with a singleton providerFunc that is used to create new instances of a given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="providerFunc">The provider function.</param>
        /// <returns></returns>
        public IContainer RegisterUsingFactory<T>(Func<T> providerFunc)
            where T : class
        {
            var component = Component.For<T>()
                .UsingFactoryMethod(_ => providerFunc());

            Instance.Register(component);
            return this;
        }

        public IContainer RegisterAsSingleton(Type implType)
        {
            Instance.Register(Component.For(implType)
                .ImplementedBy(implType)
                .LifeStyle.Singleton);
            return this;
        }

        public IContainer RegisterAsSingleton<T>(Type implType,
            string name = null,
            params Tuple<Type, string>[] hints) 
            where T: class
        {
            var component = Component.For<T>()
                .ImplementedBy(implType)
                .LifeStyle.Singleton;

            if (!string.IsNullOrWhiteSpace(name))
                component.Named(name);

            ApplyHints(component, hints);
            Instance.Register(component);
            return this;
        }

        public IContainer RegisterAsSingletonWithInterception<TPlugin, TIntercept>(Type type, 
            string name = null, 
            params Tuple<Type, string>[] hints)
            where TPlugin : class
        {
            var interceptorTypes = (from iType in ResolveAll<TIntercept>()
                                    select iType.GetType()).ToArray();

            var component = Component.For(typeof (TPlugin))
                .LifeStyle.Transient
                .ImplementedBy(type)
                .Interceptors(interceptorTypes);

            if (!string.IsNullOrWhiteSpace(name))
                component.Named(name);

            ApplyHints(component, hints);
            Instance.Register(component);
            return this;
        }

        public IContainer RegisterInstance<T>(T instance, bool overwrite = false)
            where T : class
        {
            Instance.Register(overwrite
                                   ? Component.For<T>().Instance(instance).OverWrite()
                                   : Component.For<T>().Instance(instance));
            return this;
        }

        public IContainer RegisterInstance<T>(T instance, string name)
            where T : class
        {
            Instance.Register(Component.For<T>().Instance(instance).Named(name));
            return this;
        }

        public IContainer RegisterInstance(Type implType, object instance, string name = null)
        {
            var registration = Component.For(implType).Instance(instance);            

            if (!string.IsNullOrWhiteSpace(name))
                registration.Named(name);

            Instance.Register(registration);
            return this;
        }

        public IContainer RegisterAll<T>()
            where T : class
        {
            Type[] components;

            if (TypeDiscovery.Discover<T>(out components))
                components.ToList().ForEach(c => RegisterAsTransient<T>(c));            
            return this;
        }

        public IContainer RegisterAllWithInterception<T, TI>()
        {
            Type[] components;

            if (!TypeDiscovery.Discover<T>(out components))
                return this;

            var interceptorTypes = (from iType in ResolveAll<TI>()
                                    select iType.GetType()).ToArray();

            components.ToList().ForEach(c =>
                               Instance.Register(Component.For(typeof (T))
                                                       .LifeStyle.Transient
                                                       .ImplementedBy(c)
                                                       .Interceptors(interceptorTypes)));
            return this;
        }

        public object Resolve(string componentName)
        {
            return Instance.Resolve<object>(componentName, new Dictionary<string, string>());
        }

        public T Resolve<T>()
        {
            return Instance.Resolve<T>();
        }

        public T[] ResolveAll<T>()
        {
            return Instance.ResolveAll<T>();
        }

        public T Find<T>(Func<IEnumerable<T>, T> filter)
        {
            var components = ResolveAll<T>();
            return filter(components);
        }

        public void ResolveAll<T>(Action<T> action)
        {
            var components = ResolveAll<T>();
            components.ToList().ForEach(action);
        }

        public bool IsRegistered<T>()
        {
            return IsRegistered(typeof (T));
        }

        public bool IsRegistered(Type type)
        {
            return Instance.Kernel.HasComponent(type);
        }

        private void ApplyHints<T>(ComponentRegistration<T> component, IEnumerable<Tuple<Type, string>> hints)
            where T: class
        {
            if (hints == null)
                return;

            foreach (var hint in hints)
                component.DependsOn(Dependency.OnComponent(hint.Item1, hint.Item2));           
        }
    }
}