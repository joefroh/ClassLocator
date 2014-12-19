using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLocator
{
    //TODO: Sweep for interfaces/type registration and just log them into a temp list initially, as resources are requested, lazy load the instance. 
    //This should mostly avoid the embedded location call problem.

    public class ClassLocator
    {
        //Privates (lol)
        private static ClassLocator _locator;
        private static bool _loading;
        
        //Active instances of classes and stuff.
        private static Dictionary<Type, Object> _instances;
        private static Dictionary<Type, Type> _classes;

        
        private ClassLocator()
        {
            _loading = true;
            _instances = new Dictionary<Type, object>();
            _classes = new Dictionary<Type, Type>();
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
            Type type;

            //Hae we already loaded an instance?
            _instances.TryGetValue(typeof(T), out temp);

            if (temp != null)
            {
                //This is a safe assumption, as we do the sanitization on input.
                return (T)temp;
            }

            //Maybe we have registered and not loaded it yet.
            _classes.TryGetValue(typeof(T), out type);

            if (type != null)
            {
                temp = Activator.CreateInstance(type);
                _instances.Add(typeof(T), temp);
                return (T)temp;
            }

            //TODO: This should trigger a new sweep first
            throw new ArgumentException("The requested type has not been registered with the Locator or cannot be found.");
        }

        /// <summary>
        /// Sweeps currently active binaries for a registration class.
        /// </summary>
        private void Sweep()
        {
            //TODO
        }

        public static ClassLocator Locator
        {
            get
            {
                if (_locator == null)
                {
                    _locator = new ClassLocator();
                    _locator.Sweep();
                    return _locator;
                }

                return _locator;
            }
        }
    }
}
