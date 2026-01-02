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

        private List<string> DrawEntries = new List<string>();

        public static List<EnemyObject> EnemyObjectList = new List<EnemyObject>();

        private void Start()
        {
            lineWidth = boxWidth - (boxInnerPadding * 2);
            windowRect.width = boxWidth;
        }

        private void Update()
        {
            foreach (EnemyObject enemy in EnemyObjectList)
            {
                if (enemy == null)
                {
                    EnemyObjectList.Remove(enemy);
                    break;
                }
            }
            CheatMenuEntryHandler.KeybindBehaviour();
        }

        private void OnGUI()
        {
            if (CheatMenuEntryHandler.GetValue("ShowEnemyHP"))
            {
                DrawEnemyHealth();
            }
            if (!CheatMenuEntryHandler.GetValue("IsVisible", true))
                return;
            DrawEntries = CheatMenuEntryHandler.GetDrawBuffer();
            windowRect.height = boxInnerPadding * 2 + (DrawEntries.Count * lineHeight) + 20;
            windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)DrawCheatWindow, "Cheat Menu");
        }

        private void DrawEnemyHealth()
        {
            foreach (EnemyObject enemy in EnemyObjectList)
            {
                if (enemy == null || enemy.DeadState != EnemyDead.State.Alive || enemy.HP == null) continue;
                Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
                string hpText = $"{enemy.HP.Current}/{enemy.HP.Max}";

                GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 14,
                    alignment = TextAnchor.MiddleCenter
                };
                float textWidth = labelStyle.CalcSize(new GUIContent(hpText)).x;
                float textHeight = labelStyle.CalcSize(new GUIContent(hpText)).y;

                float boxWidth = textWidth + 10f;
                float boxHeight = textHeight + 5f;

                float x = screenPos.x - boxWidth / 2;
                float y = Screen.height - screenPos.y - boxHeight - 50;

                GUI.Box(new Rect(x, y, boxWidth, boxHeight), GUIContent.none);
                GUI.Label(new Rect(x, y, boxWidth, boxHeight), hpText, labelStyle);
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
