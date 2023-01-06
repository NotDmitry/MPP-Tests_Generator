namespace Test_Generator.Tests.Classes
{
    public class MyClassesA
    {
        public void FirstMethod()
        {
            Console.WriteLine("First method");
        }

        public void SecondMethod()
        {
            Console.WriteLine("Second method");
        }

        public void ThirdMethod(int a)
        {
            Console.WriteLine("Third method (int)");
        }

        public void ThirdMethod(double a)
        {
            Console.WriteLine("Third method (double)");
        }
    }

    public class MyClassesB
    {
        public void FirstMethod()
        {
            Console.WriteLine("First");
        }
    }
}
