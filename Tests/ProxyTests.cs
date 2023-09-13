using AOPDemo.DynamicProxy.Proxies;
using Castle.DynamicProxy;
using FluentAssertions;

namespace AOPDemo.Tests
{
    [TestClass]
    public class ProxyTests
    {
        [TestMethod]
        public void Test_Logging()
        {
            var generator = new ProxyGenerator();
            var calc = generator.CreateInterfaceProxyWithTarget<ICalculator>
                                        (new DynamicProxy.Calculator(), new EntryExitLoggingInterceptor());

            calc.Add(10, 5).Should().Be(15);

            var act = () => calc.Divide(10, 0);
            act.Should().Throw<DivideByZeroException>();           
        }

        [TestMethod]
        public void Test_Profiled()
        {
            var generator = new ProxyGenerator();
            var calc = generator.CreateInterfaceProxyWithTarget<ICalculator>
                                        (new DynamicProxy.Calculator(), new ProfiledInterceptor());

            calc.Add(10, 5).Should().Be(15);
            calc.Divide(10, 5).Should().Be(2);
        }

        [TestMethod]
        public void Test_MethodCaching()
        {
            var generator = new ProxyGenerator();
            var myClassProxy = generator.CreateClassProxy<DynamicProxy.Calculator>(new CacheInterceptor());

            myClassProxy.Add(10, 5).Should().Be(15);
            myClassProxy.Add(10, 5).Should().Be(15);
            myClassProxy.Divide(10, 2);
        }
    }
}