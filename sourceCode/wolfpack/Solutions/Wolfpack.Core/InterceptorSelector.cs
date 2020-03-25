using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;

namespace Wolfpack.Core
{
    public class InterceptorSelector<T> : IInterceptorSelector
    {
        protected string _methodToIntercept;

        public InterceptorSelector(string methodToIntercept)
        {
            _methodToIntercept = methodToIntercept;
        }

        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var active = new List<IInterceptor>(interceptors);

            if (string.CompareOrdinal(method.Name, _methodToIntercept) != 0)
            {
                // play nicely with other filters, remove our publisher filters
                // if the property/method being intercepted is not "Consume"
                active.RemoveAll(i => i is T);
                return active.ToArray();
            }

            // ok, something has called the Consume method
            return active.ToArray();
        }
    }
}