using UnityEngine;

namespace SiNiSistar2Mod.CheatMenu
{
    /// <summary>
    /// Owns the visual language of the cheat menu: a small set of procedurally
    /// generated rounded textures baked into per-purpose <see cref="GUIStyle"/>s,
    /// plus the reusable widgets (switch / button / key chip) every entry draws.
    ///
    /// Everything renders through <c>GUI.Box</c> / <c>GUI.Button</c> / <c>GUI.Label</c>
    /// with a style background — the same primitives the original menu used. We
    /// deliberately avoid <c>GUI.DrawTexture</c>, <c>GUI.color</c> and
    /// <c>GUI.backgroundColor</c>: this game's IL2CPP build strips them and
    /// Il2CppInterop cannot unstrip them ("Method unstripping failed"). Colors are
    /// therefore baked into textures rather than applied as a runtime tint.
    /// </summary>
    public sealed class CheatMenuTheme
    {
        // ---- palette -------------------------------------------------------
        public static readonly Color Panel    = new Color(0.09f, 0.10f, 0.13f, 0.97f);
        public static readonly Color ListBg   = new Color(0.12f, 0.13f, 0.17f, 0.85f);
        public static readonly Color ChipBg   = new Color(0.20f, 0.21f, 0.27f, 1f);
        public static readonly Color Accent   = new Color(0.29f, 0.86f, 0.60f, 1f); // mint green
        public static readonly Color Danger   = new Color(0.93f, 0.36f, 0.38f, 1f); // red
        public static readonly Color Muted    = new Color(0.28f, 0.30f, 0.36f, 1f);
        public static readonly Color TrackOff = new Color(0.26f, 0.27f, 0.32f, 1f);
        public static readonly Color Knob     = new Color(0.96f, 0.97f, 0.99f, 1f);
        public static readonly Color TextMain = new Color(0.90f, 0.91f, 0.94f, 1f);
        public static readonly Color TextDim  = new Color(0.56f, 0.58f, 0.64f, 1f);
        public static readonly Color OnDark   = new Color(0.05f, 0.07f, 0.09f, 1f); // text on accent

        // ---- layout metrics ------------------------------------------------
        public const float Pad     = 12f;
        public const float RowH    = 30f;
        public const float RowGap  = 2f;
        public const float CatH    = 24f;
        public const float HeaderH = 44f;
        public const float SwitchW = 46f;
        public const float SwitchH = 24f;
        public const float BtnW    = 78f;
        public const float BtnH    = 24f;
        public const float ChipW   = 30f;
        public const float ChipH   = 20f;
        public const float Gap     = 8f;

        // ---- text styles ---------------------------------------------------
        public GUIStyle Window   { get; private set; }
        public GUIStyle Title    { get; private set; }
        public GUIStyle Version  { get; private set; }
        public GUIStyle Category { get; private set; }
        public GUIStyle Label          { get; private set; }
        public GUIStyle LabelDim       { get; private set; }
        public GUIStyle LabelDimCenter { get; private set; }

        // ---- widget styles -------------------------------------------------
        private GUIStyle _chip;
        private GUIStyle _list;
        private GUIStyle _trackOn, _trackOff, _knob;
        private GUIStyle _btnAccent, _btnDanger, _btnMuted;
        private GUIStyle _selRow, _sepLight, _sepAccent;

        private bool _built;

        /// <summary>Builds textures + styles once. Must be called from within OnGUI.</summary>
        public void EnsureBuilt()
        {
            if (_built)
                return;
            _built = true;

            // --- text styles ---
            Window = BoxStyle(MakeRounded(24, 24, 10, Panel), 12);

            Title = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };
            Title.normal.textColor = TextMain;

            Version = new GUIStyle(GUI.skin.label) { fontSize = 11, alignment = TextAnchor.MiddleRight };
            Version.normal.textColor = TextDim;

            Category = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.LowerLeft
            };
            Category.normal.textColor = Accent;

