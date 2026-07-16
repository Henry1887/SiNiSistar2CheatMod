using SiNiSistar2.Manager;
using SiNiSistar2.Obj;
using UnityEngine.InputSystem;

namespace SiNiSistar2Mod.CheatMenu
{
    public class HideEntry : ICheatMenuEntry
    {
        public string GetDrawText() => "F1: Hide Menu";

        public void KeybindBehaviour()
            => CheatMenuEntryHandler.SetValue(CheatKeys.IsVisible, !CheatMenuEntryHandler.GetValue(CheatKeys.IsVisible, true));

        public bool IsKeybindTriggered => Keyboard.current.f1Key.wasPressedThisFrame;
    }

    public class MaxHPEntry : ICheatMenuEntry
    {
        public string GetDrawText()
            => CheatMenuEntryHandler.FormatToggle("F2: Max HP", CheatMenuEntryHandler.GetValue(CheatKeys.MaxHP));

        public void KeybindBehaviour()
        {
            CheatMenuEntryHandler.ToggleValue(CheatKeys.MaxHP);
            // Max HP and Lock HP=1 are mutually exclusive.
            if (CheatMenuEntryHandler.GetValue(CheatKeys.LockHP1))
                CheatMenuEntryHandler.SetValue(CheatKeys.LockHP1, false);
        }

        public bool IsKeybindTriggered => Keyboard.current.f2Key.wasPressedThisFrame;
    }

    public class MaxMPEntry : ICheatMenuEntry
    {
        public string GetDrawText()
            => CheatMenuEntryHandler.FormatToggle("F3: Max MP", CheatMenuEntryHandler.GetValue(CheatKeys.MaxMP));

        public void KeybindBehaviour() => CheatMenuEntryHandler.ToggleValue(CheatKeys.MaxMP);

        public bool IsKeybindTriggered => Keyboard.current.f3Key.wasPressedThisFrame;
    }

    public class AddRelicsEntry : ICheatMenuEntry
    {
        public string GetDrawText() => "F4: Add 1000 Relics";

        public void KeybindBehaviour() => ManagerList.PlayerStatus.AddRelics(1000, false);

        public bool IsKeybindTriggered => Keyboard.current.f4Key.wasPressedThisFrame;
    }

    public class LockHP1Entry : ICheatMenuEntry
    {
        public string GetDrawText()
            => CheatMenuEntryHandler.FormatToggle("F5: Lock HP to 1", CheatMenuEntryHandler.GetValue(CheatKeys.LockHP1))
               + " (May still cause a Game Over)";

        public void KeybindBehaviour()
        {
            CheatMenuEntryHandler.ToggleValue(CheatKeys.LockHP1);
            // Mutually exclusive with Max HP.
            if (CheatMenuEntryHandler.GetValue(CheatKeys.MaxHP))
                CheatMenuEntryHandler.SetValue(CheatKeys.MaxHP, false);
        }

        public bool IsKeybindTriggered => Keyboard.current.f5Key.wasPressedThisFrame;
    }

    public class AddItemEntry : ICheatMenuEntry
    {
        private int selectedItemIndex = 0;
        // The trailing enum value is a sentinel (e.g. Count/None), so the usable
        // range is [0, Length-2). This is why the wraparound math below stops short.
        private readonly Array itemEnumValues = Enum.GetValues(typeof(ItemID));

