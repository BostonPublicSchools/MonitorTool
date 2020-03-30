using Castle.DynamicProxy;

namespace Wolfpack.Core.Containers
{
    public abstract class InterceptorBase : IInterceptor
    {
        protected string _methodToIntercept;

        protected InterceptorBase(string methodToIntercept)
        {
            _methodToIntercept = methodToIntercept;
        }

        protected virtual bool InterceptThisMethod(IInvocation invocation)
        {
            return (string.CompareOrdinal(invocation.Method.Name, _methodToIntercept) == 0);
        }

        public void Intercept(IInvocation invocation)
        {
            if (InterceptThisMethod(invocation))
            {
                HandleIntercept(invocation);
            }
            else
            {
                invocation.Proceed();
            }
        }

        protected abstract void HandleIntercept(IInvocation invocation);
    }
}