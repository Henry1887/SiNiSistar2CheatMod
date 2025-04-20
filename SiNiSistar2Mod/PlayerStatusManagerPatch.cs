using HarmonyLib;
using SiNiSistar2.Damage;
using SiNiSistar2.Obj;
using UnityEngine;

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

    [HarmonyPatch(typeof(Bind))]
    public static class BindPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("StartBind")]
        public static bool StartBindPrefix()
        {
            return !Plugin.BlockBind;
        }
    }

    [HarmonyPatch(typeof(DamageManager), "AddDamage", new[] {
        typeof(IDamageSenderOwner),
        typeof(IDamageReceiverOwner),
        typeof(int),
        typeof(int),
        typeof(DamageSenderCollider),
        typeof(DamageReceiverCollider),
        typeof(DamageParameter),
        typeof(int),
        typeof(float),
        typeof(Vector3),
        typeof(DamageTargetType),
        typeof(DamageSourceType)
    })]
    public static class DamageManager_AddDamage_Patch
    {
        public static bool Prefix(
            IDamageSenderOwner inSenderObject,
            IDamageReceiverOwner inReceiverObject,
            int inSenderID,
            int inReceiverID,
            DamageSenderCollider inSenderCollider,
            DamageReceiverCollider inReceiverCollider,
            ref DamageParameter inDamageParameter,
            int inHitCountMax,
            float inHitInterval,
            Vector3 inHitPosition,
            DamageTargetType inDamageTargetType,
            DamageSourceType damageSourceType)
        {
            if (DamageTargetType.Player == inDamageTargetType)
            {
                if (Plugin.BlockAllDamage)
                {
                    return false;
                }
            }
            else
            {
                if (Plugin.AttackCheat)
                {
                    inReceiverObject.HP.SetCurrentValue(0);
                }
            }

            return true;
        }
    }
}
