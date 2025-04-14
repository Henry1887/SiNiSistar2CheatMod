using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using SiNiSistar2.Obj;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SiNiSistar2Mod
{
    [BepInPlugin("com.Henry1887.SiNiSistar2Mod", "SiNiSistar 2 Cheat Mod", "1.0.0")]
    public class Plugin : BasePlugin
    {
        internal static Plugin Instance { get; private set; }

        internal static PlayerStatusManager PlayerStatusManagerInstance { get; set; }

        private Harmony _harmony;

        internal static bool MaxHpEnabled = false;
        internal static bool MaxMpEnabled = false;
        internal static bool MenuVisible = true;
        internal static bool LockHP1Enabled = false;

        public override void Load()
        {
            Instance = this;

            Log.LogInfo("Loading Mod...");

            _harmony = new Harmony("com.Henry1887.SiNiSistar2Mod");
            _harmony.PatchAll();

            ClassInjector.RegisterTypeInIl2Cpp<CheatMenuBehaviour>();

            SceneManager.add_sceneLoaded((UnityEngine.Events.UnityAction<Scene, LoadSceneMode>)OnSceneLoaded);

            Log.LogInfo("Mod loaded successfully!");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Log.LogInfo($"Scene loaded: {scene.name}");

            if (GameObject.Find("CheatMenu") == null)
            {
                var obj = new GameObject("CheatMenu");
                obj.AddComponent<CheatMenuBehaviour>();
                UnityEngine.Object.DontDestroyOnLoad(obj);

                Log.LogInfo("CheatMenuBehaviour attached successfully!");
            }
        }

        public override bool Unload()
        {
            _harmony.UnpatchSelf();
            SceneManager.remove_sceneLoaded((UnityEngine.Events.UnityAction<Scene, LoadSceneMode>)OnSceneLoaded);
            Log.LogInfo("Mod unloaded!");
            return true;
        }
    }
}