        public string GetDrawText()
            => $"F6: Add 1 ({(ItemID)itemEnumValues.GetValue(selectedItemIndex)}) - F7 Scroll Down - F8 Scroll Up";

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
                if (selectedItemIndex < 0)
                    selectedItemIndex = itemEnumValues.Length - 2;
            }
            if (Keyboard.current.f8Key.wasPressedThisFrame)
            {
                selectedItemIndex++;
                if (selectedItemIndex > itemEnumValues.Length - 2)
                    selectedItemIndex = 0;
            }
        }

        public bool IsKeybindTriggered =>
            Keyboard.current.f6Key.wasPressedThisFrame ||
            Keyboard.current.f7Key.wasPressedThisFrame ||
            Keyboard.current.f8Key.wasPressedThisFrame;
    }

    /// <summary>
    /// Enable/disable Abnormal Statuses on the player.
    /// F9       — toggle the selected status on/off
    /// F10/F11  — scroll selection
    /// Shift+F10/F11 — for statuses with MaxLevel &gt; 1, decrement / increment the level
    /// </summary>
    public class AbnormalEntry : ICheatMenuEntry
    {
        // Index 0 of the AbnormalType enum is a sentinel (None), so we start at 1.
        private int selectedAbnormalIndex = 1;
        private readonly Array abnormalEnumValues = Enum.GetValues(typeof(AbnormalType));

        private AbnormalType SelectedType => (AbnormalType)abnormalEnumValues.GetValue(selectedAbnormalIndex);

        public string GetDrawText()
        {
            AbnormalType selectedType = SelectedType;

            bool hasState = false;
            int currentLevel = 0;
            int maxLevel = 0;

            // PlayerStatus / AbnormalList can be uninitialized while a scene is loading.
            // Swallow narrowly and log at debug so it doesn't spam release logs every frame.
            try
            {
                if (ManagerList.PlayerStatus != null && ManagerList.PlayerStatus.AbnormalList != null)
                {
                    hasState = ManagerList.PlayerStatus.AbnormalList.Has(selectedType);
                    if (hasState)
                    {
                        AbnormalData data = ManagerList.PlayerStatus.AbnormalList.GetAbnormalData(selectedType);
                        if (data != null)
                        {
                            currentLevel = data.Level;
                            maxLevel = data.MaxLevel;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Instance.Log.LogDebug($"AbnormalEntry.GetDrawText: {ex.GetType().Name}: {ex.Message}");
            }

            string status = hasState ? "Enabled" : "Disabled";
            string levelInfo = (hasState && maxLevel > 1)
                ? $" [Lv {currentLevel}/{maxLevel}, Shift+F10/F11 to adjust]"
                : "";
            return $"F9: ({selectedType}) {status}{levelInfo} - F10 Scroll Down - F11 Scroll Up (May break your game)";
        }

        public void KeybindBehaviour()
        {
            bool shift = Keyboard.current.shiftKey.isPressed;

            if (Keyboard.current.f9Key.wasPressedThisFrame)
                ToggleSelected();

            if (Keyboard.current.f10Key.wasPressedThisFrame)
            {
                if (shift) AdjustLevel(-1);
                else ScrollSelection(-1);
            }

            if (Keyboard.current.f11Key.wasPressedThisFrame)
            {
                if (shift) AdjustLevel(+1);
                else ScrollSelection(+1);
            }
        }

        public bool IsKeybindTriggered =>
            Keyboard.current.f9Key.wasPressedThisFrame  ||
            Keyboard.current.f10Key.wasPressedThisFrame ||
            Keyboard.current.f11Key.wasPressedThisFrame;

        private void ScrollSelection(int direction)
        {
            selectedAbnormalIndex += direction;
            if (selectedAbnormalIndex < 1)
                selectedAbnormalIndex = abnormalEnumValues.Length - 1;
            else if (selectedAbnormalIndex > abnormalEnumValues.Length - 1)
                selectedAbnormalIndex = 1;
        }

        private void ToggleSelected()
        {
            AbnormalType selectedType = SelectedType;
            AbnormalList abnormalList = ManagerList.PlayerStatus.AbnormalList;

            if (abnormalList.Has(selectedType))
            {
                abnormalList.RemoveAbnormal(selectedType);
            }
            else
            {
                abnormalList.AddOrRemoveAbnormal(selectedType, true);
                // Some statuses won't add via the normal path. Force-load and inject if needed.
                if (!abnormalList.Has(selectedType))
                {
                    if (AbnormalDataHandler.TryLoadAbnormalData(selectedType, out AbnormalData data))
                    {
                        abnormalList.AddAbnormal(data);
                        Plugin.Instance.Log.LogInfo($"Forcefully added {selectedType} to the Abnormal List");
                        Plugin.Instance.Log.LogWarning("Forcefully adding a status will likely bug out your game or do nothing.");
                    }
                    else
                    {
                        Plugin.Instance.Log.LogError($"Failed to add {selectedType} to the Abnormal List");
                    }
                }
            }
            Plugin.Instance.Log.LogInfo($"Toggled {selectedType}");
        }

        private void AdjustLevel(int direction)
        {
            AbnormalType selectedType = SelectedType;
            AbnormalList abnormalList = ManagerList.PlayerStatus?.AbnormalList;

            if (abnormalList == null || !abnormalList.Has(selectedType))
            {
                Plugin.Instance.Log.LogInfo($"Cannot adjust level: {selectedType} is not currently active.");
                return;
            }

            AbnormalData data = abnormalList.GetAbnormalData(selectedType);
            if (data == null)
                return;

            if (data.MaxLevel <= 1)
            {
                Plugin.Instance.Log.LogInfo($"{selectedType} has no adjustable levels (MaxLevel = {data.MaxLevel}).");
                return;
            }

            // _IncrementLevel / _DecrementLevel are the game's own clamped accessors.
            if (direction > 0) data._IncrementLevel();
            else               data._DecrementLevel();

            Plugin.Instance.Log.LogInfo($"{selectedType} level: {data.Level}/{data.MaxLevel}");
        }
    }

    public class BlockBindEntry : ICheatMenuEntry
    {
        public string GetDrawText()
            => CheatMenuEntryHandler.FormatToggle("1: Block Bind", CheatMenuEntryHandler.GetValue(CheatKeys.BlockBind));

        public void KeybindBehaviour() => CheatMenuEntryHandler.ToggleValue(CheatKeys.BlockBind);

        public bool IsKeybindTriggered => Keyboard.current.digit1Key.wasPressedThisFrame;
    }

    public class ToggleClothingEntry : ICheatMenuEntry
    {
        public string GetDrawText() => "2: Toggle Clothing State";

        public void KeybindBehaviour()
        {
            var durability = ManagerList.PlayerStatus.Durability;
            durability.SetCurrentValue(durability.Current != 0 ? 0 : durability.Max);
        }

        public bool IsKeybindTriggered => Keyboard.current.digit2Key.wasPressedThisFrame;
    }

    public class AttackEntry : ICheatMenuEntry
    {
        public string GetDrawText()
            => CheatMenuEntryHandler.FormatToggle("3: Instant Kill", CheatMenuEntryHandler.GetValue(CheatKeys.AttackCheat));

        public void KeybindBehaviour() => CheatMenuEntryHandler.ToggleValue(CheatKeys.AttackCheat);

        public bool IsKeybindTriggered => Keyboard.current.digit3Key.wasPressedThisFrame;
    }

    public class ReleaseBindEntry : ICheatMenuEntry
    {
        public string GetDrawText() => "4: Release current Bind";

        public void KeybindBehaviour() => ManagerList.Object.Lelia.Bind.ReleaseBind();

        public bool IsKeybindTriggered => Keyboard.current.digit4Key.wasPressedThisFrame;
    }

    public class BlockAllDamageEntry : ICheatMenuEntry
    {
        public string GetDrawText()
            => CheatMenuEntryHandler.FormatToggle("5: Block All Damage", CheatMenuEntryHandler.GetValue(CheatKeys.BlockAllDamage));

        public void KeybindBehaviour() => CheatMenuEntryHandler.ToggleValue(CheatKeys.BlockAllDamage);

        public bool IsKeybindTriggered => Keyboard.current.digit5Key.wasPressedThisFrame;
    }

    public class ShowEnemyHealthEntry : ICheatMenuEntry
    {
        public string GetDrawText()
            => CheatMenuEntryHandler.FormatToggle("6: Show Enemy Health", CheatMenuEntryHandler.GetValue(CheatKeys.ShowEnemyHP));

        public void KeybindBehaviour() => CheatMenuEntryHandler.ToggleValue(CheatKeys.ShowEnemyHP);

        public bool IsKeybindTriggered => Keyboard.current.digit6Key.wasPressedThisFrame;
    }

    public class KillAllEnemiesEntry : ICheatMenuEntry
    {
        public string GetDrawText() => "7: Kill All Enemies in current Level";

        public void KeybindBehaviour()
        {
            foreach (EnemyObject enemy in CheatMenuBehaviour.EnemyObjectList)
            {
                if (enemy == null || enemy.DeadState != EnemyDead.State.Alive || enemy.HP == null)
                    continue;
                enemy.HP.SetCurrentValue(0);
            }
        }

        public bool IsKeybindTriggered => Keyboard.current.digit7Key.wasPressedThisFrame;
    }
}
