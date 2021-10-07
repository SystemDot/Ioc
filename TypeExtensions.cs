using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SystemDot.Ioc
{
    public static class TypeExtensions
    {
        public static IEnumerable<ConstructorInfo> GetAllConstructors(this Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors;
        }

        public static Type MakeGenericTypeFrom(this Type type, Type toMakeFrom)
        {
            return type.MakeGenericType(type.FindMatchingOpenTypeInHeritage(toMakeFrom).GenericTypeArguments);
        }

        static Type FindMatchingOpenTypeInHeritage(this Type type, Type toSearch)
        {
            foreach (var openGenericInterface in type.GetInterfaces())
                if (openGenericInterface.SharesSameGenericTypeDefinitionAs(toSearch))
                    return toSearch;

            foreach (var openGenericInterface in type.GetInterfaces())
            foreach (var toSearchInterface in toSearch.GetInterfaces())
                if (openGenericInterface.SharesSameGenericTypeDefinitionAs(toSearchInterface))
                    return toSearchInterface;
            return null;
        }

        static bool SharesSameGenericTypeDefinitionAs(this Type type, Type toSearch)
        {
            if (!type.GetTypeInfo().IsGenericType) return false;
            if (!toSearch.GetTypeInfo().IsGenericType) return false;
            return type.GetGenericTypeDefinition() == toSearch.GetGenericTypeDefinition();
        }

        public static bool IsAssignableFromOpenGenericType(this Type type, Type openGenericType)
        {
            return type.GetTypeInfo().IsGenericType
                   && openGenericType.GetTypeInfo().IsAssignableFrom(type.GetGenericTypeDefinition().GetTypeInfo());
        }

        public static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        public static IEnumerable<Type> ThatImplement<TType>(this IEnumerable<Type> types)
        {
            return types.WhereNormalConcrete().WhereImplements<TType>();
        }

        public static IEnumerable<Type> WhereImplements<TImplemented>(this IEnumerable<Type> types)
        {
            return types.Where(
                t =>
                    t.GetNonBaseInterfaces().Contains(typeof(TImplemented))
                    || t.GetBaseInterfaces().Contains(typeof(TImplemented)));
        }

        public static IEnumerable<Type> GetNonBaseInterfaces(this Type type)
        {
            IEnumerable<Type> baseInterfaces = GetBaseInterfaces(type);
            return type.GetInterfaces().Where(t => !baseInterfaces.Contains(t));
        }

        static IEnumerable<Type> GetBaseInterfaces(this Type type)
        {
            var types = new List<Type>();
            Type baseType = type.GetTypeInfo().BaseType;

            if (baseType == typeof(MemberInfo)) return types;

            while (baseType != null)
            {
                types.AddRange(baseType.GetInterfaces());
                baseType = baseType.GetTypeInfo().BaseType;
                if (baseType == typeof(MemberInfo)) return types;
            }

            return types;
        }
        
        public static IEnumerable<Type> WhereNormalConcrete(this IEnumerable<Type> types)
        {
            return types.WhereNonAbstract().WhereNonGeneric().WhereConcrete();
        }

        static IEnumerable<Type> WhereNonAbstract(this IEnumerable<Type> types)
        {
            return types.Where(t => !t.IsAbstract());
        }

        static bool IsAbstract(this Type type)
        {
            return type.GetTypeInfo().IsAbstract;
        }

        static IEnumerable<Type> WhereConcrete(this IEnumerable<Type> types)
        {
            return types.Where(IsConcrete);
        }

        public static bool IsNormalConcrete(this Type type)
        {
            return !type.IsAbstract() && !type.IsGeneric() && type.IsConcrete();
        }

        static bool IsConcrete(this Type type)
        {
            return !type.GetTypeInfo().IsInterface && type.GetTypeInfo().IsClass;
        }

        static IEnumerable<Type> WhereNonGeneric(this IEnumerable<Type> types)
        {
            return types.Where(t => !t.IsGeneric());
        }

        static bool IsGeneric(this Type type)
        {
            return type.GetTypeInfo().ContainsGenericParameters;
        }

        public static MethodInfo GetMethod(this Type type, string methodName, Type[] args)
        {
            return type.GetRuntimeMethod(methodName, args);
        }

        public static IEnumerable<MethodInfo> GetMethods(this Type type)
        {
            return type.GetRuntimeMethods();
        }
    }
}