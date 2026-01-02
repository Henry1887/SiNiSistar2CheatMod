using SiNiSistar2;
using SiNiSistar2.Obj;
using UnityEngine;

namespace SiNiSistar2Mod
{
    public class AbnormalDataHandler
    {
        public static Dictionary<string, AssetBundle> cachedAssetBundle = new Dictionary<string, AssetBundle>();

        public static bool TryLoadBundle(string assetBundleName, out AssetBundle bundle)
        {
            bundle = null;
            if (cachedAssetBundle.ContainsKey(assetBundleName))
            {
                bundle = cachedAssetBundle[assetBundleName];
                return true;
            }
            var assetBundlePath = Path.Combine(Application.dataPath, "StreamingAssets", "game_bin", assetBundleName);
            if (File.Exists(assetBundlePath))
            {
                var bytes = File.ReadAllBytes(assetBundlePath);
                var decrypted_bytes = Util.OutDeCreateByte(bytes, bytes.Length);
                var assetBundle = AssetBundle.LoadFromMemory(decrypted_bytes);
                if (assetBundle != null)
                {
                    cachedAssetBundle[assetBundleName] = assetBundle;
                    bundle = assetBundle;
                    return true;
                }
                else
                {
                    Plugin.Instance.Log.LogError($"Failed to load AssetBundle: {assetBundleName}");
                }
            }
            else
            {
                Plugin.Instance.Log.LogError($"AssetBundle not found: {assetBundlePath}");
            }
            return false;
        }

        public static bool TryLoadAbnormalData(AbnormalType type, out AbnormalData abnormData)
        {
            abnormData = null;

            string assetBundleName;

            if (!AssetBundleLoader.Instance.m_AssetBundleNameTable.TryGetAssetBundleNameFromVirtualPath(string.Format("ScriptableObject/Abnormal/{0}.asset", type.ToString()), out assetBundleName))
            {
                Plugin.Instance.Log.LogError($"AssetBundle name not found for AbnormalData: {type}");
                return false;
            }

            AssetBundle assetBundle;
            if (!TryLoadBundle(assetBundleName, out assetBundle))
            {
                Plugin.Instance.Log.LogError($"Failed to load AssetBundle for AbnormalData: {type}");
                return false;
            }

            AbnormalData abnormalData = assetBundle.LoadAsset<AbnormalData>(type.ToString());
            if (abnormalData != null)
            {
                abnormData = abnormalData;
                return true;
            }
            else
            {
                Plugin.Instance.Log.LogError($"Failed to load AbnormalData: {type}");
            }
            return false;
        }
    }
}
