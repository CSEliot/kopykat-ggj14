using UnityEngine;
using System.Collections;

namespace KopyKat
{
    public static class UIConstants
    {
        public static float MenuBGWidth = Screen.width * 0.125f;
        public static float MenuBGHeight = Screen.height * 0.20f;
        public static float ButtonWidth = Screen.width * 0.1f;
        public static float ButtonHeight = Screen.height * 0.033f;
        public static float ButtonSpacing = (MenuBGWidth - ButtonWidth) / 2.0f;
    }
}