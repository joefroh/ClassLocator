using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLocator
{
    class Registrar : IClassRegistrar
    {
        public void RegisterClasses(Locator locator)
        {
            locator.Register<ITest,TestClass>();
        }
    }
}
