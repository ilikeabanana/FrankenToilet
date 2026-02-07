#pragma warning disable CS8618
using FrankenToilet;
using FrankenToilet.Core;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityObject = UnityEngine.Object;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/*
TODO:
1. 2
*/

namespace FrankenToilet.somebilly {
    [EntryPoint]
    // THIS IS THE PLUGIN.
    public class Plugin {
        public static Bib bib;

        [EntryPoint]
        public static void Initialize() {
            LogHelper.LogInfo("SOMETHING MALICIOUS IS BREWING...");
            SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode lsm) {
            if (Plugin.bib != null) {
                return;
            }
            AddThings();
        }
        public static void AddThings() {
            GameObject theStuff = new GameObject("frankenstuff");
            Plugin.bib = theStuff.AddComponent<Bib>();
            UnityObject.DontDestroyOnLoad(theStuff);
        }
    }


    // THIS IS THE INFO TEXT.
    public class GoodInfoText : MonoBehaviour {
        RectTransform rect;
        GameObject lineWelcome; TextMeshProUGUI textWelcome;
        GameObject lineDate; TextMeshProUGUI textDate;
        GameObject lineRandom; TextMeshProUGUI textRandom;
        GameObject lineScene; TextMeshProUGUI textScene;
        float timerSecond = 0f;

        public void Start() {
            VerticalLayoutGroup layout = this.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 20f;
            layout.childAlignment = UnityEngine.TextAnchor.UpperCenter;

            // place at the top
            rect = GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -10f);
            rect.pivot = new Vector2(0.5f, 1f);

            lineWelcome = new GameObject("LineWelcome");
            lineWelcome.transform.SetParent(layout.transform);
            textWelcome = lineWelcome.AddComponent<TextMeshProUGUI>();
            textWelcome.fontSize = 28;
            textWelcome.enableWordWrapping = false;
            textWelcome.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;

            lineDate = new GameObject("LineDate");
            lineDate.transform.SetParent(layout.transform);
            GoodSwayer lineDateSwayer = lineDate.AddComponent<GoodSwayer>();
            lineDateSwayer.speed = UnityEngine.Random.Range(1.75f, 4f);
            textDate = lineDate.AddComponent<TextMeshProUGUI>();
            textDate.fontSize = 28;
            textDate.enableWordWrapping = false;
            textDate.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;

            lineRandom = new GameObject("LineRandom");
            lineRandom.transform.SetParent(layout.transform);
            GoodScaler lineRandomScaler = lineRandom.AddComponent<GoodScaler>();
            lineRandomScaler.speed = UnityEngine.Random.Range(1.75f, 4f);
            textRandom = lineRandom.AddComponent<TextMeshProUGUI>();
            textRandom.fontSize = 28;
            textRandom.enableWordWrapping = false;
            textRandom.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;

            lineScene = new GameObject("LineScene");
            lineScene.transform.SetParent(layout.transform);
            GoodSwayer lineSceneSwayer = lineScene.AddComponent<GoodSwayer>();
            lineSceneSwayer.speed = UnityEngine.Random.Range(1.75f, 4f);
            textScene = lineScene.AddComponent<TextMeshProUGUI>();
            textScene.fontSize = 28;
            textScene.enableWordWrapping = false;
            textScene.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            UpdateTexts();
        }

        public void Update() {
            timerSecond += Time.deltaTime;
            if (timerSecond >= 1f) {
                timerSecond = 0f;
                UpdateTexts();
            }
        }

