using UnityEngine;
using System.Collections;

namespace KopyKat
{
    public class textcolor : MonoBehaviour
    {
        private static bool Initialized;
        private static Color textColor;

        // Use this for initialization
        void Start()
        {
            if (!Initialized)
            {
                textColor = (Resources.Load("text") as Material).color;
                Initialized = true;
            }

            guiText.material.color = textColor;
        }
    }
}