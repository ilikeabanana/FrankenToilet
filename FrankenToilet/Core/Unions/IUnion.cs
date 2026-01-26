using System;
using JetBrains.Annotations;

namespace FrankenToilet.Core.Unions;

[PublicAPI]
public interface IUnion
{
    object RawObj { get; }
    Type[] GetPossibleTypes();
}