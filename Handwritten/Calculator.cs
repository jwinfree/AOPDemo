using System.Diagnostics;

namespace AOPDemo.Handwritten
{
    public class Calculator : ICalculator
    {
        public int Add(int a, int b)
        {            
            var stp = Stopwatch.StartNew();
            try
            {
                return a + b;
            }
            finally
            {
                Debug.WriteLine($"{DateTime.Now:HH:mm:ss:fff} Calculator.Add - Elapsed: {stp.Elapsed}");
            }            
        }

        public int Add(string a, string b)
        {
            throw new NotImplementedException();
        }

        public int Divide(int a, int b)
        {
            var stp = Stopwatch.StartNew();
            try
            {                    
                return a / b;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                Debug.WriteLine($"{DateTime.Now:HH:mm:ss:fff} Calculator.Divide - Elapsed: {stp.Elapsed}");
            }            
        }

        public int Multiply(int a, int b)
        {
            throw new NotImplementedException();
        }
    }
}
