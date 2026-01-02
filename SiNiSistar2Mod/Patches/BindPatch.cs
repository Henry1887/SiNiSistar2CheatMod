using HarmonyLib;
using SiNiSistar2.Obj;
using SiNiSistar2Mod.CheatMenu;

namespace SiNiSistar2Mod.Patches
{
    [HarmonyPatch(typeof(Bind))]
    public static class BindPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("StartBind")]
        public static bool StartBindPrefix()
        {
            return !CheatMenuEntryHandler.GetValue("BlockBind");
        }
    }
}
