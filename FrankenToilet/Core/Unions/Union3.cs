using System;
using JetBrains.Annotations;

namespace FrankenToilet.Core.Unions;

/// <summary>
/// A simple union class that can hold one of three reference types.
/// </summary>
/// <remarks>
/// If you wish to use value types, box them
/// using <see cref="System.Runtime.CompilerServices.StrongBox{T}">StrongBox&lt;T&gt;</see>
/// </remarks>
[PublicAPI]
public sealed class Union<T1, T2, T3> : IUnion
    where T1 : class
    where T2 : class
    where T3 : class
{
    public readonly UnionType3 unionType;
    public bool IsT1 => unionType == UnionType3.T1;
    public bool IsT2 => unionType == UnionType3.T2;
    public bool IsT3 => unionType == UnionType3.T3;

    public T1 T1Obj => IsT1 ? (T1)RawObj : throw new InvalidOperationException();
    public T2 T2Obj => IsT2 ? (T2)RawObj : throw new InvalidOperationException();
    public T3 T3Obj => IsT3 ? (T3)RawObj : throw new InvalidOperationException();
    public object RawObj { get; }

    public Union(T1 item) => (unionType, RawObj) = (UnionType3.T1, item);
    public Union(T2 item) => (unionType, RawObj) = (UnionType3.T2, item);
    public Union(T3 item) => (unionType, RawObj) = (UnionType3.T3, item);

    public Union(object obj) => (unionType, RawObj) = obj switch
    {
        T1 => (UnionType3.T1, obj),
        T2 => (UnionType3.T2, obj),
        T3 => (UnionType3.T3, obj),
        _  => throw new ArgumentException($"Object must be of type {typeof(T1)} or {typeof(T2)} or {typeof(T3)}")
    };

    public static implicit operator Union<T1, T2, T3>(T1 item) => new(item);
    public static implicit operator Union<T1, T2, T3>(T2 item) => new(item);
    public static implicit operator Union<T1, T2, T3>(T3 item) => new(item);

    public bool Equals(T1 other) => IsT1 && T1Obj.Equals(other);
    public bool Equals(T2 other) => IsT2 && T2Obj.Equals(other);
    public bool Equals(T3 other) => IsT3 && T3Obj.Equals(other);

    Type[] IUnion.GetPossibleTypes() => [typeof(T1), typeof(T2), typeof(T3)];
}