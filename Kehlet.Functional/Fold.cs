namespace Kehlet.Functional;

public readonly struct Fold<TAccumulate>(TAccumulate initialValue)
{
    private readonly Func<TAccumulate, bool>? predicate;

    private Fold(TAccumulate initialValue, Func<TAccumulate, bool> predicate) : this(initialValue) => 
        this.predicate = predicate;

    public Fold<TAccumulate> Where(Func<TAccumulate, bool> predicate) => 
        new(initialValue, predicate);

    public TAccumulate SelectMany<TValue>(Func<TAccumulate, IEnumerable<TValue>> selector, Func<TAccumulate, TValue, TAccumulate> accumulator)
    {
        var values = selector(initialValue);

        var acc = initialValue;
        foreach (var value in values)
        {
            acc = accumulator(acc, value);
            if (predicate?.Invoke(acc) is false)
            {
                return acc;
            }
        }

        return acc;
    }
}
