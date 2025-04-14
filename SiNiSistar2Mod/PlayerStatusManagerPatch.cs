using HarmonyLib;
using SiNiSistar2.Obj;

namespace SiNiSistar2Mod
{
    [HarmonyPatch(typeof(PlayerStatusManager))]
    public static class PlayerStatusManagerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static bool UpdatePrefix(PlayerStatusManager __instance)
        {
            if (Plugin.PlayerStatusManagerInstance == null)
            {
                Plugin.PlayerStatusManagerInstance = __instance;
            }

            if (Plugin.MaxHpEnabled)
            {
                __instance.HP.SetCurrentValue(__instance.HP.Max);
            }
            else if (Plugin.LockHP1Enabled)
            {
                __instance.HP.SetCurrentValue(1);
            }

            if (Plugin.MaxMpEnabled)
            {
                __instance.MP.SetCurrentValue(__instance.MP.Max);
            }
            return true;
        }
    }
}
