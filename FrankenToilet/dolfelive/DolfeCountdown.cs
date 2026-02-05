using System;
using System.Collections;
using System.Collections.Generic;
using FrankenToilet.Core.Extensions;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FrankenToilet.dolfelive;

public class DolfeCountdown : MonoBehaviour
{
    public float timeLeft = 20;
    public TextMeshProUGUI textPrefab;
    public Transform container;
    public float minShakeAmount = 0.75f;
    public float maxShakeAmount = 2.75f;
    public float charSpacing = 15f;
    public float maxPulseInterval = 1f;
    public float minPulseInterval = 0.1f;
    public float flashDuration = 0.1f;
    public bool countingDown = false;
    
    private List<TextMeshProUGUI> _charTexts = new List<TextMeshProUGUI>();
    private string _lastStr = "";
    private float _pulseTimer = 0f;
    private float _rainbowTimer = 0f;
    private float _randomTime = 90f;

    private void Start()
    {
        _randomTime = Random.Range(60f, 900f);
    }
    
    void RemakeStr(string newStr)
    {
        if (newStr.Length != _lastStr.Length)
        {
            foreach (var txt in _charTexts)
                Destroy(txt.gameObject);
            _charTexts.Clear();
            
            float totalWidth = (newStr.Length - 1) * charSpacing;
            float startX = -totalWidth ;
            Vector3 localPosition = this.gameObject.transform.localPosition;
            localPosition.x = startX;
            this.gameObject.transform.localPosition = localPosition;
            
            for (int i = 0; i < newStr.Length; i++)
            {
                var newText = Instantiate(textPrefab, container, false);
                _charTexts.Add(newText);
            }
            
            _lastStr = newStr;
        }
    }

    void RainbowCountdown()
    {
        _rainbowTimer += Time.deltaTime;
        float baseTime = _randomTime;
        float amplitude = 50f;
        float speed = 0.5f;

        float currentTime = baseTime + Mathf.Sin(Time.time * speed) * amplitude;
        TimeSpan t = TimeSpan.FromSeconds(currentTime);
        string displayStr = $"{(int)t.TotalMinutes:00}:{t.Seconds:00}.{t.Milliseconds / 10:00}";
        
        RemakeStr(displayStr);
        
        for (int i = 0; i < displayStr.Length; i++)
        {
            float movementOffset = (_rainbowTimer * 0.3f) % 1f;
            float hueOffset = (movementOffset + (float)i / displayStr.Length) % 1f;
            Color color = Color.HSVToRGB(hueOffset, 1f, 1f);
            string colorTag = $"color=#{color.ToHexString()}";
        
            float wobbleAmount = Mathf.Sin(_rainbowTimer * 2f + i * 0.5f) * 3f;
        
            _charTexts[i].text = MapToAtlas(displayStr[i], colorTag);
            _charTexts[i].rectTransform.anchoredPosition = new Vector2(i * charSpacing + wobbleAmount, 0);
        }
    }
    
    void Update()
    {
        if (!countingDown)
        {
            RainbowCountdown();
            
            return;
        }
        
        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Clamp((float)timeLeft, 0f, float.MaxValue);
        // out format: mm:ss.ms
        // have each letter shake
        // pulse red when low and have sfx play

        int minutes = (int)(timeLeft / 60);
        float seconds = timeLeft - minutes * 60;

        string unmappedStr = $"{minutes:00}:{seconds:00.00}";

        RemakeStr(unmappedStr);
        
        float normalizedTime = timeLeft > 10 ? 1f : (float)(timeLeft / 10f);
        float pulseInterval = Mathf.Lerp(minPulseInterval, maxPulseInterval, normalizedTime);
        float currentShake = Mathf.Lerp(maxShakeAmount, minShakeAmount, normalizedTime);

        _pulseTimer += Time.deltaTime;

        bool REDTIME = timeLeft <= 0 || (_pulseTimer % pulseInterval) < flashDuration;
        string colorTag = REDTIME ? $"color=#FF0000FF" : "";

        for (int i = 0; i < unmappedStr.Length; i++)
        {
            float offsetX = Random.Range(-currentShake, currentShake);
            float offsetY = Random.Range(-currentShake, currentShake);
            _charTexts[i].text = MapToAtlas(unmappedStr[i], colorTag);
            _charTexts[i].rectTransform.anchoredPosition = new Vector2(i * charSpacing + offsetX, offsetY);
        }
    }

    string MapToAtlas(char val, string extraTag)
    {
        switch (val)
        {
            case '0': return $"<sprite name=\"Zero\" {extraTag}>";
            case '1': return $"<sprite name=\"One\" {extraTag}>";
            case '2': return $"<sprite name=\"Two\" {extraTag}>";
            case '3': return $"<sprite name=\"Three\" {extraTag}>";
            case '4': return $"<sprite name=\"Four\" {extraTag}>";
            case '5': return $"<sprite name=\"Five\" {extraTag}>";
            case '6': return $"<sprite name=\"Six\" {extraTag}>";
            case '7': return $"<sprite name=\"Seven\" {extraTag}>";
            case '8': return $"<sprite name=\"Eight\" {extraTag}>";
            case '9': return $"<sprite name=\"Nine\" {extraTag}>";
            case ':': return $"<sprite name=\":\" {extraTag}>";
            case '.': return $"<sprite name=\"Period\" {extraTag}>";
            default:
                Debug.LogError($"Undefined character: {val}");
                return "ERROR";
        }
    }
}
