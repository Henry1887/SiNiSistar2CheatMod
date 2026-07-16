using SiNiSistar2;
using SiNiSistar2.Obj;
using UnityEngine;

namespace SiNiSistar2Mod
{
    public static class AbnormalDataHandler
    {
        private static readonly Dictionary<string, AssetBundle> CachedAssetBundles = new();

        public static bool TryLoadBundle(string assetBundleName, out AssetBundle bundle)
        {
            if (CachedAssetBundles.TryGetValue(assetBundleName, out bundle))
                return true;

            string assetBundlePath = Path.Combine(Application.dataPath, "StreamingAssets", "game_bin", assetBundleName);
            if (!File.Exists(assetBundlePath))
            {
                Plugin.Instance.Log.LogError($"AssetBundle not found: {assetBundlePath}");
                return false;
            }

            byte[] bytes = File.ReadAllBytes(assetBundlePath);
            byte[] decryptedBytes = Util.OutDeCreateByte(bytes, bytes.Length);
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(decryptedBytes);
            if (assetBundle == null)
            {
                Plugin.Instance.Log.LogError($"Failed to load AssetBundle: {assetBundleName}");
                return false;
            }

            CachedAssetBundles[assetBundleName] = assetBundle;
            bundle = assetBundle;
            return true;
        }

        public static bool TryLoadAbnormalData(AbnormalType type, out AbnormalData abnormalData)
        {
            abnormalData = null;

            if (!AssetBundleLoader.Instance.m_AssetBundleNameTable.TryGetAssetBundleNameFromVirtualPath(
                    $"ScriptableObject/Abnormal/{type}.asset", out string assetBundleName))
            {
                Plugin.Instance.Log.LogError($"AssetBundle name not found for AbnormalData: {type}");
                return false;
            }

            if (!TryLoadBundle(assetBundleName, out AssetBundle assetBundle))
            {
                Plugin.Instance.Log.LogError($"Failed to load AssetBundle for AbnormalData: {type}");
                return false;
            }

            AbnormalData loaded = assetBundle.LoadAsset<AbnormalData>(type.ToString());
            if (loaded == null)
            {
                Plugin.Instance.Log.LogError($"Failed to load AbnormalData: {type}");
                return false;
            }

            abnormalData = loaded;
            return true;
        }
    }
}
