using SiNiSistar2.Obj;
using UnityEngine;

namespace SiNiSistar2Mod.CheatMenu
{
    public class CheatMenuBehaviour : MonoBehaviour
    {
        public CheatMenuBehaviour(System.IntPtr ptr) : base(ptr) { }

        public int boxWidth = 500;
        public int boxInnerPadding = 5;
        public int lineHeight = 22;
        public int lineWidth;

        private Rect windowRect = new Rect(10, 10, 500, 300);

        private List<string> DrawEntries = new();

        public static List<EnemyObject> EnemyObjectList = new();

        private GUIStyle _enemyHpStyle;
        private Camera _cachedCamera;

        private void Start()
        {
            lineWidth = boxWidth - (boxInnerPadding * 2);
            windowRect.width = boxWidth;
        }

        private void Update()
        {
            EnemyObjectList.RemoveAll(e => e == null);

            CheatMenuEntryHandler.KeybindBehaviour();
        }

        private void OnGUI()
        {
            if (CheatMenuEntryHandler.GetValue(CheatKeys.ShowEnemyHP))
                DrawEnemyHealth();

            if (!CheatMenuEntryHandler.GetValue(CheatKeys.IsVisible, true))
                return;

            DrawEntries = CheatMenuEntryHandler.GetDrawBuffer();
            windowRect.height = boxInnerPadding * 2 + (DrawEntries.Count * lineHeight) + 20;
            windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)DrawCheatWindow, "Cheat Menu");
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

        private void DrawCheatWindow(int windowID)
        {
            for (int i = 0; i < DrawEntries.Count; i++)
            {
                GUI.Label(new Rect(boxInnerPadding, boxInnerPadding + (i * lineHeight) + 20, lineWidth, lineHeight), DrawEntries[i]);
            }

            GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));
        }
    }
}
