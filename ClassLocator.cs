using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ClassLocator
{
    //TODO: Sweep for interfaces/type registration and just log them into a temp list initially, as resources are requested, lazy load the instance. 
    //This should mostly avoid the embedded location call problem.

    public class Locator
    {
        //Privates (lol)
        private static Locator _locator;
        private static bool _loading;
        
        //Active instances of classes and stuff.
        private static Dictionary<Type, Object> _instances;
        private static Dictionary<Type, Type> _classes;
        private static List<Assembly> _scannedAssemblies; 

        
        private Locator()
        {
            _loading = true;
            _instances = new Dictionary<Type, object>();
            _classes = new Dictionary<Type, Type>();
            _scannedAssemblies = new List<Assembly>();
        }

        public void RegisterInstance<T>(T obj)
        {
            if(_instances.ContainsKey(typeof(T))){

                //This is replacing the registered instance... may want to do some sort of disposal call here for safety.
                _instances[typeof(T)] = obj;
            }
            else
            {
                _instances.Add(typeof(T),obj);
            }
        }

        public void Register<U, T>() where T : U
        {
            if (_classes.ContainsKey(typeof(U)))
            {
                throw new ArgumentException("The provided type has already been registered.");
            }

            _classes.Add(typeof(U), typeof(T));
        }

        public T Fetch<T>(){
            object temp;
           
            //Hae we already loaded an instance?
            _instances.TryGetValue(typeof(T), out temp);

            if (temp != null)
            {
                //This is a safe assumption, as we do the sanitization on input.
                return (T)temp;
            }

            //Maybe we have registered and not loaded it yet.
            temp = InitRegisteredClass<T>();
            if (temp != null)
            {
                return (T)temp;
            }
           
            //Do a sweep to see if the DLL has been loaded and we can register the class.
            Sweep();
            temp = InitRegisteredClass<T>();
            if (temp != null)
            {
                return (T)temp;
            }

            //If we get here, something has gone wrong. The asked for class does not exist in the locator.
            throw new ArgumentException("The requested type has not been registered with the Locator or cannot be found.");
        }

        private T InitRegisteredClass<T>()
        {
            Type type;
            object temp = null;
            _classes.TryGetValue(typeof (T), out type);

            if (type != null)
            {
                temp = Activator.CreateInstance(type);
                _instances.Add(typeof (T), temp);
                
            }

            return (T)temp;
        }

        /// <summary>
        /// Sweeps currently active binaries for a registration class.
        /// </summary>
        private void Sweep()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            foreach (var assembly in assemblies)
            {
                if (_scannedAssemblies.Contains(assembly))
                {
                    continue;
                }

                foreach (var type in assembly.DefinedTypes)
                {
                    if (typeof(IClassRegistrar).IsAssignableFrom(type) && typeof(IClassRegistrar) != type)
                    {
                        var reg = Activator.CreateInstance(type) as IClassRegistrar;
                        reg.RegisterClasses(_locator);
                    }
                }
                _scannedAssemblies.Add(assembly);
            }

        }

        public static Locator Instance
        {
            get
            {
                if (_locator == null)
                {
                    _locator = new Locator();
                    _locator.Sweep();
                    return _locator;
                }

                return _locator;
            }
        }
    }
}
