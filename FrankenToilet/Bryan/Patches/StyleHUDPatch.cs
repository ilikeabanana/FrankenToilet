namespace FrankenToilet.Bryan.Patches;

using FrankenToilet.Core;
using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Patch the silly lil style hud :3 </summary>
[PatchOnEntry]
[HarmonyPatch(typeof(StyleHUD))]
public static class StyleHUDPatch
{
    /// <summary> Goofed style ranks :3 </summary>
    public static Dictionary<string, string> styleEdits = new()
    {
        ["ultrakill.kill"] = "DEAD",
        ["ultrakill.doublekill"] = "<color=orange>DEADx2",
        ["ultrakill.triplekill"] = "<color=orange>DEADx3",
        ["ultrakill.bigkill"] = "BIG DEAD",
        ["ultrakill.bigfistkill"] = "BIG ARM DEAD",
        ["ultrakill.headshot"] = "NECKSHOT",
        ["ultrakill.bigheadshot"] = "BIG NECKSHOT",
        ["ultrakill.limbhit"] = "LIMB SHOT",
        ["ultrakill.interruption"] = "<color=pink>SUPRISE",
        ["ultrakill.arsenal"] = "<color=lime>WEAPONS",
        ["ultrakill.splattered"] = "SPLAT",
        ["ultrakill.instakill"] = "<color=pink>QUICKLYDEAD",
        ["ultrakill.fireworks"] = "<color=lime>MAKE IT RAIN",
        ["ultrakill.airslam"] = "<color=lime>AIR POUND",
        ["ultrakill.airshot"] = "<color=lime>AIRSHIT",
        ["ultrakill.groundslam"] = "GROUND POUND",
        ["ultrakill.overkill"] = "MEGAMURDER",
        ["ultrakill.exploded"] = "BOOM",
        ["ultrakill.fried"] = "SIZLE",
        ["ultrakill.mauriced"] = "ROCK CRUSH",
        ["ultrakill.multikill"] = "<color=orange>DEADxALOT",
        ["ultrakill.finishedoff"] = "<color=lime>FINISH HIM",
        ["ultrakill.iconoclasm"] = "BOOM!!!",
        ["ultrakill.roundtrip"] = "VOYAGE",
    };

    /// <summary> use my style edits first 3:< </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(StyleHUD), "GetLocalizedName")]
    static bool USEMINEGRRR(string id, ref string __result)
    {
        if (styleEdits.TryGetValue(id, out var replacement))
        {
            __result = replacement;
            return false;
        }

        return true;
    }
}