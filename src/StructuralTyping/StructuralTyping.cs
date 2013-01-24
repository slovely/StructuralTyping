using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;

namespace StructuralTyping
{
    public class Method
    {
        public string Name { get; set; }
        public Delegate Delegate { get; set; }
    }

    public static class A
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();

        public static T New<T>(object propertyValues = null, params KeyValuePair<string, Delegate>[] methods) where T : class
        {
            propertyValues = propertyValues ?? new object();
            var obj = _generator.CreateInterfaceProxyWithoutTarget<T>(new PropertyInteceptor(propertyValues.ToDictionary()), new MethodInterceptor(methods));
            return obj;
        }

        private static IDictionary<string, object> ToDictionary(this object data)
        {
            const BindingFlags publicAttributes = BindingFlags.Public | BindingFlags.Instance;
            return data.GetType().GetProperties(publicAttributes)
                       .Where(property => property.CanRead)
                       .ToDictionary(property => property.Name, property => property.GetValue(data, null));
        }

        private class MethodInterceptor : IInterceptor
        {
            private readonly Dictionary<string, Delegate> _methods;

            public MethodInterceptor(IEnumerable<KeyValuePair<string, Delegate>> methods)
            {
                _methods = methods.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method.IsSpecialName) return;

                KeyValuePair<string, Delegate> item = _methods.FirstOrDefault(x => x.Key == invocation.Method.Name);
                if (item.Equals(default(KeyValuePair<string, Delegate>))) return;

                MethodInfo method = item.Value.Method;

                ParameterInfo[] expectedParameters = invocation.Method.GetParameters();
                bool methodMatches = expectedParameters.SequenceEqual(method.GetParameters(), new MethodParamComparer())
                                     && method.ReturnType == invocation.Method.ReturnType;

                if (!methodMatches) return;

                Delegate d = CreateDelegate(method);

                invocation.ReturnValue = d.DynamicInvoke(invocation.Arguments);
            }

            public static Delegate CreateDelegate(MethodInfo method)
            {
                var args = new List<Type>(
                    method.GetParameters().Select(p => p.ParameterType));
                Type delegateType;
                if (method.ReturnType == typeof (void))
                {
                    delegateType = Expression.GetActionType(args.ToArray());
                }
                else
                {
                    args.Add(method.ReturnType);
                    delegateType = Expression.GetFuncType(args.ToArray());
                }
                Delegate d = Delegate.CreateDelegate(delegateType, null, method);
                Console.WriteLine(d);
                return d;
            }
        }

        private class MethodParamComparer : IEqualityComparer<ParameterInfo>
        {
            public bool Equals(ParameterInfo x, ParameterInfo y)
            {
                return x.ParameterType == y.ParameterType;
            }

            public int GetHashCode(ParameterInfo obj)
            {
                return obj.GetHashCode();
            }
        }

        private class PropertyInteceptor : IInterceptor
        {
            private readonly IDictionary<string, object> _propertyValues;

            public PropertyInteceptor(IDictionary<string, object> propertyValues)
            {
                _propertyValues = propertyValues;
            }

            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method.IsSpecialName && invocation.Method.Name.StartsWith("get_", StringComparison.Ordinal))
                {
                    KeyValuePair<string, object> property = _propertyValues.FirstOrDefault(x => x.Key == invocation.Method.Name.Remove(0, 4));
                    if (!property.Equals(default(KeyValuePair<string, object>)))
                    {
                        invocation.ReturnValue = property.Value;
                    }
                    else
                    {
                        if (invocation.Method.ReturnType.IsValueType)
                        {
                            invocation.ReturnValue = Activator.CreateInstance(invocation.Method.ReturnType);
                        }
                        else
                        {
                            invocation.ReturnValue = null;
                        }
                    }
                }
                else if (invocation.Method.IsSpecialName && invocation.Method.Name.StartsWith("set_"))
                {
                    _propertyValues[invocation.Method.Name.Remove(0, 4)] = invocation.GetArgumentValue(0);
                }
                else
                {
                    invocation.Proceed();
                }
            }
        }
    }
}