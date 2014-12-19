using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLocator
{
    class Program
    {
        static void Main(string[] args)
        {
            var locator = ClassLocator.Locator;

            locator.RegisterInstance<TestClass>(new TestClass());

            locator.Fetch<TestClass>().HelloWorld();

            Console.ReadKey();
        }
    }
}
