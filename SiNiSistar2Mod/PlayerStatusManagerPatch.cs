using HarmonyLib;
using SiNiSistar2.Damage;
using SiNiSistar2.Obj;
using System.Reflection;

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

            if (CheatMenuEntryHandler.GetValue("MaxHP"))
            {
                __instance.HP.SetCurrentValue(__instance.HP.Max);
            }
            else if (CheatMenuEntryHandler.GetValue("LockHP1"))
            {
                __instance.HP.SetCurrentValue(1);
            }

            if (CheatMenuEntryHandler.GetValue("MaxMP"))
            {
                __instance.MP.SetCurrentValue(__instance.MP.Max);
            }
            return true;
        }
    }

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

    [HarmonyPatch]
    public static class DamageManager_AddDamage_Patch
    {
        static MethodBase TargetMethod()
        {
            return typeof(DamageManager)
                .GetMethods()
                .First(m => m.Name == "AddDamage" && m.GetParameters().Length == 12);
        }
        public static bool Prefix(
            IDamageReceiverOwner inReceiverObject,
            DamageTargetType inDamageTargetType)
        {
            if (DamageTargetType.Player == inDamageTargetType)
            {
                if (CheatMenuEntryHandler.GetValue("BlockAllDamage"))
                {
                    return false;
                }
            }
            else
            {
                if (CheatMenuEntryHandler.GetValue("AttackCheat"))
                {
                    inReceiverObject.HP.SetCurrentValue(0);
                }
            }

            return true;
        }
    }
}
