using SiNiSistar2.Manager;
using SiNiSistar2.Obj;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SiNiSistar2Mod.CheatMenu
{
    internal static class Categories
    {
        public const string Player = "Player";
        public const string Combat = "Combat";
        public const string Items  = "Items";
        public const string Status = "Abnormal Status";
    }

    // ----- Player -----------------------------------------------------------

    public class MaxHPEntry : ToggleEntry
    {
        protected override string Label => "Max HP";
        protected override string CheatKey => CheatKeys.MaxHP;
        protected override string Hotkey => "F2";
        public override string Category => Categories.Player;

        protected override void OnChanged(bool nowOn)
        {
            // Max HP and Lock HP=1 are mutually exclusive.
            if (nowOn && CheatMenuEntryHandler.GetValue(CheatKeys.LockHP1))
                CheatMenuEntryHandler.SetValue(CheatKeys.LockHP1, false);
        }

        public override bool IsKeybindTriggered => Keyboard.current.f2Key.wasPressedThisFrame;
    }

    public class MaxMPEntry : ToggleEntry
    {
        protected override string Label => "Max MP";
        protected override string CheatKey => CheatKeys.MaxMP;
        protected override string Hotkey => "F3";
        public override string Category => Categories.Player;
        public override bool IsKeybindTriggered => Keyboard.current.f3Key.wasPressedThisFrame;
    }

    public class LockHP1Entry : ToggleEntry
    {
        protected override string Label => "Lock HP to 1";
        protected override string CheatKey => CheatKeys.LockHP1;
        protected override string Hotkey => "F5";
        protected override string SubLabel => "May still cause a Game Over";
        public override string Category => Categories.Player;

        protected override void OnChanged(bool nowOn)
        {
            // Mutually exclusive with Max HP.
            if (nowOn && CheatMenuEntryHandler.GetValue(CheatKeys.MaxHP))
                CheatMenuEntryHandler.SetValue(CheatKeys.MaxHP, false);
        }

        public override bool IsKeybindTriggered => Keyboard.current.f5Key.wasPressedThisFrame;
    }

    public class ToggleClothingEntry : ActionEntry
    {
        protected override string Label => "Toggle Clothing State";
        protected override string Hotkey => "2";
        protected override string ButtonText => "Toggle";
        public override string Category => Categories.Player;

        protected override void Run()
        {
            var durability = ManagerList.PlayerStatus.Durability;
            durability.SetCurrentValue(durability.Current != 0 ? 0 : durability.Max);
        }

        public override bool IsKeybindTriggered => Keyboard.current.digit2Key.wasPressedThisFrame;
    }

    public class AddRelicsEntry : ActionEntry
    {
        protected override string Label => "Add 1000 Relics";
        protected override string Hotkey => "F4";
        protected override string ButtonText => "Add";
        public override string Category => Categories.Player;

        protected override void Run() => ManagerList.PlayerStatus.AddRelics(1000, false);

        public override bool IsKeybindTriggered => Keyboard.current.f4Key.wasPressedThisFrame;
    }

    // ----- Combat -----------------------------------------------------------

    public class AttackEntry : ToggleEntry
    {
        protected override string Label => "Instant Kill";
        protected override string CheatKey => CheatKeys.AttackCheat;
        protected override string Hotkey => "3";
        public override string Category => Categories.Combat;
        public override bool IsKeybindTriggered => Keyboard.current.digit3Key.wasPressedThisFrame;
    }

    public class BlockAllDamageEntry : ToggleEntry
    {
        protected override string Label => "Block All Damage";
        protected override string CheatKey => CheatKeys.BlockAllDamage;
        protected override string Hotkey => "5";
        public override string Category => Categories.Combat;
        public override bool IsKeybindTriggered => Keyboard.current.digit5Key.wasPressedThisFrame;
    }

    public class BlockBindEntry : ToggleEntry
    {
        protected override string Label => "Block Bind";
        protected override string CheatKey => CheatKeys.BlockBind;
        protected override string Hotkey => "1";
        public override string Category => Categories.Combat;
        public override bool IsKeybindTriggered => Keyboard.current.digit1Key.wasPressedThisFrame;
    }

    public class ReleaseBindEntry : ActionEntry
    {
        protected override string Label => "Release Current Bind";
        protected override string Hotkey => "4";
        protected override string ButtonText => "Release";
        public override string Category => Categories.Combat;

        protected override void Run() => ManagerList.Object.Lelia.Bind.ReleaseBind();

        public override bool IsKeybindTriggered => Keyboard.current.digit4Key.wasPressedThisFrame;
    }

    public class ShowEnemyHealthEntry : ToggleEntry
    {
        protected override string Label => "Show Enemy Health";
        protected override string CheatKey => CheatKeys.ShowEnemyHP;
        protected override string Hotkey => "6";
        public override string Category => Categories.Combat;
        public override bool IsKeybindTriggered => Keyboard.current.digit6Key.wasPressedThisFrame;
    }

    public class KillAllEnemiesEntry : ActionEntry
    {
        protected override string Label => "Kill All Enemies";
        protected override string Hotkey => "7";
        protected override string ButtonText => "Kill All";
        public override string Category => Categories.Combat;
        protected override bool IsDanger => true;

        protected override void Run()
        {
            foreach (EnemyObject enemy in CheatMenuBehaviour.EnemyObjectList)
            {
                if (enemy == null || enemy.DeadState != EnemyDead.State.Alive || enemy.HP == null)
                    continue;
                enemy.HP.SetCurrentValue(0);
            }
        }

        public override bool IsKeybindTriggered => Keyboard.current.digit7Key.wasPressedThisFrame;
    }

    // ----- Items ------------------------------------------------------------

    /// <summary>
    /// Selector row: pick an item with the ◀ / ▶ buttons (or F7/F8) and grant one
    /// copy with the ADD button (or F6).
    /// </summary>
    public class AddItemEntry : ICheatMenuEntry
    {
        private int selectedItemIndex = 0;
        // The trailing enum value is a sentinel (e.g. Count/None), so the usable
        // range is [0, Length-2). This is why the wraparound math below stops short.
        private readonly Array itemEnumValues = Enum.GetValues(typeof(ItemID));

        public string Category => Categories.Items;

        private ItemID Selected => (ItemID)itemEnumValues.GetValue(selectedItemIndex);

        private void ScrollItem(int dir)
        {
            selectedItemIndex += dir;
            if (selectedItemIndex < 0)
                selectedItemIndex = itemEnumValues.Length - 2;
            else if (selectedItemIndex > itemEnumValues.Length - 2)
                selectedItemIndex = 0;
        }

        private void AddSelected()
        {
            ItemID itemId = Selected;
            try
            {
                ManagerList.PlayerStatus.InventoryHandler.AddItem(itemId, 1);
                Plugin.Instance.Log.LogInfo($"Added 1 {itemId}");
            }
            catch (Exception ex)
            {
                Plugin.Instance.Log.LogWarning($"Add Item ({itemId}): {ex.GetType().Name}: {ex.Message}");
            }
        }

        public float DrawRow(Rect area, CheatMenuTheme theme)
        {
            const float h = 30f;
            var row = new Rect(area.x, area.y + 2f, area.width, h);

            float addW = 62f, arrow = 28f;
            var add  = new Rect(row.xMax - addW, row.y + (h - CheatMenuTheme.BtnH) / 2f, addW, CheatMenuTheme.BtnH);
            var next = new Rect(add.x - CheatMenuTheme.Gap - arrow, add.y, arrow, CheatMenuTheme.BtnH);
            var prev = new Rect(area.x, add.y, arrow, CheatMenuTheme.BtnH);
            var name = new Rect(prev.xMax + 4f, row.y, next.x - 4f - (prev.xMax + 4f), h);

            theme.ListBox(name);
            var label = new Rect(name.x + 8f, name.y, name.width - 16f, name.height);
            GUI.Label(label, $"<b>{Selected}</b>", theme.Label);

            if (theme.SmallButton(prev, "<")) ScrollItem(-1);
            if (theme.SmallButton(next, ">")) ScrollItem(+1);
            if (theme.PrimaryButton(add, "ADD")) AddSelected();

            return h + 4f;
        }

        public bool IsKeybindTriggered =>
            Keyboard.current.f6Key.wasPressedThisFrame ||
            Keyboard.current.f7Key.wasPressedThisFrame ||
            Keyboard.current.f8Key.wasPressedThisFrame;

        public void KeybindBehaviour()
        {
            if (Keyboard.current.f6Key.wasPressedThisFrame) AddSelected();
            if (Keyboard.current.f7Key.wasPressedThisFrame) ScrollItem(-1);
            if (Keyboard.current.f8Key.wasPressedThisFrame) ScrollItem(+1);
        }
    }

    // ----- Abnormal Status --------------------------------------------------

    /// <summary>
    /// Scrollable list of every abnormal status. Each row has an ON/OFF switch;
    /// statuses with more than one level get - / + buttons to adjust the level.
    /// Keyboard fallback: F9 toggles the highlighted row, F10/F11 move the
    /// highlight, Shift+F10/F11 adjust its level.
    /// </summary>
    public class AbnormalEntry : ICheatMenuEntry
    {
        // Index 0 of the AbnormalType enum is a sentinel (None), so we start at 1.
        private int selectedAbnormalIndex = 1;
        private readonly Array abnormalEnumValues = Enum.GetValues(typeof(AbnormalType));
        private int _page;

        private const int PageSize = 6;
        private const float ItemHeight = 28f;

        public string Category => Categories.Status;

        // IMGUI scroll views are stripped in this IL2CPP build, so the list is
        // paginated with plain buttons instead of a GUI.BeginScrollView.
        private int LastIndex => abnormalEnumValues.Length - 1; // inclusive, sentinel at 0 excluded
        private int PageCount => Mathf.Max(1, Mathf.CeilToInt(LastIndex / (float)PageSize));

        private AbnormalType TypeAt(int index) => (AbnormalType)abnormalEnumValues.GetValue(index);
        private AbnormalType SelectedType => TypeAt(selectedAbnormalIndex);

        public float DrawRow(Rect area, CheatMenuTheme theme)
        {
            _page = Mathf.Clamp(_page, 0, PageCount - 1);

            var hint = new Rect(area.x, area.y, area.width, 16f);
            GUI.Label(hint, "<size=10>Click a status to toggle • use - / + to change level</size>", theme.LabelDim);

            var box = new Rect(area.x, hint.yMax + 2f, area.width, PageSize * ItemHeight + 8f);
            theme.ListBox(box);

            AbnormalList list = SafeAbnormalList();
            int first = 1 + _page * PageSize;
            for (int slot = 0; slot < PageSize; slot++)
            {
                int i = first + slot;
                if (i > LastIndex)
                    break;
                var rowRect = new Rect(box.x + 4f, box.y + 4f + slot * ItemHeight, box.width - 8f, ItemHeight);
                DrawStatusRow(theme, rowRect, i, TypeAt(i), list);
            }

            // Pager footer: ◀  page x / y  ▶
            var footer = new Rect(area.x, box.yMax + 4f, area.width, 22f);
            const float navW = 30f;
            var prev = new Rect(footer.x, footer.y, navW, 22f);
            var next = new Rect(footer.xMax - navW, footer.y, navW, 22f);
            var pageLbl = new Rect(prev.xMax, footer.y, next.x - prev.xMax, 22f);

            if (theme.SmallButton(prev, "<")) _page = (_page - 1 + PageCount) % PageCount;
            GUI.Label(pageLbl, $"Page {_page + 1} / {PageCount}", theme.LabelDimCenter);
            if (theme.SmallButton(next, ">")) _page = (_page + 1) % PageCount;

            return (footer.yMax - area.y) + 4f;
        }

        private void DrawStatusRow(CheatMenuTheme theme, Rect r, int index, AbnormalType type, AbnormalList list)
        {
            bool selected = index == selectedAbnormalIndex;
            if (selected)
                theme.RowHighlight(r);

            bool has = false;
            int level = 0, maxLevel = 0;
            if (list != null)
            {
                try
                {
                    has = list.Has(type);
                    if (has)
                    {
                        AbnormalData data = list.GetAbnormalData(type);
                        if (data != null) { level = data.Level; maxLevel = data.MaxLevel; }
                    }
                }
                catch { /* status list can churn mid-scene; treat as inactive this frame */ }
            }

            var sw = new Rect(r.xMax - CheatMenuTheme.SwitchW - 4f,
                              r.y + (r.height - CheatMenuTheme.SwitchH) / 2f,
                              CheatMenuTheme.SwitchW, CheatMenuTheme.SwitchH);

            // Level stepper (only for multi-level statuses that are active).
            float rightEdge = sw.x;
            if (has && maxLevel > 1)
            {
                const float stepW = 24f;
                var plus = new Rect(sw.x - CheatMenuTheme.Gap - stepW, sw.y, stepW, CheatMenuTheme.SwitchH);
                var lvl  = new Rect(plus.x - 44f, r.y, 44f, r.height);
                var minus = new Rect(lvl.x - stepW, sw.y, stepW, CheatMenuTheme.SwitchH);

                if (theme.SmallButton(minus, "-")) { selectedAbnormalIndex = index; AdjustLevel(type, -1); }
                GUI.Label(lvl, $"<size=11>Lv {level}/{maxLevel}</size>", theme.LabelDim);
                if (theme.SmallButton(plus, "+")) { selectedAbnormalIndex = index; AdjustLevel(type, +1); }
                rightEdge = minus.x;
            }

            var nameRect = new Rect(r.x + 8f, r.y, rightEdge - r.x - 8f, r.height);
            GUI.Label(nameRect, type.ToString(), has ? theme.Label : theme.LabelDim);

            if (theme.Switch(sw, has))
            {
                selectedAbnormalIndex = index;
                Toggle(type);
            }
        }

        // ----- shared behaviour (keyboard + click) --------------------------

        private static AbnormalList SafeAbnormalList()
        {
            try { return ManagerList.PlayerStatus?.AbnormalList; }
            catch { return null; }
        }

        private void ScrollSelection(int direction)
        {
            selectedAbnormalIndex += direction;
            if (selectedAbnormalIndex < 1)
                selectedAbnormalIndex = abnormalEnumValues.Length - 1;
            else if (selectedAbnormalIndex > abnormalEnumValues.Length - 1)
                selectedAbnormalIndex = 1;

            // Keep the highlighted row on the visible page.
            _page = (selectedAbnormalIndex - 1) / PageSize;
        }

        private void Toggle(AbnormalType selectedType)
        {
            AbnormalList abnormalList = SafeAbnormalList();
            if (abnormalList == null)
            {
                Plugin.Instance.Log.LogInfo("Abnormal list is not available yet.");
                return;
            }

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

        private void AdjustLevel(AbnormalType selectedType, int direction)
        {
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

        public bool IsKeybindTriggered =>
            Keyboard.current.f9Key.wasPressedThisFrame  ||
            Keyboard.current.f10Key.wasPressedThisFrame ||
            Keyboard.current.f11Key.wasPressedThisFrame;

        public void KeybindBehaviour()
        {
            bool shift = Keyboard.current.shiftKey.isPressed;

            if (Keyboard.current.f9Key.wasPressedThisFrame)
                Toggle(SelectedType);

            if (Keyboard.current.f10Key.wasPressedThisFrame)
            {
                if (shift) AdjustLevel(SelectedType, -1);
                else ScrollSelection(-1);
            }

            if (Keyboard.current.f11Key.wasPressedThisFrame)
            {
                if (shift) AdjustLevel(SelectedType, +1);
                else ScrollSelection(+1);
            }
        }
    }
}
