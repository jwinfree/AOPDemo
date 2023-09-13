using Castle.DynamicProxy;
using System.Diagnostics;

namespace AOPDemo.DynamicProxy.Proxies
{
    [Serializable]
    public class EntryExitLoggingInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Debug.WriteLine($"Calling method: {invocation.MethodInvocationTarget.DeclaringType.Name}.{invocation.MethodInvocationTarget.Name}");

            try
            {
                invocation.Proceed();

                Debug.WriteLine($"{invocation.MethodInvocationTarget.DeclaringType.Name}.{invocation.MethodInvocationTarget.Name} executed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{invocation.MethodInvocationTarget.DeclaringType.Name}.{invocation.MethodInvocationTarget.Name} threw an exception: {ex}");
                throw;
            }
        }
    }
}
