namespace AOPDemo.DynamicProxy
{
    public class Calculator : ICalculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Add(string a, string b)
        {
            throw new NotImplementedException();
        }

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
