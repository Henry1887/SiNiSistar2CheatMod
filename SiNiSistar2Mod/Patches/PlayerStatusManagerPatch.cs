using HarmonyLib;
using SiNiSistar2.Obj;
using SiNiSistar2Mod.CheatMenu;

namespace SiNiSistar2Mod.Patches
{
    [HarmonyPatch(typeof(PlayerStatusManager))]
    public static class PlayerStatusManagerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static bool UpdatePrefix(PlayerStatusManager __instance)
        {
            if (CheatMenuEntryHandler.GetValue(CheatKeys.MaxHP))
                __instance.HP.SetCurrentValue(__instance.HP.Max);
            else if (CheatMenuEntryHandler.GetValue(CheatKeys.LockHP1))
                __instance.HP.SetCurrentValue(1);

            if (CheatMenuEntryHandler.GetValue(CheatKeys.MaxMP))
                __instance.MP.SetCurrentValue(__instance.MP.Max);

            return true;
        }
    }
}
