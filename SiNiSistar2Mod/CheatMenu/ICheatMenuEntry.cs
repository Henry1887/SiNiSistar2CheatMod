namespace SiNiSistar2Mod.CheatMenu
{
    public interface ICheatMenuEntry
    {
        string GetDrawText(); // Gets called by OnGUI if the menu is visible
        void KeybindBehaviour(); // Gets called if IsKeybindTriggered() returns true
        bool IsKeybindTriggered { get; } // Checks if the keybind is triggered
    }
}
