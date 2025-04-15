using HarmonyLib;
using SiNiSistar2;
using UnityEngine;

namespace SiNiSistar2Mod
{
    // Force debug options and features to load
    [HarmonyPatch(typeof(Debug))]
    public static class DebugPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("get_isDebugBuild")]
        public static bool isDebugBuildPrefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(ProjectSetting))]
    public static class ProjectSettingPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("get_IsDebugBuild")]
        public static bool IsDebugBuildPrefix(ref bool __result)
        {
            // Force the result to true, simulating a debug build
            __result = true;
            return false;
        }
    }
}
