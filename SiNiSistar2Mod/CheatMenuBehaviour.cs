using SiNiSistar2.Obj;
using UnityEngine.InputSystem;
using UnityEngine;

namespace SiNiSistar2Mod
{
    public class CheatMenuBehaviour : MonoBehaviour
    {
        public CheatMenuBehaviour(System.IntPtr ptr) : base(ptr) { }

        private int selectedItemIndex = 0;
        private Array itemEnumValues;

        private int selectedAbnormalIndex = 1;
        private Array abnormalEnumValues;

        private void Start()
        {
            itemEnumValues = Enum.GetValues(typeof(ItemID));
            abnormalEnumValues = Enum.GetValues(typeof(AbnormalType));
            Plugin.Instance.Log.LogInfo("CheatMenuBehaviour started!");
        }

        private void Update()
        {
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
                Plugin.PlayerStatusManagerInstance.AbnormalList.AddOrRemoveAbnormal(selectedType, !Plugin.PlayerStatusManagerInstance.AbnormalList.Has(selectedType));
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
            GUI.Label(new Rect(10, 150, 500, 20), $"Note: Abnormal Statuses are location dependent if they can be added.");
        }
    }
}
