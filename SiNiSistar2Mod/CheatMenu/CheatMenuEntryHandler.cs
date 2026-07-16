namespace SiNiSistar2Mod.CheatMenu
{
    public static class CheatMenuEntryHandler
    {
        private static readonly List<ICheatMenuEntry> CheatMenuEntries = new();
        private static readonly Dictionary<string, bool> EntryValues = new();

        public static bool GetValue(string key, bool defaultValue = false)
            => EntryValues.TryGetValue(key, out bool value) ? value : defaultValue;

        public static void SetValue(string key, bool value) => EntryValues[key] = value;

        public static bool ToggleValue(string key)
        {
            bool next = !GetValue(key);
            EntryValues[key] = next;
            return next;
        }

        public static string FormatToggle(string label, bool state)
            => $"{label} - {(state ? "Enabled" : "Disabled")}";

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
            List<string> drawBuffer = new(CheatMenuEntries.Count);
            foreach (ICheatMenuEntry entry in CheatMenuEntries)
                drawBuffer.Add(entry.GetDrawText());
            return drawBuffer;
        }

        public static void KeybindBehaviour()
        {
            foreach (ICheatMenuEntry entry in CheatMenuEntries)
            {
                if (entry.IsKeybindTriggered)
                    entry.KeybindBehaviour();
            }
        }
    }
}
