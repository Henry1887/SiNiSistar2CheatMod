namespace SiNiSistar2Mod
{
    internal interface ICheatMenuEntry
    {
        string GetDrawText(); // Gets called by OnGUI if the menu is visible
        void KeybindBehaviour(); // Gets called if IsKeybindTriggered() returns true
        bool IsKeybindTriggered { get; } // Checks if the keybind is triggered
    }

    public static class CheatMenuEntryHandler
    {
        private static readonly List<ICheatMenuEntry> CheatMenuEntries = new();
        private static Dictionary<string, bool> EntryValues = new();

        public static bool GetValue(string key, bool defaultValue = false)
        {
            if (EntryValues.TryGetValue(key, out bool value))
            {
                return value;
            }
            return defaultValue;
        }
        public static void SetValue(string key, bool value)
        {
            if (EntryValues.ContainsKey(key))
            {
                EntryValues[key] = value;
            }
            else
            {
                EntryValues.Add(key, value);
            }
        }

        public static void LoadEntries()
        {
            if (CheatMenuEntries.Count > 0)
                return;
            CheatMenuEntries.Add(new HideEntry());
            CheatMenuEntries.Add(new MaxHPEntry());
            CheatMenuEntries.Add(new MaxMPEntry());
            CheatMenuEntries.Add(new AddRelicsEntry());
            CheatMenuEntries.Add(new LockHP1Entry());
            CheatMenuEntries.Add(new AddItemEntry());
            CheatMenuEntries.Add(new AbnormalEntry());
            CheatMenuEntries.Add(new BlockBindEntry());
            CheatMenuEntries.Add(new ToggleClothingEntry());
            CheatMenuEntries.Add(new AttackEntry());
            CheatMenuEntries.Add(new ReleaseBindEntry());
            CheatMenuEntries.Add(new BlockAllDamageEntry());
            CheatMenuEntries.Add(new ShowEnemyHealthEntry());
            CheatMenuEntries.Add(new KillAllEnemiesEntry());
        }

        public static List<string> GetDrawBuffer()
        {
            List<string> drawBuffer = new();
            foreach (ICheatMenuEntry entry in CheatMenuEntries)
            {
                drawBuffer.Add(entry.GetDrawText());
            }
            return drawBuffer;
        }

        public static void KeybindBehaviour()
        {
            foreach (ICheatMenuEntry entry in CheatMenuEntries)
            {
                if (!entry.IsKeybindTriggered)
                    continue;
                entry.KeybindBehaviour();
            }
        }
    }
}
