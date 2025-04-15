using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NJG.Utilities
{
    public static class DrawDebug
    {
        private const float _defaultIconSize = 0.1f;
        private static readonly Color _defaultIconColor = Color.magenta;


        public static void Lines(params Vector4[] lines)
        {
            Lines(null, lines);
        }

        public static void Lines(Color? lineColor, params Vector4[] lines)
        {
            if (lines == null) return;

            for (int i = 0; i < lines.Length; i++)
            {
                Vector4 line = lines[i];
                Debug.DrawLine(new Vector3(line.x, line.y), new Vector3(line.z, line.w),
                    lineColor ?? Color.HSVToRGB(((i * 1) % 360) / 360F, 1, 1));
            }
        }

        public static void Points(params Vector2[] points)
        {
            Points(_defaultIconSize, _defaultIconColor, points);
        }

        public static void Points(Color iconColor, params Vector2[] points)
        {
            Points(_defaultIconSize, iconColor, points);
        }

        public static void Points(float iconSize, params Vector2[] points)
        {
            Points(iconSize, _defaultIconColor, points);
        }

        public static void Points(float iconSize, Color iconColor, params Vector2[] points)
        {
            if (points == null) return;

            Vector2 bottomLeft = new (-iconSize, -iconSize);
            Vector2 bottomRight = new (iconSize, -iconSize);
            Vector2 topLeft = new (-iconSize, iconSize);
            Vector2 topRight = new (iconSize, iconSize);

            foreach (Vector2 point in points)
            {
                Debug.DrawLine(point + bottomLeft, point + topRight, iconColor);
                Debug.DrawLine(point + topLeft, point + bottomRight, iconColor);
            }
        }

        public static void Label(string text, Vector2 position)
        {
#if UNITY_EDITOR
            Color oldColor = GUI.color;
            GUI.color = Color.red;
            Handles.Label(position, text);
            GUI.color = oldColor;
#endif
        }
    }
}