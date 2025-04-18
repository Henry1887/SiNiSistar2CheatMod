﻿using SiNiSistar2.Obj;
using UnityEngine.InputSystem;
using UnityEngine;
using SiNiSistar2.Manager;

namespace SiNiSistar2Mod
{
    public class CheatMenuBehaviour : MonoBehaviour
    {
        public CheatMenuBehaviour(System.IntPtr ptr) : base(ptr) { }

        private int selectedItemIndex = 0;
        private Array itemEnumValues;

        private int selectedAbnormalIndex = 1;
        private Array abnormalEnumValues;

        private bool SceneSelectUIOpen = false;

        private int previousAttackLvl = -1;

        private void Start()
        {
            itemEnumValues = Enum.GetValues(typeof(ItemID));
            abnormalEnumValues = Enum.GetValues(typeof(AbnormalType));
            Plugin.Instance.Log.LogInfo("CheatMenuBehaviour started!");
        }

        private void Update()
        {
            // TODO: Fix
            if (Keyboard.current.leftAltKey.wasPressedThisFrame && Plugin.PlayerStatusManagerInstance != null)
            {
                if (previousAttackLvl == -1)
                {
                    previousAttackLvl = Plugin.PlayerStatusManagerInstance.AttackLv;
                    Plugin.PlayerStatusManagerInstance.AttackLv = 100;
                    Plugin.Instance.Log.LogInfo($"Attack Level set to 100");
                }
                else
                {
                    Plugin.PlayerStatusManagerInstance.AttackLv = previousAttackLvl;
                    previousAttackLvl = -1;
                    Plugin.Instance.Log.LogInfo($"Attack Level reset to {Plugin.PlayerStatusManagerInstance.AttackLv}");
                }
            }

            if (Keyboard.current.wKey.wasPressedThisFrame && Plugin.PlayerStatusManagerInstance != null)
            {
                if (Plugin.PlayerStatusManagerInstance.Durability.Current != 0)
                {
                    Plugin.PlayerStatusManagerInstance.Durability.SetCurrentValue(0);
                    Plugin.Instance.Log.LogInfo($"Durability set to 0");
                }
                else
                {
                    Plugin.PlayerStatusManagerInstance.Durability.SetCurrentValue(Plugin.PlayerStatusManagerInstance.Durability.Max);
                    Plugin.Instance.Log.LogInfo($"Durability set to Max");
                }
            }
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                Plugin.MenuVisible = !Plugin.MenuVisible;
                Plugin.Instance.Log.LogInfo($"Menu visibility: {Plugin.MenuVisible}");
            }

            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                Plugin.MaxHpEnabled = !Plugin.MaxHpEnabled;
                if (Plugin.LockHP1Enabled) Plugin.LockHP1Enabled = false;
                Plugin.Instance.Log.LogInfo($"Max HP Cheat: {Plugin.MaxHpEnabled}");
            }

            if (Keyboard.current.f3Key.wasPressedThisFrame)
            {
                Plugin.MaxMpEnabled = !Plugin.MaxMpEnabled;
                Plugin.Instance.Log.LogInfo($"Max MP Cheat: {Plugin.MaxMpEnabled}");
            }

            if (Keyboard.current.f4Key.wasPressedThisFrame && Plugin.PlayerStatusManagerInstance != null)
            {
                Plugin.PlayerStatusManagerInstance.AddRelics(1000, false);
                Plugin.Instance.Log.LogInfo($"Added 1000 Relics.");
            }

            if (Keyboard.current.f5Key.wasPressedThisFrame)
            {
                Plugin.LockHP1Enabled = !Plugin.LockHP1Enabled;
                if (Plugin.MaxHpEnabled) Plugin.MaxHpEnabled = false;
                Plugin.Instance.Log.LogInfo($"Max HP Cheat: {Plugin.LockHP1Enabled}");
            }

            if (Keyboard.current.f6Key.wasPressedThisFrame && Plugin.PlayerStatusManagerInstance != null)
            {
                Plugin.PlayerStatusManagerInstance.InventoryHandler.AddItem((ItemID)selectedItemIndex, 1);
                Plugin.Instance.Log.LogInfo($"Added 1 {itemEnumValues.GetValue(selectedItemIndex)} to the Inventory.");
            }

            if (Keyboard.current.f7Key.wasPressedThisFrame)
            {
                selectedItemIndex--;
                if (selectedItemIndex < 0) selectedItemIndex = itemEnumValues.Length - 2;
            }

            if (Keyboard.current.f8Key.wasPressedThisFrame)
            {
                selectedItemIndex++;
                if (selectedItemIndex > itemEnumValues.Length - 2) selectedItemIndex = 0;
            }

