namespace SiNiSistar2Mod.CheatMenu
{
    public static class CheatMenuEntryHandler
    {
        private static readonly List<ICheatMenuEntry> CheatMenuEntries = new();
        private static readonly Dictionary<string, bool> EntryValues = new();

        public static IReadOnlyList<ICheatMenuEntry> Entries => CheatMenuEntries;

        public static bool GetValue(string key, bool defaultValue = false)
            => EntryValues.TryGetValue(key, out bool value) ? value : defaultValue;

        public static void SetValue(string key, bool value) => EntryValues[key] = value;

        public static bool ToggleValue(string key)
        {
            bool next = !GetValue(key);
            EntryValues[key] = next;
            return next;
        }

        public static void LoadEntries()
        {
            if (CheatMenuEntries.Count > 0)
                return;

            // Ordered by category so the menu can group them by first-seen section.
            // Player
            CheatMenuEntries.Add(new MaxHPEntry());
            CheatMenuEntries.Add(new MaxMPEntry());
            CheatMenuEntries.Add(new LockHP1Entry());
            CheatMenuEntries.Add(new ToggleClothingEntry());
            CheatMenuEntries.Add(new AddRelicsEntry());
            // Combat
            CheatMenuEntries.Add(new AttackEntry());
            CheatMenuEntries.Add(new BlockAllDamageEntry());
            CheatMenuEntries.Add(new BlockBindEntry());
            CheatMenuEntries.Add(new ReleaseBindEntry());
            CheatMenuEntries.Add(new ShowEnemyHealthEntry());
            CheatMenuEntries.Add(new KillAllEnemiesEntry());
            // Items
            CheatMenuEntries.Add(new AddItemEntry());
            // Abnormal Status
            CheatMenuEntries.Add(new AbnormalEntry());
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
