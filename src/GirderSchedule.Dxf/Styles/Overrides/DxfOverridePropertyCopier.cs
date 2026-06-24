using System;
using System.Collections;
using System.Reflection;

namespace GirderSchedule.Dxf.Styles.Overrides
{
    internal static class DxfOverridePropertyCopier
    {
        public static void CopyWritableProperty(object source, object target, string name)
        {
            if (source == null || target == null)
            {
                return;
            }

            var sourceProperty = FindProperty(source.GetType(), name);
            var targetProperty = FindProperty(target.GetType(), name);

            if (sourceProperty == null || targetProperty == null)
            {
                return;
            }

            if (!sourceProperty.CanRead || !targetProperty.CanWrite)
            {
                return;
            }

            if (!targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
            {
                return;
            }

            try
            {
                var value = sourceProperty.GetValue(source, null);
                targetProperty.SetValue(target, value, null);
            }
            catch
            {
            }
        }

        public static PropertyInfo FindProperty(Type type, string name)
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

        public static IEnumerable GetCollectionValues(object collection)
        {
            if (collection == null)
            {
                return null;
            }

            var valuesProperty = collection.GetType().GetProperty("Values", BindingFlags.Instance | BindingFlags.Public);

            if (valuesProperty != null && valuesProperty.CanRead)
            {
                var values = valuesProperty.GetValue(collection, null) as IEnumerable;

                if (values != null)
                {
                    return values;
                }
            }

            return collection as IEnumerable;
        }

        public static void ClearCollection(object collection)
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

            try
            {
                clearMethod.Invoke(collection, null);
            }
            catch
            {
            }
        }

        public static void AddObjectToCollection(object collection, object value)
        {
            if (collection == null || value == null)
            {
                return;
            }

            var methods = collection.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);

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

                if (!parameters[0].ParameterType.IsAssignableFrom(value.GetType()))
                {
                    continue;
                }

                try
                {
                    method.Invoke(collection, new[] { value });
                    return;
                }
                catch
                {
                    return;
                }
            }
        }

        public static object CloneObject(object value)
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

            try
            {
                return cloneMethod.Invoke(value, null);
            }
            catch
            {
                return value;
            }
        }
    }
}