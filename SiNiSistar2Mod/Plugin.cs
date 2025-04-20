using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using SiNiSistar2.Obj;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SiNiSistar2Mod
{
    [BepInPlugin("com.Henry1887.SiNiSistar2Mod", "SiNiSistar 2 Cheat Mod", "1.0.5")]
    public class Plugin : BasePlugin
    {
        internal static Plugin Instance { get; private set; }

        internal static PlayerStatusManager PlayerStatusManagerInstance { get; set; }

        private Harmony _harmony;

        internal static bool MaxHpEnabled = false;
        internal static bool MaxMpEnabled = false;
        internal static bool MenuVisible = true;
        internal static bool LockHP1Enabled = false;
        internal static bool BlockBind = false;
        internal static bool BlockAllDamage = false;
        internal static bool AttackCheat = false;

        internal static Dictionary<string, string> AbnormalDataToAssetBundleDict = new Dictionary<string, string>()
        {
            { "Assimilation", "4f4dc7b245bd26399d19121733377" },
            { "Assimilation_Seed", "2c4fc2d07bf4a2c8cadd624873908" },
            { "BigPicture", "2fe80610ba441ba9beb66880dd4ce" },
            { "Blessing_Lost", "4f913ca62f57d257c2eb0ea57ae92" },
            { "Blinded", "d7f4ebb7dfc341fa52b4bb2dd1034" },
            { "BolaBind", "5cee6c27351f42dcc20f430f54cb1" },
            { "Book", "2550dbc9e14f8b3ebdba6f6604419" },
            { "Breast", "ec53e46e9bbd309a7c6538169811e" },
            { "BreastSuper", "acc81d4c719812bd14279e24a81c5" },
            { "Broken2", "535287de0ae1ed95523f99b77bfec" },
            { "ChangeZombie", "88c4e60ac79963cc90ef011adc407" },
            { "Clouding", "c0d7bec33e2b063a3721bd8eed9e7" },
            { "Defilement", "d7c5f3a018d5fd355286937751e25" },
            { "Down2", "b4a5550803dd4ffe3fbe06f573723" },
            { "Down2ForSewageBoss", "4bb0958de963a051ad76b2f413fed" },
            { "Drunk", "a45c1c9a118f87f2e2c98d0d87d51" },
            { "Entombed", "e3aee4e5cd50c5dad6048580e4866" },
            { "EvilWoodSeed", "9b96b3240bbc032fb80fe1867c906" },
            { "FallSleep", "04272c09709ce255e5b7fc6b28b94" },
            { "Feed", "6c5e6b89796c14569989269fc53dc" },
            { "FungalBed", "839d2e07069889c9882dda32fcec7" },
            { "GO_OuterOne", "fec218f430b54f57167f6e9eda3b5" },
            { "GO_Stomach", "4d6fefa5c321c2340368f36a13f39" },
            { "Handcuffed", "e5000e40a2e7f0016e48645d245ae" },
            { "HandRestraints", "990be8f8498cf8a5c1a242a6bcd7a" },
            { "Infertility", "7c592a2bdb4771c13f96452df489d" },
            { "InfertilityBlessing", "5cf4af6042b889f4a5998b94b317c" },
            { "LeechEgg", "536ada906d884e18f9e76f81754a8" },
            { "LeechEgg_Boss", "53dcf7c749f464ebaf91c6e8d7b46" },
            { "LeechInfestation", "250558681f378f0c980247525fdb9" },
            { "LeechInfestation_Hold", "6d7e81cf73bfe22a3c53a16d3c2f3" },
            { "LimbLoss", "418a6d619147612c188cf3fd5aa53" },
            { "LivestockParasite", "ad4c8254c7ed2b474ce12eda4e15c" },
            { "Livestock_Pig", "1ea403bf9fa77bb1e57bfc7660251" },
            { "Lustfull", "1dedea12f957d56079a0934ea157f" },
            { "Lustfull_Forever", "50e9dfe923d873b36aaad8a3b67d5" },
            { "LustMarkCurse", "095f5f708f32e1dd31e905440ebc3" },
            { "Mask", "31934e9170e193f40bdfcd192f69d" },
            { "MeatBud", "721b7c0874259480146eeb61cb45b" },
            { "MeatBuding", "6d9d3808d8f1153a10525d3cc0775" },
            { "MeatTentacleCluster1Bind", "9769bb154570f61ab07a70f3501b1" },
            { "Milk", "c666f8f2aef078589da3737a6b323" },
            { "MindControl", "b762d0b80c5124e64ca1942117823" },
            { "MindIntegration", "0e6d7a9aafb6ab6d29415ab199e84" },
            { "MotherBody", "73aa673fe70fa5ee776acb215b749" },
            { "Mucus", "40948db64656e480b49a7d4ebd808" },
            { "MucusSlug", "17619ffe51dbe4bd31dec0a2e183e" },
            { "Mutation", "99a9b8694a084237fa014ce9ab56b" },
            { "Parasite", "9b79b40dda5a81337d5b66b6d09df" },
            { "ParasiteLv13", "599bb50416d23f18f68401c0d6880" },
            { "ParasitePennis", "0d64bf9c58b52d354e00c9d8b22f8" },
            { "Poison", "b51e1eb83f32942b262cae6d8fc74" },
            { "Pregnant", "180ef7b64614c18f5aeed7fc36cf5" },
            { "Pregnant_Demi", "cb73705cbabe7a4083127ef846a6d" },
            { "SandBlinded", "fd6b68bfc15ff106e2bcede21870c" },
            { "Semen", "662ce50444d44c11d01102718876b" },
            { "Semen_mucus", "502052ab9b0cd3d58665fcf2e3a7c" },
            { "Slow", "b6ca5af18f73c55dc3dc3ecb162e3" },
            { "Spore", "75063032a0215a52c0ee62d37d530" },
            { "Stone", "183015a082901c28350b3c9974a70" },
            { "TentacleEgg", "5d15848506ffc4236180a97edc4be" },
            { "TentacleEgg_GO", "a649713629e1cc3feb2588011683e" },
            { "TreeDisorientation", "70436f8ede2013d119e5e9a264e83" },
            { "WetNurse", "a5a48464e8008be6362708ddcdb1a" },
        };

        internal static Dictionary<string, AssetBundle> cachedAssetBundle = new Dictionary<string, AssetBundle>();

        public override void Load()
        {
            Instance = this;

            Log.LogInfo("Loading Mod...");

            _harmony = new Harmony("com.Henry1887.SiNiSistar2Mod");
            _harmony.PatchAll();

            ClassInjector.RegisterTypeInIl2Cpp<CheatMenuBehaviour>();

            SceneManager.add_sceneLoaded((UnityEngine.Events.UnityAction<Scene, LoadSceneMode>)OnSceneLoaded);

            CheatMenuEntryHandler.LoadEntries();

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

        public static AssetBundle TryLoadBundle(string assetBundleName)
        {
            if (cachedAssetBundle.ContainsKey(assetBundleName))
            {
                return cachedAssetBundle[assetBundleName];
            }
            var assetBundlePath = System.IO.Path.Combine(Application.dataPath, "StreamingAssets", "game_bin", assetBundleName);
            if (System.IO.File.Exists(assetBundlePath))
            {
                var bytes = System.IO.File.ReadAllBytes(assetBundlePath);
                var decrypted_bytes = Util.OutDeCreateByte(bytes, bytes.Length);
                var assetBundle = AssetBundle.LoadFromMemory(decrypted_bytes);
                if (assetBundle != null)
                {
                    cachedAssetBundle[assetBundleName] = assetBundle;
                    return assetBundle;
                }
                else
                {
                    Instance.Log.LogError($"Failed to load AssetBundle: {assetBundleName}");
                }
            }
            else
            {
                Instance.Log.LogError($"AssetBundle not found: {assetBundlePath}");
            }
            return null;
        }

        public static AbnormalData TryToLoadAbnormalData(AbnormalType type)
        {
            Plugin.Instance.Log.LogInfo($"Trying to load AbnormalData: {type}");

            AssetBundle assetBundle = TryLoadBundle(AbnormalDataToAssetBundleDict[type.ToString()]);
            if (assetBundle != null)
            {
                var abnormalData = assetBundle.LoadAsset<AbnormalData>(type.ToString());
                if (abnormalData != null)
                {
                    return abnormalData;
                }
                else
                {
                    Instance.Log.LogError($"Failed to load AbnormalData: {type}");
                }
            }
            return null;
        }
    }
}
