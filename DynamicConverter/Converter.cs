using DynamicConverter.Attributs;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConverter
{
    public class Converter
    {
        public static bool TryConvert<T>(dynamic source, out T value)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            return TryConvertHelper(source, out value);
        }

        public static bool TryConvert<T>(ExpandoObject source, out T value)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            return TryConvertHelper(source, out value);
        }
        public static bool TryConvert<TSource, T>(TSource source, out T value)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return TryConvertHelper(source, out value);
        }

        private static bool TryConvertHelper<TSource>(TSource source, out object value, Type targetType)
        {
            var sourceType = source.GetType();
            if (targetType.IsAssignableFrom(sourceType))
            {
                var clone = sourceType.GetMethod("Clone", Type.EmptyTypes);
                if (clone != null && clone.IsStatic == false)
                    value = clone.Invoke(source, Array.Empty<object>());
                else
                    value = source;
                return true;
            }

            var type = targetType;
            var ctorInfo = type.GetConstructor(Array.Empty<Type>());

            if (ctorInfo == null)
            {
                value = default;
                return false;
            }

            value = ctorInfo.Invoke(new object[0]);

            var propaties = GeterPropaties(source);

            foreach (var item in SeterPropaties(type).Where(p => propaties.ContainsKey(p.Name)))
            {
                var val = propaties[item.Name];
                if (val != null)
                    TryConvertHelper(val, out val, item.PropertyType);
                if (val == null && item.PropertyType.IsValueType)
                {
                    var ctor = item.PropertyType.GetConstructor(Type.EmptyTypes);
                    val = ctor?.Invoke(null, Type.EmptyTypes);
                }
                item.SetValue(value, val);
            }

            return true;
        }

        private static bool TryConvertHelper<TSource, T>(TSource source, out T value)
        {
            var ret = TryConvertHelper<TSource>(source, out var val, typeof(T));
            value = ret ? (T)val : default;
            return ret;
        }
        private static IDictionary<string, object> GeterPropaties(ExpandoObject source)
        {
            return source;
        }


        private static IDictionary<string, object> GeterPropaties<T>(T source)
        {
            if (source is ExpandoObject obj)
                return GeterPropaties(obj);
            else
            {
                var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;
                return (source.GetType()).GetProperties(flags).ToDictionary(p => p.Name, p => p.GetValue(source));
            }
        }


        private static IEnumerable<PropertyInfo> SeterPropaties(Type type)
        {
            var propFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty;
            var fieldFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField;
            //var huga = type.GetProperties(propFlags);
            //var hoge = type.GetProperties(propFlags)
            //    .Where(p => p.GetSetMethod(HasFourceSetAttribute(p)) != null);
            return
                type.GetProperties(propFlags)
                .Where(p => p.GetSetMethod(HasFourceSetAttribute(p)) != null)
                .Concat(type.GetProperties(fieldFlags));


            bool HasFourceSetAttribute(PropertyInfo info)
            {
                return info?.GetCustomAttribute(typeof(ForceSetAttribute)) != null;
            }
        }

    }
}
