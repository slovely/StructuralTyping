namespace StructuralTyping
{
    public static class A
    {
        public static T New<T>()
        {
            return default(T);
        }
    }
}