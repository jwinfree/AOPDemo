using FluentAssertions;

namespace AOPDemo.Tests
{
    [TestClass]
    public class WeavingTests
    {
        [TestMethod]
        public void Test_Logging()
        {
            var calc = new ILWeaving.Calculator();

            calc.Add(10, 5).Should().Be(15);

            var act = () => calc.Divide(10, 0); 
            act.Should().Throw<DivideByZeroException>();
        }

        [TestMethod]
        public void Test_Profiled()
        {
            var calc = new ILWeaving.Calculator();

            calc.Add(10, 5).Should().Be(15);
            calc.Divide(10, 5).Should().Be(2);
        }

        [TestMethod]
        public void Test_MethodCaching()
        {
            var calc = new ILWeaving.Calculator();

            calc.Add(10, 5).Should().Be(15);
            calc.Add(10, 5).Should().Be(15);
        }

        [TestMethod]
        public void Test_PropertyCaching()
        {
            var calc = new ILWeaving.Calculator();

            int result = calc.Multiplier;

            result = calc.Multiplier;
        }

        [TestMethod]
        public void Test_LogExceptionsAndThrow()
        {
            var calc = new ILWeaving.Calculator();

            var act = () => calc.Divide(10, 0);
            act.Should().Throw<DivideByZeroException>();
        }

        [TestMethod]
        public void Test_LogExceptionsAndReturnNull()
        {
            var instance = ILWeaving.Calculator.GetInstance_Kaboom();

            instance.Should().BeNull();
        }

        [TestMethod]
        public void Test_LogExceptionsAndReturnNewInstance()
        {
            var instance = ILWeaving.Calculator.GetInstance_Kaboom2();

            instance.Should().NotBeNull();            
        }

        [TestMethod]
        public void Test_Validation()
        {
            var calc = new ILWeaving.Calculator();

            calc.Add("5", "5").Should().Be(10);

            var act = () => calc.Add("Sandy", "Cheeks");
            act.Should().Throw<ArgumentException>();
        }
    }
}