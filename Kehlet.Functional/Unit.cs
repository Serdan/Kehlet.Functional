using System.Collections;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Kehlet.Functional;

public readonly struct Unit :
    ITuple,
    IEquatable<Unit>,
    IComparable,
    IComparable<Unit>,
    IStructuralEquatable,
    IStructuralComparable,
    IParsable<Unit>
{
    object ITuple.this[int index] =>
        throw new IndexOutOfRangeException();

    [JsonIgnore]
    public int Length => 0;

    [Pure]
    public override bool Equals(object? obj) =>
        obj is Unit;

    [Pure]
    public bool Equals(Unit other) =>
        true;

    [Pure]
    bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer) =>
        other is Unit;

    [Pure]
    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is not Unit)
        {
            throw new InvalidOperationException($"Invalid type: {obj.GetType()}");
        }

        return 0;
    }

    [Pure]
    public int CompareTo(Unit other) =>
        0;

    [Pure]
    int IStructuralComparable.CompareTo(object? other, IComparer comparer)
    {
        if (other is null)
        {
            return 1;
        }

        if (other is not Unit)
        {
            throw new InvalidOperationException($"Invalid type: {other.GetType()}");
        }

        return 0;
    }

    [Pure]
    public override int GetHashCode() =>
        0;

    [Pure]
    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) =>
        0;

    [Pure]
    public override string ToString() =>
        "()";

    [Pure]
    public static Unit Parse(string s, IFormatProvider? provider) =>
        string.Equals(s, "()")
            ? default
            : throw new FormatException();

    [Pure]
    public static bool TryParse(string? s, IFormatProvider? provider, out Unit result) =>
        string.Equals(s, "()");

    public static bool operator ==(Unit lhs, Unit rhs) => true;

    public static bool operator !=(Unit lhs, Unit rhs) => false;

    public static implicit operator ValueTuple(Unit _) => default;

    public static implicit operator Unit(ValueTuple _) => default;
}
