using Castle.DynamicProxy;
using System.Diagnostics;

namespace AOPDemo.DynamicProxy.Proxies
{
    [Serializable]
    public class ProfiledInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var stp = Stopwatch.StartNew();

            try
            {
                invocation.Proceed();
            } 
            finally
            {
                Debug.WriteLine($"{DateTime.Now:HH:mm:ss:fff} {invocation.MethodInvocationTarget.DeclaringType.Name}.{invocation.MethodInvocationTarget.Name} - Elapsed: {stp.Elapsed}");
            }
        }
    }
}
