using AOPDemo.ILWeaving.Aspects;

namespace AOPDemo.ILWeaving
{
    //[ValidateParameters]
    [EntryExitLogging]
    public class Calculator : ICalculator
    {
        [CachedPropertyAttribute(CacheKey = "Calculator.Multipler", CacheExpirationTime = 180)]
        public int Multiplier { get { return 5 * 5; } }

        public static Calculator GetInstance()
        {
            Thread.Sleep(5000);
            return new Calculator();
        }

        [LogExceptionsAndReturnNull]
        public static Calculator GetInstance_Kaboom()
        {
            Thread.Sleep(5000);
            throw new ArgumentException();
        }

        [LogExceptionsAndReturnNewInstanceOfReturnType]
        public static Calculator GetInstance_Kaboom2()
        {
            Thread.Sleep(5000);
            throw new ArgumentException();
        }

        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Add([NumericOnlyString] string a, [NumericOnlyString] string b)
        {
            return int.Parse(a) + int.Parse(b);
        }

        [LogExceptionsAndThrow]
        public int Divide(int a, int b)
        {                
            return a / b;
        }

        public int Multiply(int a, int b)
        {
            return a * b;
        }
    }
}