            Label = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft,
                richText = true,
                wordWrap = false
            };
            Label.normal.textColor = TextMain;

            LabelDim = new GUIStyle(Label);
            LabelDim.normal.textColor = TextDim;

            LabelDimCenter = new GUIStyle(LabelDim) { alignment = TextAnchor.MiddleCenter };

            // --- widget styles (backgrounds baked with color) ---
            _chip = new GUIStyle(GUIStyle.none) { fontSize = 11, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            _chip.border = new RectOffset(6, 6, 6, 6);
            _chip.normal.background = MakeRounded(18, 18, 6, ChipBg);
            _chip.normal.textColor = TextDim;

            _list     = BoxStyle(MakeRounded(20, 20, 8, ListBg), 8);
            _trackOn  = BoxStyle(MakeRounded((int)SwitchW, (int)SwitchH, (int)(SwitchH / 2f), new Color(Accent.r, Accent.g, Accent.b, 0.9f)), 12);
            _trackOff = BoxStyle(MakeRounded((int)SwitchW, (int)SwitchH, (int)(SwitchH / 2f), TrackOff), 12);
            _knob     = BoxStyle(MakeRounded(18, 18, 9, Knob), 9);

            _btnAccent = ButtonStyle(MakeRounded(20, 20, 8, Accent), OnDark);
            _btnDanger = ButtonStyle(MakeRounded(20, 20, 8, Danger), Color.white);
            _btnMuted  = ButtonStyle(MakeRounded(20, 20, 8, Muted), TextMain);

            _selRow    = BoxStyle(MakeRounded(1, 1, 0, new Color(Accent.r, Accent.g, Accent.b, 0.12f)), 0);
            _sepLight  = BoxStyle(MakeRounded(1, 1, 0, new Color(1f, 1f, 1f, 0.06f)), 0);
            _sepAccent = BoxStyle(MakeRounded(1, 1, 0, new Color(Accent.r, Accent.g, Accent.b, 0.55f)), 0);
        }

        // ---- widgets -------------------------------------------------------

        /// <summary>Draws a toggle switch; returns true on the frame it is clicked.</summary>
        public bool Switch(Rect r, bool on)
        {
            GUI.Box(r, GUIContent.none, on ? _trackOn : _trackOff);
            float d = r.height - 6f;
            float kx = on ? r.xMax - d - 3f : r.x + 3f;
            GUI.Box(new Rect(kx, r.y + 3f, d, d), GUIContent.none, _knob);
            return GUI.Button(r, GUIContent.none, GUIStyle.none);
        }

        /// <summary>Accent (or danger) filled button; returns true on click.</summary>
        public bool PrimaryButton(Rect r, string label, bool danger = false)
            => GUI.Button(r, label, danger ? _btnDanger : _btnAccent);

        /// <summary>Small muted button (arrows / steppers); returns true on click.</summary>
        public bool SmallButton(Rect r, string label) => GUI.Button(r, label, _btnMuted);

        /// <summary>Small pill showing the keyboard shortcut for an entry.</summary>
        public void KeyChip(Rect r, string key)
        {
            if (!string.IsNullOrEmpty(key))
                GUI.Label(r, key, _chip);
        }

        public void ListBox(Rect r)      => GUI.Box(r, GUIContent.none, _list);
        public void RowHighlight(Rect r) => GUI.Box(r, GUIContent.none, _selRow);
        public void Separator(Rect r, bool accent) => GUI.Box(r, GUIContent.none, accent ? _sepAccent : _sepLight);

        // ---- style + texture helpers --------------------------------------

        private static GUIStyle BoxStyle(Texture2D tex, int border)
        {
            var s = new GUIStyle(GUIStyle.none);
            s.normal.background = tex;
            if (border > 0)
                s.border = new RectOffset(border, border, border, border);
            return s;
        }

        private static GUIStyle ButtonStyle(Texture2D tex, Color text)
        {
            var s = new GUIStyle(GUIStyle.none)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                border = new RectOffset(8, 8, 8, 8)
            };
            s.normal.background = s.hover.background = s.active.background = s.focused.background = tex;
            s.normal.textColor = s.hover.textColor = s.active.textColor = s.focused.textColor = text;
            return s;
        }

        /// <summary>
        /// Builds a rounded-rectangle texture with anti-aliased corners. Radius 0
        /// yields a solid fill; radius == height/2 yields a pill; a square with
        /// radius == size/2 yields a circle. The color is baked in.
        /// </summary>
        private static Texture2D MakeRounded(int w, int h, int radius, Color col)
        {
            var tex = new Texture2D(w, h, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                hideFlags = HideFlags.HideAndDontSave
            };

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float a = RoundedAlpha(x, y, w, h, radius);
                    tex.SetPixel(x, y, new Color(col.r, col.g, col.b, col.a * a));
                }
            }

            tex.Apply();
            return tex;
        }

        // Signed-distance coverage for a rounded rect: 1 inside, fading to 0 across
        // a ~1px band at the rounded corners.
        private static float RoundedAlpha(int x, int y, int w, int h, int r)
        {
            if (r <= 0)
                return 1f;
            float fx = x + 0.5f, fy = y + 0.5f;
            float cx = Mathf.Clamp(fx, r, w - r);
            float cy = Mathf.Clamp(fy, r, h - r);
            float dx = fx - cx, dy = fy - cy;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            return Mathf.Clamp01(r - dist + 0.5f);
        }
    }
}
