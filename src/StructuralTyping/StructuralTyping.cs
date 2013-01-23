using Castle.DynamicProxy;

namespace StructuralTyping
{
    public static class A
    {
        private static ProxyGenerator _generator = new ProxyGenerator();

        public static T New<T>() where T : class
        {
            return _generator.CreateInterfaceProxyWithoutTarget<T>();
        }
    }
}