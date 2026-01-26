using System;
using System.Threading;
using HarmonyLib;
using Steamworks;
using FrankenToilet.Core.Extensions;
using JetBrains.Annotations;

namespace FrankenToilet.Core;
/// <summary>
/// A thread-safe random number generator seeded with the hash of the current user's SteamID.
/// </summary>
/// <remarks>
///     I don't know what'll happen if the user is not logged on...
///     Check using <see cref="SteamClient.IsLoggedOn"/> before construction
/// </remarks>
[PublicAPI]
public class UserRandom() : Random(SteamClient.SteamId.Value.GetHashCode())
{
    public static UserRandom Shared { get; } = new();
    private SpinLock _lock = new(false);

    public override int Next()
    {
        var taken = false;
        try { _lock.Enter(ref taken); return base.Next(); }
        finally { if (taken) _lock.Exit(false); }
    }
    public override int Next(int maxValue)
    {
        var taken = false;
        try { _lock.Enter(ref taken); return base.Next(maxValue); }
        finally { if (taken) _lock.Exit(false); }
    }

    public override int Next(int minValue, int maxValue)
    {
        var taken = false;
        try { _lock.Enter(ref taken); return base.Next(minValue, maxValue); }
        finally { if (taken) _lock.Exit(false); }
    }

    public override void NextBytes(byte[] buffer)
    {
        var taken = false;
        try { _lock.Enter(ref taken); base.NextBytes(buffer); }
        finally { if (taken) _lock.Exit(false); }
    }

    public override double NextDouble()
    {
        var taken = false;
        try { _lock.Enter(ref taken); return base.NextDouble(); }
        finally { if (taken) _lock.Exit(false); }
    }

    public override void NextBytes(Span<byte> buffer)
    {
        var taken = false;
        try { _lock.Enter(ref taken); base.NextBytes(buffer); }
        finally { if (taken) _lock.Exit(false); }
    }
}