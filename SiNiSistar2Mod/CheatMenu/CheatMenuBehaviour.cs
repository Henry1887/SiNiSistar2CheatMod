using SiNiSistar2.Obj;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SiNiSistar2Mod.CheatMenu
{
    public class CheatMenuBehaviour : MonoBehaviour
    {
        public CheatMenuBehaviour(System.IntPtr ptr) : base(ptr) { }

        private const float MenuWidth = 430f;

        private Rect windowRect = new Rect(10, 10, MenuWidth, 760);
        private float _contentHeight = 760f; // measured during draw, applied next frame

        public static List<EnemyObject> EnemyObjectList = new();

        private readonly CheatMenuTheme _theme = new();
        private GUIStyle _enemyHpStyle;
        private Camera _cachedCamera;
        private bool _loggedDrawError; // avoids spamming the log every frame on a draw fault

        private void Update()
        {
            EnemyObjectList.RemoveAll(e => e == null);

            if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
                CheatMenuEntryHandler.SetValue(CheatKeys.IsVisible,
                    !CheatMenuEntryHandler.GetValue(CheatKeys.IsVisible, true));

            // The menu needs a visible pointer to be clickable.
            if (CheatMenuEntryHandler.GetValue(CheatKeys.IsVisible, true))
            {
                try
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                catch { /* Cursor accessors are ICALL-backed; ignore if unavailable */ }
            }

            CheatMenuEntryHandler.KeybindBehaviour();
        }

        private void OnGUI()
        {
            try
            {
                _theme.EnsureBuilt();

                if (CheatMenuEntryHandler.GetValue(CheatKeys.ShowEnemyHP))
                    DrawEnemyHealth();

                if (!CheatMenuEntryHandler.GetValue(CheatKeys.IsVisible, true))
                    return;

                windowRect.width = MenuWidth;
                windowRect.height = _contentHeight;
                windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)DrawCheatWindow, "", _theme.Window);
            }
            catch (Exception ex)
            {
                if (!_loggedDrawError)
                {
                    _loggedDrawError = true;
                    Plugin.Instance.Log.LogError($"Cheat menu draw failed: {ex}");
                }
            }
        }

        private void DrawCheatWindow(int windowID)
        {
            // This runs via an IL2CPP delegate trampoline that swallows exceptions,
            // so the guard has to live here rather than around GUI.Window.
            try
            {
                DrawHeader();

                float x = CheatMenuTheme.Pad;
                float contentW = MenuWidth - CheatMenuTheme.Pad * 2f;
                float y = CheatMenuTheme.HeaderH + 6f;
                string currentCategory = null;

                foreach (ICheatMenuEntry entry in CheatMenuEntryHandler.Entries)
                {
                    if (entry.Category != currentCategory)
                    {
                        currentCategory = entry.Category;
                        DrawCategory(currentCategory, x, ref y, contentW);
                    }

                    float h = entry.DrawRow(new Rect(x, y, contentW, CheatMenuTheme.RowH), _theme);
                    y += h + CheatMenuTheme.RowGap;
                }

                _contentHeight = y + CheatMenuTheme.Pad;
            }
            catch (Exception ex)
            {
                if (!_loggedDrawError)
                {
                    _loggedDrawError = true;
                    Plugin.Instance.Log.LogError($"Cheat menu window draw failed: {ex}");
                }
            }

            // Drag anywhere on the header strip (the close button, drawn first, wins its own clicks).
            GUI.DragWindow(new Rect(0, 0, MenuWidth, CheatMenuTheme.HeaderH));
        }

        private void DrawHeader()
        {
            var title = new Rect(CheatMenuTheme.Pad, 0, MenuWidth - 90f, CheatMenuTheme.HeaderH);
            GUI.Label(title, $"<color=#4ADB99>SiNiSistar 2</color>  <color=#8A8D96>Cheats</color>", _theme.Title);

            var close = new Rect(MenuWidth - CheatMenuTheme.Pad - 26f,
                                 (CheatMenuTheme.HeaderH - 26f) / 2f, 26f, 26f);
            if (_theme.PrimaryButton(close, "X", danger: true))
                CheatMenuEntryHandler.SetValue(CheatKeys.IsVisible, false);

            var version = new Rect(close.x - 60f, 0, 56f, CheatMenuTheme.HeaderH);
            GUI.Label(version, "v1.0.8", _theme.Version);

            // Accent separator under the header.
            _theme.Separator(new Rect(CheatMenuTheme.Pad, CheatMenuTheme.HeaderH - 1f,
                                      MenuWidth - CheatMenuTheme.Pad * 2f, 2f), accent: true);
        }

        private void DrawCategory(string name, float x, ref float y, float contentW)
        {
            var label = new Rect(x, y, contentW, CheatMenuTheme.CatH);
            GUI.Label(label, name.ToUpperInvariant(), _theme.Category);
            _theme.Separator(new Rect(x, y + CheatMenuTheme.CatH - 3f, contentW, 1f), accent: false);
            y += CheatMenuTheme.CatH;
        }

        private void DrawEnemyHealth()
        {
            _enemyHpStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };

            // Camera.main can become null between scenes; refresh lazily.
            if (_cachedCamera == null)
                _cachedCamera = Camera.main;
            if (_cachedCamera == null)
                return;

            foreach (EnemyObject enemy in EnemyObjectList)
            {
                if (enemy == null || enemy.DeadState != EnemyDead.State.Alive || enemy.HP == null)
                    continue;

                Vector3 screenPos = _cachedCamera.WorldToScreenPoint(enemy.transform.position);
                if (screenPos.z < 0f)
                    continue; // behind the camera — would render mirrored otherwise

                string hpText = $"{enemy.HP.Current}/{enemy.HP.Max}";
                Vector2 size = _enemyHpStyle.CalcSize(new GUIContent(hpText));

                float boxW = size.x + 10f;
                float boxH = size.y + 5f;
                float x = screenPos.x - boxW / 2;
                float y = Screen.height - screenPos.y - boxH - 50;

                Rect r = new Rect(x, y, boxW, boxH);
                GUI.Box(r, GUIContent.none);
                GUI.Label(r, hpText, _enemyHpStyle);
            }
        }
    }
}
