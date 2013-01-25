using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;

namespace StructuralTyping
{
    public static class A
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();

        public static T New<T>(object propertyValues = null, object target = null) where T : class
        {
            propertyValues = propertyValues ?? new object();
            var obj = _generator.CreateInterfaceProxyWithoutTarget<T>(new PropertyInteceptor(propertyValues.ToDictionary(), target));
            return obj;
        }

        private static IDictionary<string, object> ToDictionary(this object data)
        {
            const BindingFlags publicAttributes = BindingFlags.Public | BindingFlags.Instance;
            return data.GetType().GetProperties(publicAttributes)
                       .Where(property => property.CanRead)
                       .ToDictionary(property => property.Name, property => property.GetValue(data, null));
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
            private readonly object _target;

            public PropertyInteceptor(IDictionary<string, object> propertyValues)
            {
                _propertyValues = propertyValues;
            }

            public PropertyInteceptor(IDictionary<string, object> propertyValues, object target)
            {
                _propertyValues = propertyValues;
                _target = target;
            }

            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method.IsSpecialName && invocation.Method.Name.StartsWith("get_", StringComparison.Ordinal))
                {
                    var property = _propertyValues.FirstOrDefault(x => x.Key == invocation.Method.Name.Remove(0, 4));
                    if (!property.Equals(default(KeyValuePair<string, object>)))
                    {
                        invocation.ReturnValue = property.Value;
                    }
                    else
                    {
                        invocation.ReturnValue = invocation.Method.ReturnType.IsValueType ? Activator.CreateInstance(invocation.Method.ReturnType) : null;
                    }
                }
                else if (invocation.Method.IsSpecialName && invocation.Method.Name.StartsWith("set_"))
                {
                    _propertyValues[invocation.Method.Name.Remove(0, 4)] = invocation.GetArgumentValue(0);
                }
                else
                {
                    if (invocation.Method.IsSpecialName) return;

                    var item = _propertyValues.FirstOrDefault(x => x.Key == invocation.Method.Name);
                    if (item.Equals(default(KeyValuePair<string, object>))) return;

                    var method = ((Delegate) item.Value).Method;

                    var expectedParameters = invocation.Method.GetParameters();
                    var methodMatches = expectedParameters.SequenceEqual(method.GetParameters(), new MethodParamComparer())
                                         && method.ReturnType == invocation.Method.ReturnType;

                    if (!methodMatches) return;

                    var d = CreateDelegate(method);
                    if (method.ReturnType == typeof (void))
                    {
                        d.DynamicInvoke(invocation.Arguments);
                        return;
                    }
                    invocation.ReturnValue = d.DynamicInvoke(invocation.Arguments);
                }
            }

            private Delegate CreateDelegate(MethodInfo method)
            {
                var args = method.GetParameters().Select(p => p.ParameterType).ToList();
                Type delegateType;
                if (method.ReturnType == typeof(void))
                {
                    delegateType = Expression.GetActionType(args.ToArray());
                }
                else
                {
                    args.Add(method.ReturnType);
                    delegateType = Expression.GetFuncType(args.ToArray());
                }
                return Delegate.CreateDelegate(delegateType, _target, method);
            }
        }
    }
}