        public void UpdateTexts() {
            textWelcome.text = "<b>WELCOME TO FRANKENTOILET. TODAY IS:</b>";
            textDate.text = $"<b>{System.DateTime.Now.ToString("MMMM dd, yyyy")}</b>";
            textRandom.text = $"<b>Random number: {UnityEngine.Random.Range(0, 1000001)}</b>";
            textScene.text = $"<b>Current scene: {SceneHelper.CurrentScene}</b>";
        }
    }

    // THIS IS THE SWAYER.
    public class GoodSwayer : MonoBehaviour {
        public float progress = 0f;
        public float speed = 2f;
        public float minAngle = -5f;
        public float maxAngle = 5f;
        public void Update() {
            progress += Time.deltaTime * speed;
            progress = progress % (2f * Mathf.PI);

            float normalizedProgress = 0.5f * (Mathf.Sin(progress) + 1f);
            float newAngle = Mathf.Lerp(minAngle, maxAngle, normalizedProgress);
            this.transform.eulerAngles = new Vector3(0f, 0f, newAngle);
        }
    }

    // THIS IS THE SCALER.
    public class GoodScaler : MonoBehaviour {
        public float progress = 0f;
        public float speed = 2f;
        public float minScale = 0.9f;
        public float maxScale = 1.1f;
        public void Update() {
            progress += Time.deltaTime * speed;
            progress = progress % (2f * Mathf.PI);

            float normalizedProgress = 0.5f * (Mathf.Sin(progress) + 1f);
            float newScale = Mathf.Lerp(minScale, maxScale, normalizedProgress);
            this.transform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }


    // ---------------
    // ----- HUD -----
    // ---------------
    [PatchOnEntry]
    [HarmonyPatch]
    public static class HUDPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(OptionsMenuToManager), "Start")]
        public static void Postfix(OptionsMenuToManager __instance) {
            if (__instance.name != "Canvas") {
                return;
            }
            GameObject info = new GameObject("FrankenInfo");
            info.transform.SetParent(__instance.transform);
            info.AddComponent<GoodInfoText>();
        }
    }



    // THIS IS THE BIB.
    public class Bib : MonoBehaviour {
        public static Texture2D Confetti; public static AudioClip ConfettiSound;

        // MACHINES
        public static AudioClip VoiceV1;
        public static AudioClip VoiceSwordsmachine; public static AudioClip VoiceSwordsmachineAgony; public static AudioClip VoiceSwordsmachineTundra;
        public static AudioClip VoiceDrone;
        public static AudioClip VoiceStreetcleaner;
        public static AudioClip VoiceV2; public static AudioClip VoiceV2_2;
        public static AudioClip VoiceMindflayer; public static AudioClip VoiceMindflayerIdoled;
        public static AudioClip VoiceSentry;
        public static AudioClip VoiceGutterman;
        public static AudioClip VoiceGuttertank;
        public static AudioClip VoiceLandmine;
        public static AudioClip VoiceEarthmover; public static AudioClip VoiceEarthmoverBrain;
        public static AudioClip VoiceRocketLauncher; public static AudioClip VoiceMortar; public static AudioClip VoiceTower;

        // HUSKS
        public static AudioClip VoiceFilth;
        public static AudioClip VoiceStray;
        public static AudioClip VoiceSchism;
        public static AudioClip VoiceSoldier;
        public static AudioClip VoiceMinosCorpse; public static AudioClip VoiceMinosCorpseHand; public static AudioClip VoiceMinosCorpseLong; public static AudioClip VoiceMinosCorpseParasite;
        public static AudioClip VoiceStalker;
        public static AudioClip VoiceSisypheanInsurrectionist; public static AudioClip VoiceSisypheanInsurrectionistAngry; public static AudioClip VoiceSisypheanInsurrectionistRude;
        public static AudioClip VoiceFerryman;

        // DEMONS
        public static AudioClip VoiceMaliciousFace;
        public static AudioClip VoiceCerberus; public static AudioClip VoiceCerberusBig;
        public static AudioClip VoiceHideousMass;
        public static AudioClip VoiceIdol;
        public static AudioClip VoiceLeviathan5_3; public static AudioClip VoiceLeviathanIntro; public static AudioClip VoiceLeviathan;
        public static AudioClip VoiceMannequin;
        public static AudioClip VoiceMinotaur;

        // ANGELS
        public static AudioClip VoiceGabriel; public static AudioClip VoiceGagabriel;
        public static AudioClip VoiceVirtue;

        // OTHER
        public static AudioClip VoiceSomethingWicked;
        public static AudioClip VoiceCancerousRodent; public static AudioClip VoiceVeryCancerousRodent;
        public static AudioClip VoiceMysteriousDruidKnightAndOwl;
        public static AudioClip VoiceBigJohninator;
        public static AudioClip VoiceFleshPrison;
        public static AudioClip VoiceMinosPrime;
        public static AudioClip VoiceFleshPanopticon;
        public static AudioClip VoiceEye; public static AudioClip VoiceBigEye; public static AudioClip VoiceMiniMaurice;
        public static AudioClip VoiceSisyphusPrime;
        public static AudioClip VoicePuppet;
        public static AudioClip VoiceMirage;
        public static AudioClip VoiceGianniMatragrano;


        public static Transform GetCanvas() {
			Scene activeScene = SceneManager.GetActiveScene();
			Transform canvas = (from obj in activeScene.GetRootGameObjects()
				where obj.name == "Canvas"
				select obj).First().transform;
			return canvas;
		}


        public static Texture2D LoadEmbeddedTexture(string resourceName) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) {
                LogHelper.LogError($"Embedded resource '{resourceName}' not found");
                return null;
            }

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(buffer)) {
                return tex;
            } else {
                LogHelper.LogError("Failed to load embedded texture");
                return null;
            }
        }

        public void ObtainVoice(string path, System.Action<AudioClip> onLoaded) {
            StartCoroutine(LoadAudioFromFile(path, onLoaded));
        }

        public static IEnumerator LoadAudioFromFile(string path, System.Action<AudioClip> onLoaded) {
            string fullPath = Path.Combine(Paths.PluginPath, "FrankenToilet/" + path);
            string url = "file://" + fullPath.Replace("\\", "/");
            
            using (var request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS)) {
                yield return request.SendWebRequest();
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                string clipName = path.Split("/")[^1].Split(".")[0];
                clip.name = clipName;
                onLoaded?.Invoke(clip);
            }
        }

        public void Start() {
            MegaAss();
        }
        
        public void MegaAss() {
            Confetti = LoadEmbeddedTexture("FrankenToilet.somebilly.CONFETTI.png");
            ObtainVoice("ENEMY_VOICE/CONFETTI_SOUND.ogg", clip => {Bib.ConfettiSound = clip;});

            // MACHINES
            ObtainVoice("ENEMY_VOICE/V1.ogg", clip => {Bib.VoiceV1 = clip;});
            ObtainVoice("ENEMY_VOICE/SWORDSMACHINE.ogg", clip => {Bib.VoiceSwordsmachine = clip;});
            ObtainVoice("ENEMY_VOICE/SWORDSMACHINE_AGONY.ogg", clip => {Bib.VoiceSwordsmachineAgony = clip;});
            ObtainVoice("ENEMY_VOICE/SWORDSMACHINE_TUNDRA.ogg", clip => {Bib.VoiceSwordsmachineTundra = clip;});
            ObtainVoice("ENEMY_VOICE/DRONE.ogg", clip => {Bib.VoiceDrone = clip;});
            ObtainVoice("ENEMY_VOICE/STREETCLEANER.ogg", clip => {Bib.VoiceStreetcleaner = clip;});
            ObtainVoice("ENEMY_VOICE/V2.ogg", clip => {Bib.VoiceV2 = clip;});
            ObtainVoice("ENEMY_VOICE/V2_2.ogg", clip => {Bib.VoiceV2_2 = clip;});
            ObtainVoice("ENEMY_VOICE/MINDFLAYER.ogg", clip => {Bib.VoiceMindflayer = clip;});
            ObtainVoice("ENEMY_VOICE/MINDFLAYER_IDOLED.ogg", clip => {Bib.VoiceMindflayerIdoled = clip;});
            ObtainVoice("ENEMY_VOICE/SENTRY.ogg", clip => {Bib.VoiceSentry = clip;});
            ObtainVoice("ENEMY_VOICE/GUTTERMAN.ogg", clip => {Bib.VoiceGutterman = clip;});
            ObtainVoice("ENEMY_VOICE/GUTTERTANK.ogg", clip => {Bib.VoiceGuttertank = clip;});
            ObtainVoice("ENEMY_VOICE/LANDMINE.ogg", clip => {Bib.VoiceLandmine = clip;});
            ObtainVoice("ENEMY_VOICE/EARTHMOVER.ogg", clip => {Bib.VoiceEarthmover = clip;});
            ObtainVoice("ENEMY_VOICE/EARTHMOVER_BRAIN.ogg", clip => {Bib.VoiceEarthmoverBrain = clip;});
            ObtainVoice("ENEMY_VOICE/ROCKET_LAUNCHER.ogg", clip => {Bib.VoiceRocketLauncher = clip;});
            ObtainVoice("ENEMY_VOICE/MORTAR.ogg", clip => {Bib.VoiceMortar = clip;});
            ObtainVoice("ENEMY_VOICE/TOWER.ogg", clip => {Bib.VoiceTower = clip;});

            // HUSKS
            ObtainVoice("ENEMY_VOICE/FILTH.ogg", clip => {Bib.VoiceFilth = clip;});
            ObtainVoice("ENEMY_VOICE/STRAY.ogg", clip => {Bib.VoiceStray = clip;});
            ObtainVoice("ENEMY_VOICE/SCHISM.ogg", clip => {Bib.VoiceSchism = clip;});
            ObtainVoice("ENEMY_VOICE/SOLDIER.ogg", clip => {Bib.VoiceSoldier = clip;});
            ObtainVoice("ENEMY_VOICE/MINOS_CORPSE.ogg", clip => {Bib.VoiceMinosCorpse = clip;});
            ObtainVoice("ENEMY_VOICE/MINOS_CORPSE_HAND.ogg", clip => {Bib.VoiceMinosCorpseHand = clip;});
            ObtainVoice("ENEMY_VOICE/MINOS_CORPSE_LONG.ogg", clip => {Bib.VoiceMinosCorpseLong = clip;});
            ObtainVoice("ENEMY_VOICE/MINOS_CORPSE_PARASITE.ogg", clip => {Bib.VoiceMinosCorpseParasite = clip;});
            ObtainVoice("ENEMY_VOICE/STALKER.ogg", clip => {Bib.VoiceStalker = clip;});
            ObtainVoice("ENEMY_VOICE/SISYPHEAN_INSURRECTIONIST.ogg", clip => {Bib.VoiceSisypheanInsurrectionist = clip;});
            ObtainVoice("ENEMY_VOICE/SISYPHEAN_INSURRECTIONIST_ANGRY.ogg", clip => {Bib.VoiceSisypheanInsurrectionistAngry = clip;});
            ObtainVoice("ENEMY_VOICE/SISYPHEAN_INSURRECTIONIST_RUDE.ogg", clip => {Bib.VoiceSisypheanInsurrectionistRude = clip;});
            ObtainVoice("ENEMY_VOICE/FERRYMAN.ogg", clip => {Bib.VoiceFerryman = clip;});

            // DEMONS
            ObtainVoice("ENEMY_VOICE/MALICIOUS_FACE.ogg", clip => {Bib.VoiceMaliciousFace = clip;});
            ObtainVoice("ENEMY_VOICE/CERBERUS.ogg", clip => {Bib.VoiceCerberus = clip;});
            ObtainVoice("ENEMY_VOICE/CERBERUS_BIG.ogg", clip => {Bib.VoiceCerberusBig = clip;});
            ObtainVoice("ENEMY_VOICE/HIDEOUS_MASS.ogg", clip => {Bib.VoiceHideousMass = clip;});
            ObtainVoice("ENEMY_VOICE/IDOL.ogg", clip => {Bib.VoiceIdol = clip;});
            ObtainVoice("ENEMY_VOICE/LEVIATHAN_5-3.ogg", clip => {Bib.VoiceLeviathan5_3 = clip;});
            ObtainVoice("ENEMY_VOICE/LEVIATHAN_INTRO.ogg", clip => {Bib.VoiceLeviathanIntro = clip;});
            ObtainVoice("ENEMY_VOICE/LEVIATHAN.ogg", clip => {Bib.VoiceLeviathan = clip;});
            ObtainVoice("ENEMY_VOICE/MANNEQUIN.ogg", clip => {Bib.VoiceMannequin = clip;});
            ObtainVoice("ENEMY_VOICE/MINOTAUR.ogg", clip => {Bib.VoiceMinotaur = clip;});

            // ANGELS
            ObtainVoice("ENEMY_VOICE/GABRIEL.ogg", clip => {Bib.VoiceGabriel = clip;});
            ObtainVoice("ENEMY_VOICE/GAGABRIEL.ogg", clip => {Bib.VoiceGagabriel = clip;});
            ObtainVoice("ENEMY_VOICE/VIRTUE.ogg", clip => {Bib.VoiceVirtue = clip;});

            // OTHER
            ObtainVoice("ENEMY_VOICE/SOMETHING_WICKED.ogg", clip => {Bib.VoiceSomethingWicked = clip;});
            ObtainVoice("ENEMY_VOICE/CANCEROUS_RODENT.ogg", clip => {Bib.VoiceCancerousRodent = clip;});
            ObtainVoice("ENEMY_VOICE/VERY_CANCEROUS_RODENT.ogg", clip => {Bib.VoiceVeryCancerousRodent = clip;});
            ObtainVoice("ENEMY_VOICE/MYSTERIOUS_DRUID_KNIGHT_AND_OWL.ogg", clip => {Bib.VoiceMysteriousDruidKnightAndOwl = clip;});
            ObtainVoice("ENEMY_VOICE/BIG_JOHNINATOR.ogg", clip => {Bib.VoiceBigJohninator = clip;});
            ObtainVoice("ENEMY_VOICE/FLESH_PRISON.ogg", clip => {Bib.VoiceFleshPrison = clip;});
            ObtainVoice("ENEMY_VOICE/MINOS_PRIME.ogg", clip => {Bib.VoiceMinosPrime = clip;});
            ObtainVoice("ENEMY_VOICE/FLESH_PANOPTICON.ogg", clip => {Bib.VoiceFleshPanopticon = clip;});
            ObtainVoice("ENEMY_VOICE/SISYPHUS_PRIME.ogg", clip => {Bib.VoiceSisyphusPrime = clip;});
            ObtainVoice("ENEMY_VOICE/EYE.ogg", clip => {Bib.VoiceEye = clip;});
            ObtainVoice("ENEMY_VOICE/BIG_EYE.ogg", clip => {Bib.VoiceBigEye = clip;});
            ObtainVoice("ENEMY_VOICE/MINI_MAURICE.ogg", clip => {Bib.VoiceMiniMaurice = clip;});
            ObtainVoice("ENEMY_VOICE/PUPPET.ogg", clip => {Bib.VoicePuppet = clip;});
            ObtainVoice("ENEMY_VOICE/MIRAGE.ogg", clip => {Bib.VoiceMirage = clip;});
            ObtainVoice("ENEMY_VOICE/GIANNI_MATRAGRANO.ogg", clip => {Bib.VoiceGianniMatragrano = clip;});
        }

        // minotaur is added twice because
        public void AddAndPlayVoice(GameObject obj, AudioClip voice, float volume = 0.65f) {
            if (voice == null) {
                return;
            }

            if (Bib.CheckForVoiceObject(obj, voice.name)) {
                return;
            }

            GameObject voiceObject = new GameObject("FrankenVoiceObject");
            voiceObject.transform.SetParent(obj.transform);
            AudioSource audio = voiceObject.AddComponent<AudioSource>();
            audio.clip = voice;
            audio.volume = volume;
            audio.PlayDelayed(UnityEngine.Random.Range(0.0f, 0.1f));

            StartCoroutine(WaitAndMute(audio));
        }

        public static bool CheckForVoiceObject(GameObject obj, string voiceName) {
            Transform tryFind = obj.transform.Find("FrankenVoiceObject");
            if (tryFind == null) {
                return false;
            }
            AudioSource tryAudio = tryFind.GetComponent<AudioSource>();
            if (tryAudio == null) {
                return false;
            }
            if (tryAudio.clip.name == voiceName) {
                return true;
            }
            return false;
        }

        public static IEnumerator WaitAndMute(AudioSource audio) {
            while (audio.isPlaying) {
                yield return null;
            }
            audio.mute = true;
        }


        public void DoConfetti() {
            Transform canvas = Bib.GetCanvas();

            if (canvas == null) {
                return;
            }


            GameObject confettiObject = new GameObject("Confetti");
            confettiObject.transform.SetParent(canvas);
            Plugin.bib.AddAndPlayVoice(confettiObject, Bib.ConfettiSound);
            // confettiObject.transform.localPosition = new Vector3(0, 0, 0);
            confettiObject.transform.localScale = new Vector3(1, 1, 1);
            Vector3 pos = confettiObject.transform.position;

            Image image = confettiObject.AddComponent<Image>();
            image.sprite = Sprite.Create(Bib.Confetti, new Rect(0, 0, Confetti.width, Confetti.height), new Vector2(0.5f, 0.5f));

            RectTransform rect = confettiObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, canvasRect.sizeDelta.y);

            confettiObject.AddComponent<RemoveAfter>();
            ConfettiFall fall = confettiObject.AddComponent<ConfettiFall>();
            fall.speed = Screen.height;
        }
    }




    // --------------------
    // ----- CONFETTI -----
    // --------------------
    public class RemoveAfter : MonoBehaviour {
        public float time = 4f;
        public float currentTime = 0f;
        void Update() {
            currentTime += Time.deltaTime;
            if (currentTime >= time) {
                UnityObject.Destroy(this.gameObject);
            }
        }
    }

    public class ConfettiFall : MonoBehaviour {
        public float speed = 100f;
        void Update() {
            Vector3 pos = this.transform.position;
            this.transform.position = new Vector3(pos.x, pos.y - speed * Time.deltaTime, pos.z);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class ConfettiPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(StyleHUD), "rankIndex", MethodType.Setter)]
        public static void Postfix(int value, StyleHUD __instance) {
            if (value == 7) {
                Plugin.bib.DoConfetti();
            }
        }
    }


    // --------------------
    // ----- MACHINES -----
    // --------------------
    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceV1Patch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(NewMovement), "Start")]
        public static void Postfix(NewMovement __instance) {
            if (SceneHelper.CurrentScene == "Level 2-S") {
                return;
            }
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceV1);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceSwordsmachinePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SwordsMachine), "Start")]
        public static void Postfix(SwordsMachine __instance) {
            if (__instance.name.Contains("Agony")) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceSwordsmachineAgony);
                return;
            } else if (__instance.name.Contains("Tundra")) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceSwordsmachineTundra);
                return;
            }
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceSwordsmachine);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceDronePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Drone), "Start")]
        public static void Postfix(Drone __instance, ref EnemyIdentifier ___eid) {
            if (__instance.name.Contains("FleshCamera")) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceBigEye);
                return;
            } else if (__instance.name.Contains("Flesh")) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceEye);
                return;
            } else if (__instance.name.Contains("Skull")) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMiniMaurice);
                return;
            }
            switch (___eid.enemyType) {
                case EnemyType.Drone:
                    Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceDrone);
                    break;
                case EnemyType.Virtue:
                    Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceVirtue);
                    break;
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceStreetcleanerPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Streetcleaner), "Start")]
        public static void Postfix(Streetcleaner __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceStreetcleaner);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceV2Patch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(V2), "Start")]
        public static void Postfix(V2 __instance) {
            if (__instance.name.Contains("Green")) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceV2_2);
            } else if (__instance.name.Contains("John")) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceBigJohninator);
            } else {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceV2);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMindflayerPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mindflayer), "Start")]
        public static void Postfix(Mindflayer __instance) {
            // p-2
            if (SceneManager.GetActiveScene().name == "1f290c2101e628540bf9c6d1d2140750") {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMindflayerIdoled, 0.8f);
                return;
            }
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMindflayer);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceSentryPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Turret), "Start")]
        public static void Postfix(Turret __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceSentry);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceGuttermanPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Gutterman), "Start")]
        public static void Postfix(Gutterman __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceGutterman);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceGuttertankPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Guttertank), "Start")]
        public static void Postfix(Guttertank __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceGuttertank);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceLandminePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Landmine), "Start")]
        public static void Postfix(Landmine __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceLandmine);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceEarthmoverPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SceneHelper), "OnSceneLoaded")]
        public static void Postfix(SceneHelper __instance) {
            if (SceneHelper.CurrentScene == "Level 7-4") {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceEarthmover);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceEarthmoverBrainPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Machine), "Start")]
        public static void Postfix(Machine __instance) {
            if (__instance.name == "Brain") {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceEarthmoverBrain);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceRocketLauncherPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DroneFlesh), "Start")]
        public static void Postfix(DroneFlesh __instance) {
            if (__instance.transform.parent.name.Contains("Rocket")) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceRocketLauncher);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMortarTowerPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MortarLauncher), "Start")]
        public static void Postfix(MortarLauncher __instance) {
            if (__instance.mortar.name.Contains("HH")) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMortar);
                return;
            }
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceTower);
        }
    }


    // -----------------
    // ----- HUSKS -----
    // -----------------
    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceFilthPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ZombieMelee), "Start")]
        public static void Postfix(ZombieMelee __instance) {
            if (SceneHelper.CurrentScene == "CreditsMuseum2") {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceGianniMatragrano, 0.35f);
                return;
            }
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceFilth);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceZombiesPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ZombieProjectiles), "Start")]
        public static void Postfix(ZombieProjectiles __instance, ref EnemyIdentifier ___eid) {
            switch (___eid.enemyType) {
                case EnemyType.Stray:
                    Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceStray);
                    break;
                case EnemyType.Schism:
                    Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceSchism);
                    break;
                case EnemyType.Soldier:
                    Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceSoldier);
                    break;
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinosCorpsePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SceneHelper), "OnSceneLoaded")]
        public static void Postfix(SceneHelper __instance) {
            string[] MinosScenes = {"Level 2-1", "Level 2-2", "Level 2-3"};
            if (MinosScenes.Contains(SceneHelper.CurrentScene)) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMinosCorpse);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinosCorpseHandPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MinosArm), "Start")]
        public static void Postfix(MinosArm __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMinosCorpseHand);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinosCorpseLongPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MinosBoss), "Start")]
        public static void Postfix(MinosBoss __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMinosCorpseLong);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinosCorpseParasitePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Parasite), "Start")]
        public static void Postfix(Parasite __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMinosCorpseParasite);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceStalkerPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Stalker), "Start")]
        public static void Postfix(Stalker __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceStalker);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceSisypheanInsurrectionistPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Sisyphus), "Start")]
        public static void Postfix(Sisyphus __instance) {
            if (__instance.name.Contains("Angry")) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceSisypheanInsurrectionistAngry);
                return;
            } else if (__instance.name.Contains("Rude")) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceSisypheanInsurrectionistRude);
                return;
            }
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceSisypheanInsurrectionist);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceFerrymanPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ferryman), "Start")]
        public static void Postfix(Ferryman __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceFerryman);
        }
    }


    // ------------------
    // ----- DEMONS -----
    // ------------------
    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMaliciousFacePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SpiderBody), "Start")]
        public static void Postfix(SpiderBody __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMaliciousFace);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceCerberusPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(StatueBoss), "Start")]
        public static void Postfix(StatueBoss __instance) {
            if (SceneHelper.CurrentScene == "Level 0-5" || (SceneHelper.CurrentScene == "Level 7-1" && __instance.transform.root.Find("4 - Interior Exterior") != null)) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceCerberusBig);
                return;
            }
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceCerberus);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMassPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mass), "Start")]
        public static void Postfix(Mass __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceHideousMass);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceIdolPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Idol), "Awake")]
        public static void Postfix(Idol __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceIdol);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceLeviathan5_3Patch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ComplexSplasher), "Awake")]
        public static void Postfix(ComplexSplasher __instance) {
            if (SceneHelper.CurrentScene == "Level 5-3") {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceLeviathan5_3);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceLeviathanIntroPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LightningStrikeDecorative), "FlashStart")]
        public static void Postfix(LightningStrikeDecorative __instance) {
            if (__instance.transform.parent.name.Contains("Tease") && SceneHelper.CurrentScene == "Level 5-4") {
                Plugin.bib.AddAndPlayVoice(__instance.transform.root.gameObject, Bib.VoiceLeviathanIntro, 0.8f);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceLeviathanPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LeviathanHead), "Start")]
        public static void Postfix(LeviathanHead __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceLeviathan);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMannequinPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mannequin), "Start")]
        public static void Postfix(Mannequin __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMannequin);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinotaurChasePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MinotaurChase), "Start")]
        public static void Postfix(MinotaurChase __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMinotaur);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinotaurPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Minotaur), "Start")]
        public static void Postfix(Minotaur __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMinotaur);
        }
    }


    // ------------------
    // ----- ANGELS -----
    // ------------------
    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceGabrielPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Gabriel), "Start")]
        public static void Postfix(Gabriel __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceGabriel);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceGagabrielPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GabrielSecond), "Start")]
        public static void Postfix(GabrielSecond __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceGagabriel);
        }
    }


    // -----------------
    // ----- OTHER -----
    // -----------------
    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceSomethingWickedPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Wicked), "Start")]
        public static void Postfix(Wicked __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceSomethingWicked);
        }
    }
    
    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceCancerousRodentPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CancerousRodent), "Start")]
        public static void Postfix(CancerousRodent __instance) {
            if (__instance.name.Contains("Very")) {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceVeryCancerousRodent);
                return;
            }
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceCancerousRodent);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMysteriousDruidKnightAndOwlPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mandalore), "Start")]
        public static void Postfix(Mandalore __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMysteriousDruidKnightAndOwl);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceFleshPrisonPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FleshPrison), "Start")]
        public static void Postfix(FleshPrison __instance, ref EnemyIdentifier ___eid) {
            switch (___eid.enemyType) {
                case EnemyType.FleshPrison:
                    Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceFleshPrison);
                    break;
                case EnemyType.FleshPanopticon:
                    Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceFleshPanopticon);
                    break;
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinosPrimePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MinosPrime), "Start")]
        public static void Postfix(MinosPrime __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMinosPrime);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceSisyphusPrimePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SisyphusPrime), "Start")]
        public static void Postfix(SisyphusPrime __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceSisyphusPrime);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoicePuppetPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Puppet), "Start")]
        public static void Postfix(Puppet __instance) {
            Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoicePuppet);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMiragePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SceneHelper), "OnSceneLoaded")]
        public static void Postfix(SceneHelper __instance) {
            if (SceneHelper.CurrentScene == "Level 2-S") {
                Plugin.bib.AddAndPlayVoice(__instance.gameObject, Bib.VoiceMirage);
            }
        }
    }
}