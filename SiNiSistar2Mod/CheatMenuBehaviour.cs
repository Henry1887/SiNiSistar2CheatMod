using UnityEngine;

namespace SiNiSistar2Mod
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

        private void Start()
        {
            lineWidth = boxWidth - (boxInnerPadding * 2);
            windowRect.width = boxWidth;

            Plugin.Instance.Log.LogInfo("CheatMenuBehaviour started!");
        }

        private void Update()
        {
            CheatMenuEntryHandler.KeybindBehaviour();
        }

        private void OnGUI()
        {
            if (!Plugin.MenuVisible)
                return;
            DrawEntries = CheatMenuEntryHandler.GetDrawBuffer();
            windowRect.height = boxInnerPadding * 2 + (DrawEntries.Count * lineHeight) + 20; // +20 because of the titlebar
            windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)DrawCheatWindow, "Cheat Menu");
        }

        private void DrawCheatWindow(int windowID)
        {
            for (int i = 0; i < DrawEntries.Count; i++)
            {
                GUI.Label(new Rect(boxInnerPadding, boxInnerPadding + (i * lineHeight) + 20, lineWidth, lineHeight), DrawEntries[i]); // +20 because of the titlebar
            }

            GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));
        }
    }
}
