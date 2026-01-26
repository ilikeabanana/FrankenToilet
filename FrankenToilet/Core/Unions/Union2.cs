using System;
using JetBrains.Annotations;

namespace FrankenToilet.Core.Unions;

/// <summary>
/// A simple union class that can hold one of two reference types.
/// </summary>
/// <remarks>
/// If you wish to use value types, box them
/// using <see cref="System.Runtime.CompilerServices.StrongBox{T}">StrongBox&lt;T&gt;</see>
/// </remarks>
[PublicAPI]
public sealed class Union2<T1, T2> : IUnion
    where T1 : class
    where T2 : class
{
    public readonly UnionType2 unionType;
    public bool IsT1 => unionType == UnionType2.T1;
    public bool IsT2 => unionType == UnionType2.T2;
    public T1 T1Obj => IsT1 ? (T1)RawObj : throw new InvalidOperationException();
    public T2 T2Obj => IsT2 ? (T2)RawObj : throw new InvalidOperationException();
    public object RawObj { get; }
    public Union2(T1 item) => (unionType, RawObj) = (UnionType2.T1, item);
    public Union2(T2 item) => (unionType, RawObj) = (UnionType2.T2, item);

    public Union2(object obj) => (unionType, RawObj) = obj switch
    {
        T1 => (UnionType2.T1, obj),
        T2 => (UnionType2.T2, obj),
        _  => throw new ArgumentException($"Object must be of type {typeof(T1)} or {typeof(T2)}")
    };
    public static implicit operator Union2<T1, T2>(T1 item) => new(item);
    public static implicit operator Union2<T1, T2>(T2 item) => new(item);

    public bool Equals(T1 other) => IsT1 && T1Obj.Equals(other);
    public bool Equals(T2 other) => IsT2 && T2Obj.Equals(other);

    Type[] IUnion.GetPossibleTypes() => [typeof(T1), typeof(T2)];
}