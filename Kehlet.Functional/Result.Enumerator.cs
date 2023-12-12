using System.Collections;

namespace Kehlet.Functional;

partial struct Result<TValue>
{
    public struct Enumerator<T>
        where T : notnull
    {
        private readonly Result<T> result;
        private int index = -1;
        private readonly int length;

        internal Enumerator(Result<T> result)
        {
            this.result = result;
            length = this.result.IsOk ? 1 : 0;
        }

        public bool MoveNext()
        {
            var newIndex = index + 1;

            if (newIndex <= length)
            {
                index = newIndex;
                return newIndex < length;
            }

            return false;
        }

        public T Current
        {
            get
            {
                if (index < length)
                {
                    return result.value;
                }

                throw new InvalidOperationException();
            }
        }
    }

    private class EnumeratorObject<T>(Result<T> result) : IEnumerator<T>
        where T : notnull
    {
        private static readonly IEnumerator<T> EmptyEnumerator =
            new EnumeratorObject<T>(default);

        private int index = -1;
        private readonly int length = result.IsOk ? 1 : 0;

        public bool MoveNext()
        {
            var newIndex = index + 1;

            if (newIndex <= length)
            {
                index = newIndex;
                return newIndex < length;
            }

            return false;
        }

        public void Reset() =>
            index = -1;

        public T Current
        {
            get
            {
                if (index < length)
                {
                    return result.value;
                }

                throw new InvalidOperationException();
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        internal static IEnumerator<T> Create(Result<T> result) =>
            result.IsOk
                ? new EnumeratorObject<T>(result)
                : EmptyEnumerator;
    }
}
