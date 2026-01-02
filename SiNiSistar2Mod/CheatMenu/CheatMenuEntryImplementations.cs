using SiNiSistar2.Manager;
using SiNiSistar2.Obj;
using UnityEngine.InputSystem;

namespace SiNiSistar2Mod.CheatMenu
{
    public class HideEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return "F1: Hide Menu";
        }
        public void KeybindBehaviour()
        {
            CheatMenuEntryHandler.SetValue("IsVisible", !CheatMenuEntryHandler.GetValue("IsVisible", true));
        }
        public bool IsKeybindTriggered { get { return Keyboard.current.f1Key.wasPressedThisFrame; } }
    }
    public class MaxHPEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"F2: Max HP - {(CheatMenuEntryHandler.GetValue("MaxHP") ? "Enabled" : "Disabled")}";
        }
        public void KeybindBehaviour()
        {
            CheatMenuEntryHandler.SetValue("MaxHP", !CheatMenuEntryHandler.GetValue("MaxHP"));

            if (CheatMenuEntryHandler.GetValue("LockHP1")) CheatMenuEntryHandler.SetValue("LockHP1", false);
        }
        public bool IsKeybindTriggered { get { return Keyboard.current.f2Key.wasPressedThisFrame; } }
    }
    public class MaxMPEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"F3: Max MP - {(CheatMenuEntryHandler.GetValue("MaxMP") ? "Enabled" : "Disabled")}";
        }
        public void KeybindBehaviour()
        {
            CheatMenuEntryHandler.SetValue("MaxMP", !CheatMenuEntryHandler.GetValue("MaxMP"));
        }
        public bool IsKeybindTriggered { get { return Keyboard.current.f3Key.wasPressedThisFrame; } }
    }
    public class AddRelicsEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return "F4: Add 1000 Relics";
        }
        public void KeybindBehaviour()
        {
            ManagerList.PlayerStatus.AddRelics(1000, false);
        }
        public bool IsKeybindTriggered { get { return Keyboard.current.f4Key.wasPressedThisFrame; } }
    }

    public class LockHP1Entry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"F5: Lock HP to 1 - {(CheatMenuEntryHandler.GetValue("LockHP1") ? "Enabled" : "Disabled")} (May still cause a Game Over)";
        }
        public void KeybindBehaviour()
        {
            CheatMenuEntryHandler.SetValue("LockHP1", !CheatMenuEntryHandler.GetValue("LockHP1"));
            if (CheatMenuEntryHandler.GetValue("MaxHP")) CheatMenuEntryHandler.SetValue("MaxHP", false);
        }
        public bool IsKeybindTriggered { get { return Keyboard.current.f5Key.wasPressedThisFrame; } }
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
            if (Keyboard.current.f6Key.wasPressedThisFrame)
            {
                ItemID itemId = (ItemID)itemEnumValues.GetValue(selectedItemIndex);
                ManagerList.PlayerStatus.InventoryHandler.AddItem(itemId, 1);
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
        public bool IsKeybindTriggered
        {
            get
            {
                return
                    Keyboard.current.f6Key.wasPressedThisFrame ||
                    Keyboard.current.f7Key.wasPressedThisFrame ||
                    Keyboard.current.f8Key.wasPressedThisFrame;
            }
        }
    }

    public class AbnormalEntry : ICheatMenuEntry
    {
        private int selectedAbnormalIndex = 1;
        private readonly Array abnormalEnumValues = Enum.GetValues(typeof(AbnormalType));
        public string GetDrawText()
        {
            AbnormalType selectedType = (AbnormalType)abnormalEnumValues.GetValue(selectedAbnormalIndex);
            bool hasState;
            // May throw
            try
            {
                hasState = ManagerList.PlayerStatus != null ? ManagerList.PlayerStatus.AbnormalList.Has(selectedType) : false;
            }
            catch
            {
                hasState = false;
            }
            return $"F9: ({selectedType}) {(hasState ? "Enabled" : "Disabled")} - F10 Scroll Down - F11 Scroll Up (May break your game)";
        }
        public void KeybindBehaviour()
        {
            if (Keyboard.current.f9Key.wasPressedThisFrame)
            {
                AbnormalType selectedType = (AbnormalType)abnormalEnumValues.GetValue(selectedAbnormalIndex);
                if (ManagerList.PlayerStatus.AbnormalList.Has(selectedType))
                {
                    ManagerList.PlayerStatus.AbnormalList.RemoveAbnormal(selectedType);
                }
                else
                {
                    ManagerList.PlayerStatus.AbnormalList.AddOrRemoveAbnormal(selectedType, true);
                    // Check if the game managed to add it normally
                    if (!ManagerList.PlayerStatus.AbnormalList.Has(selectedType))
                    {
                        // Fuck the game, forcefully add it!
                        AbnormalData data;
                        if (!AbnormalDataHandler.TryLoadAbnormalData(selectedType, out data))
                        {
                            Plugin.Instance.Log.LogError($"Failed to add {selectedType} to the Abnormal List");
                        }
                        else
                        {
                            ManagerList.PlayerStatus.AbnormalList.AddAbnormal(data);
                            Plugin.Instance.Log.LogInfo($"Forcefully Added {selectedType} to the Abnormal List");
                            Plugin.Instance.Log.LogWarning("Forcefully adding a status will likely result in bugging out your game or it will outright not work.");
                        }
                    }
                }
                Plugin.Instance.Log.LogInfo($"Toggled {selectedType}");
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
        public bool IsKeybindTriggered
        {
            get
            {
                return
                    Keyboard.current.f9Key.wasPressedThisFrame ||
                    Keyboard.current.f10Key.wasPressedThisFrame ||
                    Keyboard.current.f11Key.wasPressedThisFrame;
            }
        }
    }

    public class BlockBindEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"1: Block Bind - {(CheatMenuEntryHandler.GetValue("BlockBind") ? "Enabled" : "Disabled")}";
        }
        public void KeybindBehaviour()
        {
            CheatMenuEntryHandler.SetValue("BlockBind", !CheatMenuEntryHandler.GetValue("BlockBind"));
        }
        public bool IsKeybindTriggered { get { return Keyboard.current.digit1Key.wasPressedThisFrame; } }
    }

    public class ToggleClothingEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return "2: Toggle Clothing State";
        }
        public void KeybindBehaviour()
        {
            if (ManagerList.PlayerStatus.Durability.Current != 0)
            {
                ManagerList.PlayerStatus.Durability.SetCurrentValue(0);
            }
            else
            {
                ManagerList.PlayerStatus.Durability.SetCurrentValue(ManagerList.PlayerStatus.Durability.Max);
            }
        }
        public bool IsKeybindTriggered { get { return Keyboard.current.digit2Key.wasPressedThisFrame; } }
    }

    public class AttackEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"3: Instant Kill - {(CheatMenuEntryHandler.GetValue("AttackCheat") ? "Enabled" : "Disabled")}";
        }
        public void KeybindBehaviour()
        {
            CheatMenuEntryHandler.SetValue("AttackCheat", !CheatMenuEntryHandler.GetValue("AttackCheat"));
        }
        public bool IsKeybindTriggered { get { return Keyboard.current.digit3Key.wasPressedThisFrame; } }
    }

    public class ReleaseBindEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"4: Release current Bind";
        }
        public void KeybindBehaviour()
        {
            ManagerList.Object.Lelia.Bind.ReleaseBind();
        }
        public bool IsKeybindTriggered { get { return Keyboard.current.digit4Key.wasPressedThisFrame; } }
    }

    public class BlockAllDamageEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"5: Block All Damage - {(CheatMenuEntryHandler.GetValue("BlockAllDamage") ? "Enabled" : "Disabled")}";
        }
        public void KeybindBehaviour()
        {
            CheatMenuEntryHandler.SetValue("BlockAllDamage", !CheatMenuEntryHandler.GetValue("BlockAllDamage"));
        }
        public bool IsKeybindTriggered { get { return Keyboard.current.digit5Key.wasPressedThisFrame; } }
    }

    public class ShowEnemyHealthEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"6: Show Enemy Health - {(CheatMenuEntryHandler.GetValue("ShowEnemyHP") ? "Enabled" : "Disabled")}";
        }
        public void KeybindBehaviour()
        {
            CheatMenuEntryHandler.SetValue("ShowEnemyHP", !CheatMenuEntryHandler.GetValue("ShowEnemyHP"));
        }
        public bool IsKeybindTriggered { get { return Keyboard.current.digit6Key.wasPressedThisFrame; } }
    }

    public class KillAllEnemiesEntry : ICheatMenuEntry
    {
        public string GetDrawText()
        {
            return $"7: Kill All Enemies in current Level";
        }
        public void KeybindBehaviour()
        {
            foreach (EnemyObject enemy in CheatMenuBehaviour.EnemyObjectList)
            {
                if (enemy == null || enemy.DeadState != EnemyDead.State.Alive || enemy.HP == null) continue;
                enemy.HP.SetCurrentValue(0);
            }
        }
        public bool IsKeybindTriggered { get { return Keyboard.current.digit7Key.wasPressedThisFrame; } }
    }
}