            if (Keyboard.current.f9Key.wasPressedThisFrame && Plugin.PlayerStatusManagerInstance != null)
            {
                AbnormalType selectedType = ((AbnormalType[])Enum.GetValues(typeof(AbnormalType)))[selectedAbnormalIndex];
                if (Plugin.PlayerStatusManagerInstance.AbnormalList.Has(selectedType))
                {
                    Plugin.PlayerStatusManagerInstance.AbnormalList.RemoveAbnormal(selectedType);
                }
                else
                {
                    Plugin.PlayerStatusManagerInstance.AbnormalList.AddOrRemoveAbnormal(selectedType, true);
                    // Check if the game managed to add it normally
                    if (!Plugin.PlayerStatusManagerInstance.AbnormalList.Has(selectedType))
                    {
                        // Fuck the game, forcefully add it!
                        AbnormalData data = Plugin.TryToLoadAbnormalData(selectedType);
                        if (data != null)
                        {
                            Plugin.PlayerStatusManagerInstance.AbnormalList.AddAbnormal(data);
                            Plugin.Instance.Log.LogInfo($"Forcefully Added {selectedType} to the Abnormal List");
                            Plugin.Instance.Log.LogWarning("Forcefully adding a status will likely result in bugging out your game or it will outright not work.");
                        }
                        else
                        {
                            Plugin.Instance.Log.LogError($"Failed to add {selectedType} to the Abnormal List");
                        }
                    }
                }
                Plugin.Instance.Log.LogInfo($"Toggled {abnormalEnumValues.GetValue(selectedAbnormalIndex)}");
            }

            if (Keyboard.current.f10Key.wasPressedThisFrame)
            {
                selectedAbnormalIndex--;
                if (selectedAbnormalIndex < 1) selectedAbnormalIndex = abnormalEnumValues.Length - 1;
            }

            if (Keyboard.current.f11Key.wasPressedThisFrame)
            {
                selectedAbnormalIndex++;
                if (selectedAbnormalIndex > abnormalEnumValues.Length - 1) selectedAbnormalIndex = 1;
            }

            if (Keyboard.current.f12Key.wasPressedThisFrame)
            {
                SceneSelectUIOpen = !SceneSelectUIOpen;
                ManagerList.Debugger.DebugSceneSelectUI.gameObject.SetActive(SceneSelectUIOpen);
                ManagerList.Debugger.DebugSceneSelectUI.enabled = SceneSelectUIOpen;
            }
        }

        private void OnGUI()
        {
            if (!Plugin.MenuVisible)
                return;

            GUI.Label(new Rect(10, 10, 300, 20), $"F1: Hide Menu");
            GUI.Label(new Rect(10, 30, 300, 20), $"F2: Max HP - {(Plugin.MaxHpEnabled ? "Enabled" : "Disabled")}");
            GUI.Label(new Rect(10, 50, 300, 20), $"F3: Max MP - {(Plugin.MaxMpEnabled ? "Enabled" : "Disabled")}");
            GUI.Label(new Rect(10, 70, 300, 20), $"F4: Add 1000 Relics");
            GUI.Label(new Rect(10, 90, 400, 20), $"F5: Lock HP to 1 {(Plugin.LockHP1Enabled ? "Enabled" : "Disabled")} (May still cause a Game Over)");
            GUI.Label(new Rect(10, 110, 500, 20), $"F6: Add 1 ({(ItemID)itemEnumValues.GetValue(selectedItemIndex)}) - F7 Scroll Down - F8 Scroll Up");
            bool hasState = false;
            if (Plugin.PlayerStatusManagerInstance != null)
            {
                AbnormalType selectedType = ((AbnormalType[])Enum.GetValues(typeof(AbnormalType)))[selectedAbnormalIndex];
                hasState = Plugin.PlayerStatusManagerInstance.AbnormalList.Has(selectedType);
            }
            GUI.Label(new Rect(10, 130, 500, 20), $"F9: ({(AbnormalType)abnormalEnumValues.GetValue(selectedAbnormalIndex)}) {(hasState ? "Enabled" : "Disabled")} - F10 Scroll Down - F11 Scroll Up");
            GUI.Label(new Rect(10, 150, 500, 20), $"Note: Some Abnormal Statuses will not work.");
            GUI.Label(new Rect(10, 170, 500, 20), $"F12: Toggle Scene Select UI - {(SceneSelectUIOpen ? "Enabled" : "Disabled")}");
            GUI.Label(new Rect(10, 190, 500, 20), $"LAlt: Toggle Level 100 Attack - {(previousAttackLvl != -1 ? "Enabled" : "Disabled")} (Broken)");
            GUI.Label(new Rect(10, 210, 500, 20), $"W: Toggle Clothing State");
        }
    }
}
