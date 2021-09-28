using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SystemDot.Ioc.Multiples
{
    public static class TypeExtensions
    {
        public static Assembly GetAssembly(this Type type) => type.GetTypeInfo().Assembly;

        public static IEnumerable<Type> GetTypesInAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly.ExportedTypes;
        }

        public static IEnumerable<Type> WhereImplementsOpenType(this IEnumerable<Type> types, Type openGenericType)
        {
            return types.Where(t => t.ImplementsOpenType(openGenericType));
        }

        static bool ImplementsOpenType(this Type type, Type openGenericType)
        {
            return type.IsAssignableFromOpenGenericType(openGenericType)
                || (type.GetTypeInfo().BaseType != null && type.GetTypeInfo().BaseType.IsAssignableFromOpenGenericType(openGenericType))
                || type.GetInterfaces().Any(i => i.IsAssignableFromOpenGenericType(openGenericType));
        }

        public static IEnumerable<MethodInfo> GetMethodsByName(this Type type, string methodName)
        {
            return type.GetTypeInfo().DeclaredMethods.Where(m => m.Name == methodName);
        }

        public static bool IsInAssemblyOf<T>(this Type type)
        {
            return type.GetAssembly().FullName == typeof(T).GetAssembly().FullName;
        }
    
        public static IEnumerable<Type> ThatImplement<TType>(this IEnumerable<Type> types) => types.WhereNormalConcrete().WhereImplements<TType>();

        public static IEnumerable<Type> WhereImplements<TImplemented>(
          this IEnumerable<Type> types)
        {
            return types.Where(t => t.GetNonBaseInterfaces().Contains(typeof(TImplemented)) || t.GetBaseInterfaces().Contains(typeof(TImplemented)));
        }

        public static IEnumerable<Type> GetNonBaseInterfaces(this Type type)
        {
            IEnumerable<Type> baseInterfaces = type.GetBaseInterfaces();
            return type.GetInterfaces().Where(t => !baseInterfaces.Contains(t));
        }

        private static IEnumerable<Type> GetBaseInterfaces(this Type type)
        {
            var typeList = new List<Type>();
            Type baseType = type.GetTypeInfo().BaseType;
            if (baseType == typeof(MemberInfo))
            {
                return typeList;
            }

            while (baseType != null)
            {
                typeList.AddRange(baseType.GetInterfaces());
                baseType = baseType.GetTypeInfo().BaseType;
                if (baseType == typeof(MemberInfo))
                    return typeList;
            }

            return typeList;
        }

        public static Type[] GetInterfaces(this Type type) => type.GetTypeInfo().ImplementedInterfaces.ToArray<Type>();

        public static IEnumerable<Type> WhereNormalConcrete(this IEnumerable<Type> types) => types.WhereNonAbstract().WhereNonGeneric().WhereConcrete();

        private static IEnumerable<Type> WhereNonAbstract(this IEnumerable<Type> types) => types.Where(t => !t.IsAbstract());

        private static bool IsAbstract(this Type type) => type.GetTypeInfo().IsAbstract;

        private static IEnumerable<Type> WhereConcrete(this IEnumerable<Type> types) => types.Where(IsConcrete);

        public static bool IsNormalConcrete(this Type type) => !type.IsAbstract() && !type.IsGeneric() && type.IsConcrete();

        private static bool IsConcrete(this Type type) => !type.GetTypeInfo().IsInterface && type.GetTypeInfo().IsClass;

        private static IEnumerable<Type> WhereNonGeneric(this IEnumerable<Type> types) => types.Where(t => !t.IsGeneric());

        private static bool IsGeneric(this Type type) => type.GetTypeInfo().ContainsGenericParameters;

        public static MethodInfo GetMethod(this Type type, string methodName, Type[] args) => type.GetRuntimeMethod(methodName, args);

        public static IEnumerable<MethodInfo> GetMethods(this Type type) => type.GetRuntimeMethods();
    }
}