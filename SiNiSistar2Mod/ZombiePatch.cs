using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using SiNiSistar2.Obj;
using UnityEngine;

namespace SiNiSistar2Mod
{
    [HarmonyPatch(typeof(EnemyObject))]
    public static class EnemyObjectPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        public static void AwakePostfix(EnemyObject __instance)
        {
            if (CheatMenuBehaviour.EnemyObjectList.Contains(__instance)) return;
            CheatMenuBehaviour.EnemyObjectList.Add(__instance);
            Plugin.Instance.Log.LogInfo($"EnemyObject Awake: {__instance.name}");
        }
    }
}
