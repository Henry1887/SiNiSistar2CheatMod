using HarmonyLib;
using SiNiSistar2.Damage;
using SiNiSistar2Mod.CheatMenu;
using System.Reflection;

namespace SiNiSistar2Mod.Patches
{
    [HarmonyPatch]
    public static class DamageManagerPatch
    {
        static MethodBase TargetMethod()
        {
            return typeof(DamageManager)
                .GetMethods()
                .First(m => m.Name == "AddDamage" && m.GetParameters().Length == 12);
        }
        public static bool Prefix(
            ref DamageParameter inDamageParameter,
            DamageTargetType inDamageTargetType)
        {
            if (inDamageTargetType == DamageTargetType.Player)
            {
                // Block incoming damage entirely.
                return !CheatMenuEntryHandler.GetValue(CheatKeys.BlockAllDamage);
            }

            // Outgoing damage: upgrade to critical when Instant Kill is on.
            if (CheatMenuEntryHandler.GetValue(CheatKeys.AttackCheat))
                inDamageParameter = DamageParameter.CreateCriticalDamage();

            return true;
        }
    }
}
