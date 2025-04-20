using SiNiSistar2.Manager;
using SiNiSistar2.Obj;
using UnityEngine.InputSystem;

namespace SiNiSistar2Mod
{
    public class HideEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return "F1: Hide Menu";
        }
        public void KeybindBehaviour()
        {
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                Plugin.MenuVisible = !Plugin.MenuVisible;
                Plugin.Instance.Log.LogInfo($"Menu Visibility: {Plugin.MenuVisible}");
            }
        }
    }
    public class MaxHPEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"F2: Max HP - {(Plugin.MaxHpEnabled ? "Enabled" : "Disabled")}";
        }
        public void KeybindBehaviour()
        {
            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                Plugin.MaxHpEnabled = !Plugin.MaxHpEnabled;
                if (Plugin.LockHP1Enabled) Plugin.LockHP1Enabled = false;
                Plugin.Instance.Log.LogInfo($"Max HP Cheat: {Plugin.MaxHpEnabled}");
            }
        }
    }
    public class MaxMPEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"F3: Max MP - {(Plugin.MaxMpEnabled ? "Enabled" : "Disabled")}";
        }
        public void KeybindBehaviour()
        {
            if (Keyboard.current.f3Key.wasPressedThisFrame)
            {
                Plugin.MaxMpEnabled = !Plugin.MaxMpEnabled;
                Plugin.Instance.Log.LogInfo($"Max MP Cheat: {Plugin.MaxMpEnabled}");
            }
        }
    }
    public class AddRelicsEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return "F4: Add 1000 Relics";
        }
        public void KeybindBehaviour()
        {
            if (Keyboard.current.f4Key.wasPressedThisFrame && Plugin.PlayerStatusManagerInstance != null)
            {
                Plugin.PlayerStatusManagerInstance.AddRelics(1000, false);
                Plugin.Instance.Log.LogInfo($"Added 1000 Relics.");
            }
        }
    }

    public class LockHP1Entry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"F5: Lock HP to 1 - {(Plugin.LockHP1Enabled ? "Enabled" : "Disabled")} (May still cause a Game Over)";
        }
        public void KeybindBehaviour()
        {
            if (Keyboard.current.f5Key.wasPressedThisFrame)
            {
                Plugin.LockHP1Enabled = !Plugin.LockHP1Enabled;
                if (Plugin.MaxHpEnabled) Plugin.MaxHpEnabled = false;
                Plugin.Instance.Log.LogInfo($"Lock HP to 1 Cheat: {Plugin.LockHP1Enabled}");
            }
        }
    }

    public class AddItemEntry : ICheatMenuEntry
    {
        private int selectedItemIndex = 0;
        private readonly Array itemEnumValues = Enum.GetValues(typeof(ItemID));
        public string GetDrawText()
        {
            return $"F6: Add 1 ({(ItemID)itemEnumValues.GetValue(selectedItemIndex)}) - F7 Scroll Down - F8 Scroll Up";
        }
        public void KeybindBehaviour()
        {
            if (Keyboard.current.f6Key.wasPressedThisFrame && Plugin.PlayerStatusManagerInstance != null)
            {
                ItemID itemId = (ItemID)itemEnumValues.GetValue(selectedItemIndex);
                Plugin.PlayerStatusManagerInstance.InventoryHandler.AddItem(itemId, 1);
                Plugin.Instance.Log.LogInfo($"Added 1 {itemId}");
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
        }
    }

    public class AbnormalEntry : ICheatMenuEntry
    {
        private int selectedAbnormalIndex = 1;
        private readonly Array abnormalEnumValues = Enum.GetValues(typeof(AbnormalType));
        public string GetDrawText()
        {
            bool hasState = false;
            if (Plugin.PlayerStatusManagerInstance != null)
            {
                AbnormalType selectedType = ((AbnormalType[])Enum.GetValues(typeof(AbnormalType)))[selectedAbnormalIndex];
                hasState = Plugin.PlayerStatusManagerInstance.AbnormalList.Has(selectedType);
            }
            return $"F9: ({(AbnormalType)abnormalEnumValues.GetValue(selectedAbnormalIndex)}) {(hasState ? "Enabled" : "Disabled")} - F10 Scroll Down - F11 Scroll Up (May break your game)";
        }
        public void KeybindBehaviour()
        {
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
        }
    }

    public class SceneSelectEntry : ICheatMenuEntry
    {
        private bool SceneSelectUIOpen = false;
        public string GetDrawText()
        {
            return $"F12: Scene Select UI - {(SceneSelectUIOpen ? "Enabled" : "Disabled")}";
        }
        public void KeybindBehaviour()
        {
            if (Keyboard.current.f12Key.wasPressedThisFrame)
            {
                SceneSelectUIOpen = !SceneSelectUIOpen;
                ManagerList.Debugger.DebugSceneSelectUI.gameObject.SetActive(SceneSelectUIOpen);
                ManagerList.Debugger.DebugSceneSelectUI.enabled = SceneSelectUIOpen;
            }
        }
    }

    public class BlockBindEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"1: Block Bind - {(Plugin.BlockBind ? "Enabled" : "Disabled")}";
        }
        public void KeybindBehaviour()
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                Plugin.BlockBind = !Plugin.BlockBind;
                Plugin.Instance.Log.LogInfo($"Block Bind: {Plugin.BlockBind}");
            }
        }
    }

    public class ToggleClothingEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return "2: Toggle Clothing State";
        }
        public void KeybindBehaviour()
        {
            if (Keyboard.current.digit2Key.wasPressedThisFrame && Plugin.PlayerStatusManagerInstance != null)
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
        }
    }

    public class AttackEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"3: Instant Kill - {(Plugin.AttackCheat ? "Enabled" : "Disabled")}";
        }
        public void KeybindBehaviour()
        {
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                Plugin.AttackCheat = !Plugin.AttackCheat;
                Plugin.Instance.Log.LogInfo($"Instant Kill Toggled");
            }
        }
    }

    public class ReleaseBindEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"4: Release current Bind";
        }
        public void KeybindBehaviour()
        {
            if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                ManagerList.Object.Lelia.Bind.ReleaseBind();
                Plugin.Instance.Log.LogInfo($"Released Current Bind");
            }
        }
    }

    public class BlockAllDamageEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"5: Block All Damage - {(Plugin.BlockAllDamage ? "Enabled" : "Disabled")}";
        }
        public void KeybindBehaviour()
        {
            if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                Plugin.BlockAllDamage = !Plugin.BlockAllDamage;
                Plugin.Instance.Log.LogInfo($"Toggled Block All Damage");
            }
        }
    }
}