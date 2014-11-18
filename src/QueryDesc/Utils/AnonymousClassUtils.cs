using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.Utils
{
    class AnonymousClassUtils
    {
        private static Dictionary<string, Type> anonymousClasses = new Dictionary<string, Type>();
        private static object lockObj = new object();
        private static ModuleBuilder moduleBuilder;

        static AnonymousClassUtils()
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("AnonymousAssembly"),
                AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule("AnonymousAssembly");
        }

        /// <summary>
        /// Create or get a anonymous class type which is matched the fields. The type of the fields should be basic types
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static Type CreateOrGetAnonymousType(params Tuple<string, Type>[] fields)
        {
            var typeName = GetAnonymousTypeName(fields);

            if(!anonymousClasses.ContainsKey(typeName))
            {
                lock(lockObj)
                {
                    if(!anonymousClasses.ContainsKey(typeName))
                    {
                        var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public);
                        foreach(var item in fields)
                        {
                            typeBuilder.DefineField(item.Item1, item.Item2, FieldAttributes.Public);
                        }
                        var type = typeBuilder.CreateType();
                        anonymousClasses.Add(typeName, type);
                    }
                }
            }
            return anonymousClasses[typeName];
        }

        private static string GetAnonymousTypeName(params Tuple<string, Type>[] fields)
        {
            var parts = from item in fields select item.Item1 + "<" + item.Item2.Name + ">";
            return "anonymous+" + string.Join("_", parts.ToArray());
        }
    }
}
