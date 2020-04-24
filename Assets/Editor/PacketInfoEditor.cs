using Engine.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// Editor window displaying packets and their properties
    /// </summary>
    public class PacketInfoEditor : EditorWindow
    {
        private static List<Packet> packets = new List<Packet>();

        /// <summary>
        /// Called when the window is open
        /// </summary>
        [MenuItem("Elavad/Networking/Packet Info")]
        public static void ShowWindow()
        {
            init();
            EditorWindow window = GetWindow<PacketInfoEditor>("Packet Info");
        }

        /// <summary>
        /// Called for when window opens and when scripts are reloaded
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void init()
        {
            packets.Clear();
            GetPackets();
        }

        /// <summary>
        /// Called for repaint
        /// </summary>
        private void OnGUI()
        {
            DrawPacketInfo<Packet>();
        }

        /// <summary>
        /// Find packets in assembly
        /// </summary>
        private static void GetPackets()
        {
            Type parentType = typeof(Packet);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));
                foreach (Type type in subclasses)
                {
                    Packet packet = (Packet)Activator.CreateInstance(type);
                    packets.Add(packet);
                }
            }

            //Sort
            packets.Sort((x, y) =>
                x.packetId.CompareTo(y.packetId));
        }

        /// <summary>
        /// Draw the table onto the UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void DrawPacketInfo<T>()
        {

            GUIStyle style = new GUIStyle();
            Texture2D originalBGColor = style.normal.background;
            EditorWindow window = GetWindow<PacketInfoEditor>("Packet Info");
            window.maxSize = new Vector2(650, 10000);
            window.minSize = new Vector2(0, 4 + (21 * packets.Count));
            GUILayout.BeginVertical(GUILayout.MaxHeight((20 * packets.Count) + 10));
            GUILayout.Space(5);
            bool dark = false;

            Texture2D BGText = new Texture2D(1, 1);
            BGText.wrapMode = TextureWrapMode.Repeat;
            Color color = new Color(180f / 255f, 180f / 255f, 180f / 255f);
            BGText.SetPixel(0, 0, color);
            BGText.Apply();

            Texture2D BGTextDark = new Texture2D(1, 1);
            BGTextDark.wrapMode = TextureWrapMode.Repeat;
            Color darkColor = new Color(160f / 255f, 160f / 255f, 160f / 255f);
            BGTextDark.SetPixel(0, 0, darkColor);
            BGTextDark.Apply();

            Texture2D BGTextDark2 = new Texture2D(1, 1);
            BGTextDark2.wrapMode = TextureWrapMode.Repeat;
            Color darkColor2 = new Color(120f / 255f, 120f / 255f, 120f / 255f);
            BGTextDark2.SetPixel(0, 0, darkColor2);
            BGTextDark2.Apply();

            style.richText = true;
            style.wordWrap = false;
            style.margin = new RectOffset(0, 0, 0, 0);
            style.padding = new RectOffset(0, 0, 0, 0);
            style.contentOffset = new Vector2(0, 0);

            for (int i = 0; i < packets.Count; i++)
            {
                Packet packet = packets[i];
                int packetId = packet.packetId;
                string name = packet.packetName;
                string target = packet.packetTarget;
                string reliability = packet.packetReliabilityScheme == ReliabilityScheme.RELIABLE ? "R" : "U";
                string description = packet.packetDescription;

                style.normal.background = !dark ? BGText : BGTextDark;
                GUILayout.BeginHorizontal(GUILayout.MinWidth(150), GUILayout.MaxWidth(650), GUILayout.ExpandHeight(false));
                GUILayout.Label(new GUIContent(" <size=13><b>" + packetId + ".</b></size> ", "Packet Index"), style, GUILayout.Width(30), GUILayout.ExpandHeight(false), GUILayout.Height(20), GUILayout.MinHeight(20), GUILayout.MaxHeight(20));
                GUILayout.Label(new GUIContent(" <size=13>" + name + "</size>", "Packet Name"), style, GUILayout.MinWidth(170), GUILayout.MaxWidth(170), GUILayout.MinHeight(20), GUILayout.Height(20), GUILayout.ExpandHeight(false));
                style.padding = new RectOffset(0, 0, 2, 0);
                GUILayout.Label(new GUIContent(target, "Packet Target"), style, GUILayout.MinWidth(50), GUILayout.MaxWidth(50), GUILayout.MinHeight(20), GUILayout.Height(20), GUILayout.ExpandHeight(false));
                GUILayout.Label(new GUIContent(reliability, "Reliability Scheme"), style, GUILayout.MinWidth(20), GUILayout.MaxWidth(20), GUILayout.MinHeight(20), GUILayout.Height(20), GUILayout.ExpandHeight(false));
                GUILayout.Label(new GUIContent(" <i>" + description + "</i>", "Packet Description"), style, GUILayout.MinWidth(240), GUILayout.MaxWidth(601), GUILayout.MinHeight(20), GUILayout.Height(20), GUILayout.ExpandHeight(false));
                style.padding = new RectOffset(0, 0, 0, 0);
                GUILayout.EndHorizontal();

                if (i != packets.Count - 1)
                {
                    GUILayout.Space(1);
                    GUI.DrawTexture(new Rect(0, 3 + ((i + 1) * 21), 549, 1), BGTextDark2);
                }

                dark = !dark;
            }
            style.normal.background = originalBGColor;
            GUILayout.Space(5);
            GUILayout.EndVertical();

        }

        /// <summary>
        /// Get packets in assembly
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns></returns>
        static IEnumerable<Type> GetTypesWithPacketAttribute(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(PacketAttribute), true).Length > 0)
                {
                    yield return type;
                }
            }
        }

    }
}