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
            var locator = Locator.Instance;

            locator.Fetch<ITest>().HelloWorld();
            //locator.Fetch<IService>();
            Console.ReadKey();
        }
    }
}
