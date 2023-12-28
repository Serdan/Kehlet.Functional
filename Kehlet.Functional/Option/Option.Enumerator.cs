using System.Collections;

namespace Kehlet.Functional;

partial struct Option<TValue>
{
    public struct Enumerator<T>
        where T : notnull
    {
        private readonly Option<T> option;
        private int index = -1;
        private readonly int length;

        internal Enumerator(Option<T> option)
        {
            this.option = option;
            length = this.option.IsSome ? 1 : 0;
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
                    return option.value;
                }

                throw new InvalidOperationException();
            }
        }
    }

    private class EnumeratorObject<T>(Option<T> option) : IEnumerator<T>
        where T : notnull
    {
        private static readonly IEnumerator<T> EmptyEnumerator =
            new EnumeratorObject<T>(default);

        private int index = -1;
        private readonly int length = option.IsSome ? 1 : 0;

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
                    return option.value;
                }

                throw new InvalidOperationException();
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        internal static IEnumerator<T> Create(Option<T> option) =>
            option.IsSome
                ? new EnumeratorObject<T>(option)
                : EmptyEnumerator;
    }
}
