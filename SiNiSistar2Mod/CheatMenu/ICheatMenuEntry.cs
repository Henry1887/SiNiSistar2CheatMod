using UnityEngine;

namespace SiNiSistar2Mod.CheatMenu
{
    public interface ICheatMenuEntry
    {
        /// <summary>Section this entry is grouped under in the menu.</summary>
        string Category { get; }

        /// <summary>
        /// Draws the entry inside <paramref name="area"/> (x/y/width supplied by the
        /// menu; the entry decides its own height) and returns the height consumed.
        /// Any click handling happens here — this is the clickable counterpart to the
        /// keyboard fallback.
        /// </summary>
        float DrawRow(Rect area, CheatMenuTheme theme);

        /// <summary>True on the frame this entry's keyboard shortcut fires.</summary>
        bool IsKeybindTriggered { get; }

        /// <summary>Runs the entry's action from the keyboard fallback path.</summary>
        void KeybindBehaviour();
    }
}
