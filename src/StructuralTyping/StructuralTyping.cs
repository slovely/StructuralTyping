using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace StructuralTyping
{
    public static class A
    {
        private class PropertyInteceptor : IInterceptor
        {
            private readonly IDictionary<string, object> _propertyValues;

            public PropertyInteceptor(IDictionary<string, object> propertyValues)
            {
                _propertyValues = propertyValues;
            }

            public void Intercept(IInvocation invocation)
            {
                var property = _propertyValues.FirstOrDefault(x => x.Key == invocation.Method.Name.Replace("get_", ""));
                if (!property.Equals(default(KeyValuePair<string, object>)))
                {
                    invocation.ReturnValue = property.Value;
                }
            }
        }

        private static ProxyGenerator _generator = new ProxyGenerator();

        public static T New<T>() where T : class
        {
            return _generator.CreateInterfaceProxyWithoutTarget<T>();
        }

        public static T New<T>(object propertyValues) where T : class
        {
            var obj = _generator.CreateInterfaceProxyWithoutTarget<T>(new PropertyInteceptor(propertyValues.ToDictionary()));
            

            return obj;
        }

        private static void SetProperty(this object target, string name, object value)
        {
            target.GetType().GetProperty(name).SetValue(target, value);
        }

        private static IDictionary<string, object> ToDictionary(this object data)
        {
            const BindingFlags publicAttributes = BindingFlags.Public | BindingFlags.Instance;
            return data.GetType().GetProperties(publicAttributes)
                .Where(property => property.CanRead)
                .ToDictionary(property => property.Name, property => property.GetValue(data, null));
        }
    }
}