using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobileConsole
{
    public static class TypeCollector
	{
        public static List<Type> GetAllTypes()
        {
            var types = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    var allTypes = assembly.GetTypes();
                    types.AddRange(allTypes);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }

            return types;
        }

        public static Type[] GetClassesOfInterface(Type type)
        {
            var types  = GetAllTypes();

            return types.Where(p => p.GetInterfaces().Any(i => i == type) && !p.IsInterface)
                        .ToArray();
        }

        public static Type[] GetClassesOfClass(Type type)
        {
            var types = GetAllTypes();

            return types.Where(p => p.BaseType == type)
                        .ToArray();
        }

		public static Type[] GetClassesOfBase(Type type)
		{
			var types = GetAllTypes();

			return types.Where(p => type.IsAssignableFrom(p))
						.ToArray();
		}

        public static Type[] GetInterfacesOfInterface(Type type)
        {
            var types = GetAllTypes();

            return types.Where(p => type.IsAssignableFrom(p) && p.IsInterface && p != type)
                        .ToArray();
        }
	}
}