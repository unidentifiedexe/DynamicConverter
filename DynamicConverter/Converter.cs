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
            if (source is null)
            {
                value = null;
                if (targetType.IsValueType)
                    return false;
                else
                    return true;
            }

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
            var propaties = GeterPropaties(source);


            if (!GetInstance(propaties, out value, type))
            {
                value = default;
                return false;
            }

            foreach (var item in SeterPropaties(type).Where(p => propaties.ContainsKey(p.Name)))
            {
                if(item is FieldInfo fieldInfo)
                {
                    if (TryConvertHelper(propaties[item.Name], out var val, fieldInfo.FieldType))
                        fieldInfo.SetValue(value, val);
                }
                else if(item is PropertyInfo propertyInfo)
                {

                    if (TryConvertHelper(propaties[item.Name], out var val, propertyInfo.PropertyType))
                        propertyInfo.SetValue(value, val);
                }
#if DEBUG
                else
                {
                    throw new Exception();
                }
#endif
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

        private static IEnumerable<MemberInfo> SeterPropaties(Type type)
        {
            var propFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty;
            var fieldFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField;

            return
                type.GetProperties(propFlags)
                .Where(p => p.GetSetMethod(HasFourceSetAttribute(p)) != null)
                .Concat<MemberInfo>(type.GetFields(fieldFlags));

            static bool HasFourceSetAttribute(PropertyInfo info)
            {
                return info?.GetCustomAttribute(typeof(ForceSetAttribute)) != null;
            }
        }


        private static bool GetInstance(IDictionary<string, object> sourceDic, out object value, Type targetType)
        {

            var ctorInfos = targetType.GetConstructors()
                .Select(p => new ParamInfo(p))
                .Where(p => p.IsTurget())
                .OrderByDescending(p => p.Priority)
                .ToArray();

            foreach (var ctorInfo in ctorInfos)
            {
                var args = Itr().ToArray();
                if(ctorInfo.Parametors.Length == args.Length)
                {
                    value = ctorInfo.Info.Invoke(args);
                    return true;
                }
                IEnumerable<object> Itr()
                {

                    foreach (var (info, name) in ctorInfo.Parametors.Zip(ctorInfo.GetParamNames, (info, name) => (info, name)))
                    {
                        if (!sourceDic.TryGetValue(name, out object obj))
                            yield break;
                        if (!TryConvertHelper(obj, out var val, info.ParameterType))
                            yield break;

                        yield return val;
                    }
                }
            }

            value = null;
            return false;
        }

        private struct ParamInfo
        {
            private readonly ConstructorInfo _info;
            public ConstructorInfo Info => _info;

            private readonly OptionalConstructorAttribute _attribute;


            private ParameterInfo[] _parameters;
            public ParameterInfo[] Parametors => _parameters ??= _info.GetParameters();

            public ICollection<string> GetParamNames => _attribute?.ParamNames ?? Array.Empty<string>();

            public int Priority => _attribute?.Priority ?? 0;
            public ParamInfo(ConstructorInfo info)
            {
                _info = info;
                _parameters = null;
                _attribute = _info.GetCustomAttribute<OptionalConstructorAttribute>();
            }

            public bool IsTurget()
            {
                if (Parametors.Length == 0) return true;
                if (_attribute == null) return false;
                if (_attribute.ParamNames.Count == Parametors.Length)
                    return true;
                else 
                    return false;
            }
        }
    }
}
