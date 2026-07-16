using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using SiNiSistar2Mod.CheatMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SiNiSistar2Mod
{
    [BepInPlugin("com.Henry1887.SiNiSistar2Mod", "SiNiSistar 2 Cheat Mod", "1.0.8")]
    public class Plugin : BasePlugin
    {
        public static Plugin Instance { get; private set; }

        private Harmony _harmony;
        private GameObject _menuObject;

        public override void Load()
        {
            Instance = this;

            _harmony = new Harmony("com.Henry1887.SiNiSistar2Mod");
            _harmony.PatchAll();

            ClassInjector.RegisterTypeInIl2Cpp<CheatMenuBehaviour>();

            SceneManager.add_sceneLoaded((UnityEngine.Events.UnityAction<Scene, LoadSceneMode>)OnSceneLoaded);

            CheatMenuEntryHandler.LoadEntries();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_menuObject != null)
                return;

            _menuObject = new GameObject("CheatMenu");
            _menuObject.AddComponent<CheatMenuBehaviour>();
            UnityEngine.Object.DontDestroyOnLoad(_menuObject);
        }

        public override bool Unload()
        {
            _harmony.UnpatchSelf();
            SceneManager.remove_sceneLoaded((UnityEngine.Events.UnityAction<Scene, LoadSceneMode>)OnSceneLoaded);
            return true;
        }
    }
}
