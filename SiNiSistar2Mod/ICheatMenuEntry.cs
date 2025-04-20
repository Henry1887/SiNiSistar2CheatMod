namespace SiNiSistar2Mod
{
    internal interface ICheatMenuEntry
    {
        string GetDrawText(); // Gets called by OnGUI if the menu is visible
        void KeybindBehaviour(); // Gets called constantly for keybind behaviour
    }

    public static class CheatMenuEntryHandler
    {
        private static readonly List<ICheatMenuEntry> CheatMenuEntries = new();

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
            CheatMenuEntries.Add(new SceneSelectEntry());
            CheatMenuEntries.Add(new BlockBindEntry());
            CheatMenuEntries.Add(new ToggleClothingEntry());
            CheatMenuEntries.Add(new AttackEntry());
            CheatMenuEntries.Add(new ReleaseBindEntry());
            CheatMenuEntries.Add(new BlockAllDamageEntry());
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
                entry.KeybindBehaviour();
            }
        }
    }
}
