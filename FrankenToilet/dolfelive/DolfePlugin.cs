using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrankenToilet.Core;
using HarmonyLib.PatchExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FrankenToilet.dolfelive;

[EntryPoint]
public class DolfePlugin
{
    [EntryPoint]
    public static void Awake()
    {
        LogHelper.LogDebug("DolfePlugin Awake");
        Application.runInBackground = true;
        
        if (BundleLoader.bundle == null)
        {
            LogHelper.LogError("BundleLoader.bundle is null");
            return;
        }
        
        LogHelper.LogDebug($"{string.Join(", ", BundleLoader.bundle.GetAllAssetNames())}");
        
        // assets/countdowncontainer.prefab, assets/spritesheet.asset, assets/spritesheet.png, assets/text (tmp).prefab
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            Canvas canvas = GameObject.Find("/Canvas")?.GetComponent<Canvas>();
            if (canvas != null)
            {
                GameObject prefab = BundleLoader.bundle.LoadAsset<GameObject>("Assets/Text (TMP).prefab");
                prefab.GetComponent<TextMeshProUGUI>().fontSize = 24;
                
                GameObject container =
                    BundleLoader.bundle.LoadAsset<GameObject>("Assets/CountdownContainer.prefab");
                GameObject containerInstance = GameObject.Instantiate(container, canvas.transform, false);
                
                RectTransform rt = containerInstance.GetComponent<RectTransform>();
                
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0, -40);
                
                RectTransform textRt = prefab.GetComponent<RectTransform>();
                textRt.anchorMin = new Vector2(0.5f, 0.5f);
                textRt.anchorMax = new Vector2(0.5f, 0.5f);
                textRt.pivot = new Vector2(0.5f, 0.5f);
                
                FrankenToilet.dolfelive.DolfeCountdown countdown = containerInstance.AddComponent<FrankenToilet.dolfelive.DolfeCountdown>();
                
                countdown.textPrefab = prefab.GetComponent<TextMeshProUGUI>();
                countdown.container = containerInstance.transform;
            }
            
            StatsManager sm = StatsManager.Instance;
            if (sm != null)
            {
                LogHelper.LogDebug($"Timer: {sm.timer}");
                LogHelper.LogDebug($"timeRanks: {sm.timeRanks.ToJoinedString()}");
                LogHelper.LogDebug($"S rank time: {sm.timeRanks[3]}");
            }
            else
            {
                LogHelper.LogError("StatsManager.Instance is null");
            }
        };
    }
}

public static class ListExtensions
{
    
    public static string ToJoinedString<T>(this List<T> list)
    {
        return string.Join(", ", list.Select(_ => _?.ToString()));
    }
}

public static class ArrayExtensions
{
    
    public static string ToJoinedString<T>(this T[] array)
    {
        return string.Join(", ", array.Select(_ => _?.ToString()));
    }
}



[PatchOnEntry]
public static class PatchClass
{
    [Patch(typeof(Bootstrap), "Start", AT.REDIRECT, "SceneHelper.LoadScene", occurrence: 3)]
    public static void SceneRedirect(string sceneName, bool noblocker = false)
    {
        SceneHelper.LoadScene("uk_construct", noblocker);
    }
    
    [Patch(typeof(StatsManager), "Update", AT.INVOKE, "StatsManager::seconds", occurrence: 0)]
    public static void SecondsInc(float ___seconds)
    {
        LogHelper.LogDebug($"Seconds: {___seconds}");
    }
}