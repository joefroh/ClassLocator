using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLocator
{
    class Registrar : IClassRegistrar
    {
        public void RegisterClasses(ClassLocator locator)
        {
            locator.Register<TestClass,TestClass>();
        }
    }
}
