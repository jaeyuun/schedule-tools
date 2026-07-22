using System.Collections;
using System.Reflection;

namespace GirderSchedule.Dxf.Styles.Overrides
{
    internal static class DxfOverridePropertyCopier
    {
        public static PropertyInfo? FindProperty(Type? type, string name)
        {
            while (type != null)
            {
                var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (property != null)
                {
                    return property;
                }

                type = type.BaseType;
            }

            return null;
        }

        public static IEnumerable? GetCollectionValues(object? collection)
        {
            if (collection == null)
            {
                return null;
            }

            var values = GetValuesProperty(collection);

            if (values != null)
            {
                return values;
            }

            return collection as IEnumerable;
        }

        public static void ClearCollection(object? collection)
        {
            if (collection == null)
            {
                return;
            }

            var clearMethod = collection.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);

            if (clearMethod == null)
            {
                return;
            }

            TryInvoke(clearMethod, collection, null);
        }

        public static void AddObjectToCollection(object? collection, object? value)
        {
            if (collection == null || value == null)
            {
                return;
            }

            var addMethod = FindAddMethod(collection.GetType(), value.GetType());

            if (addMethod == null)
            {
                return;
            }

            TryInvoke(addMethod, collection, new[] { value });
        }

        public static object? CloneObject(object? value)
        {
            if (value == null)
            {
                return null;
            }

            var cloneMethod = value.GetType().GetMethod("Clone", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

            if (cloneMethod == null)
            {
                return value;
            }

            var clone = TryInvoke(cloneMethod, value, null);

            return clone ?? value;
        }

        private static IEnumerable? GetValuesProperty(object collection)
        {
            var valuesProperty = collection.GetType().GetProperty("Values", BindingFlags.Instance | BindingFlags.Public);

            if (valuesProperty == null || !valuesProperty.CanRead)
            {
                return null;
            }

            return TryGetValue(valuesProperty, collection) as IEnumerable;
        }

        private static MethodInfo? FindAddMethod(Type collectionType, Type valueType)
        {
            var methods = collectionType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];

                if (method.Name != "Add")
                {
                    continue;
                }

                var parameters = method.GetParameters();

                if (parameters.Length != 1)
                {
                    continue;
                }

                if (!parameters[0].ParameterType.IsAssignableFrom(valueType))
                {
                    continue;
                }

                return method;
            }

            return null;
        }

        private static object? TryGetValue(PropertyInfo property, object target)
        {
            try
            {
                return property.GetValue(target, null);
            }
            catch
            {
                return null;
            }
        }

        private static object? TryInvoke(MethodInfo method, object target, object?[]? parameters)
        {
            try
            {
                return method.Invoke(target, parameters);
            }
            catch
            {
                return null;
            }
        }
    }
}
