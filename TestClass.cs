using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLocator
{
    public class TestClass : ITest
    {
        public TestClass()
        {

        }

        public void HelloWorld()
        {
            Console.WriteLine("Hello World");
        }

    }
}
