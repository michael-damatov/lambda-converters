namespace Tests.Shared
{
    internal static class ArrayUtils
    {
#if NET45
        static class Array<T>
        {
            [JetBrains.Annotations.NotNull]
            public static readonly T[] Empty = new T[0];
        }
#endif

        [JetBrains.Annotations.NotNull]
        public static T[] GetEmpty<T>()
#if NET45
            => Array<T>.Empty;
#else
            => System.Array.Empty<T>();
#endif

    }
}