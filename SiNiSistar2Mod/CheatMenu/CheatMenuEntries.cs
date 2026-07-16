using UnityEngine;

namespace SiNiSistar2Mod.CheatMenu
{
    /// <summary>
    /// Base for on/off entries. Renders as a labelled row with a key chip and a
    /// toggle switch on the right; clicking the switch and pressing the hotkey both
    /// route through <see cref="Perform"/>.
    /// </summary>
    public abstract class ToggleEntry : ICheatMenuEntry
    {
        protected abstract string Label { get; }
        protected abstract string CheatKey { get; }
        protected abstract string Hotkey { get; }
        public abstract string Category { get; }
        public abstract bool IsKeybindTriggered { get; }

        protected virtual bool DefaultOn => false;

        /// <summary>Optional dim helper text shown under the label.</summary>
        protected virtual string SubLabel => null;

        /// <summary>Hook for side effects (e.g. clearing a mutually-exclusive toggle).</summary>
        protected virtual void OnChanged(bool nowOn) { }

        protected bool IsOn => CheatMenuEntryHandler.GetValue(CheatKey, DefaultOn);

        private void Perform()
        {
            bool now = CheatMenuEntryHandler.ToggleValue(CheatKey);
            OnChanged(now);
        }

        public void KeybindBehaviour() => Perform();

        public float DrawRow(Rect area, CheatMenuTheme theme)
        {
            var sw = new Rect(area.xMax - CheatMenuTheme.SwitchW,
                              area.y + (area.height - CheatMenuTheme.SwitchH) / 2f,
                              CheatMenuTheme.SwitchW, CheatMenuTheme.SwitchH);
            var chip = new Rect(sw.x - CheatMenuTheme.Gap - CheatMenuTheme.ChipW,
                                area.y + (area.height - CheatMenuTheme.ChipH) / 2f,
                                CheatMenuTheme.ChipW, CheatMenuTheme.ChipH);
            var label = new Rect(area.x, area.y, chip.x - CheatMenuTheme.Gap - area.x, area.height);

            DrawLabel(theme, label);
            theme.KeyChip(chip, Hotkey);
            if (theme.Switch(sw, IsOn))
                Perform();

            return area.height;
        }

        private void DrawLabel(CheatMenuTheme theme, Rect label)
        {
            string sub = SubLabel;
            if (string.IsNullOrEmpty(sub))
            {
                GUI.Label(label, Label, theme.Label);
                return;
            }
            var top = new Rect(label.x, label.y + 1f, label.width, label.height * 0.58f);
            var bot = new Rect(label.x, label.yMax - label.height * 0.5f, label.width, label.height * 0.42f);
            GUI.Label(top, Label, theme.Label);
            GUI.Label(bot, $"<size=10>{sub}</size>", theme.LabelDim);
        }
    }

    /// <summary>
    /// Base for one-shot actions. Renders as a labelled row with a key chip and a
    /// button; clicking the button and pressing the hotkey both route through
    /// <see cref="Run"/>.
    /// </summary>
    public abstract class ActionEntry : ICheatMenuEntry
    {
        protected abstract string Label { get; }
        protected abstract string Hotkey { get; }
        protected abstract string ButtonText { get; }
        public abstract string Category { get; }
        public abstract bool IsKeybindTriggered { get; }

        protected abstract void Run();

        /// <summary>Render the button in the destructive (red) style.</summary>
        protected virtual bool IsDanger => false;

        // The menu lives in every scene, so an action can be triggered when the
        // game state it touches isn't ready (e.g. on the title screen). Swallow so
        // one bad click never tears down the whole OnGUI.
        private void SafeRun()
        {
            try { Run(); }
            catch (Exception ex) { Plugin.Instance.Log.LogWarning($"{Label}: {ex.GetType().Name}: {ex.Message}"); }
        }

        public void KeybindBehaviour() => SafeRun();

        public float DrawRow(Rect area, CheatMenuTheme theme)
        {
            var btn = new Rect(area.xMax - CheatMenuTheme.BtnW,
                               area.y + (area.height - CheatMenuTheme.BtnH) / 2f,
                               CheatMenuTheme.BtnW, CheatMenuTheme.BtnH);
            var chip = new Rect(btn.x - CheatMenuTheme.Gap - CheatMenuTheme.ChipW,
                                area.y + (area.height - CheatMenuTheme.ChipH) / 2f,
                                CheatMenuTheme.ChipW, CheatMenuTheme.ChipH);
            var label = new Rect(area.x, area.y, chip.x - CheatMenuTheme.Gap - area.x, area.height);

            GUI.Label(label, Label, theme.Label);
            theme.KeyChip(chip, Hotkey);
            if (theme.PrimaryButton(btn, ButtonText, IsDanger))
                SafeRun();

            return area.height;
        }
    }
}
