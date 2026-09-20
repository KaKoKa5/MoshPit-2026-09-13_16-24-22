using Core.Utilities;
using UnityEditor;
using UnityEngine;

namespace Core.EditorTools
{
    public class GameMetrixWindow : EditorWindow
    {
        private GameMetrix _metrix;
        private SerializedObject _serializedMetrix;
        private Vector2 _scroll;

        private static readonly Color BackgroundColor = new Color(0.04f, 0.06f, 0.08f);
        private static readonly Color AccentCyan = new Color(0.13f, 0.85f, 1f);
        private static readonly Color AccentOrange = new Color(1f, 0.55f, 0.1f);
        private static readonly Color PanelColor = new Color(0.07f, 0.1f, 0.12f);

        private GUIStyle _titleStyle;
        private GUIStyle _sectionStyle;
        private GUIStyle _panelStyle;
        private GUIStyle _monoLabelStyle;
        private Texture2D _panelTexture;

        private double _lastRepaintTime;
        private float _pulse;

        [MenuItem("Tools/Mosh Pit/Game Metrix")]
        public static void Open()
        {
            var window = GetWindow<GameMetrixWindow>("◈ METRIX CORE");
            window.minSize = new Vector2(380, 320);
        }

        private void OnEnable()
        {
            LoadAsset();
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void LoadAsset()
        {
            string[] guids = AssetDatabase.FindAssets("t:GameMetrix");

            if (guids.Length == 0)
            {
                _metrix = null;
                _serializedMetrix = null;
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _metrix = AssetDatabase.LoadAssetAtPath<GameMetrix>(path);
            _serializedMetrix = new SerializedObject(_metrix);
        }

        private void OnEditorUpdate()
        {
            if (EditorApplication.timeSinceStartup - _lastRepaintTime < 0.05d)
                return;

            _lastRepaintTime = EditorApplication.timeSinceStartup;
            _pulse = (Mathf.Sin((float)EditorApplication.timeSinceStartup * 3f) + 1f) * 0.5f;
            Repaint();
        }

        private void InitStyles()
        {
            if (_panelTexture == null)
                _panelTexture = MakeTexture(PanelColor);

            _titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = AccentCyan }
            };

            _sectionStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 11,
                normal = { textColor = AccentOrange }
            };

            _monoLabelStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = new Color(0.6f, 0.9f, 1f) }
            };

            _panelStyle = new GUIStyle
            {
                normal = { background = _panelTexture },
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 4, 4)
            };
        }

        private static Texture2D MakeTexture(Color color)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        private void OnGUI()
        {
            InitStyles();
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), BackgroundColor);

            DrawHeader();
            DrawSeparator();
            EditorGUILayout.Space(6);

            if (_metrix == null)
            {
                EditorGUILayout.HelpBox("Aucun asset GameMetrix trouvé. Crée-le via Assets > Create > Mosh Pit > Game Metrix, dans un dossier 'Resources'.", MessageType.Warning);

                if (GUILayout.Button("Rechercher à nouveau"))
                    LoadAsset();

                return;
            }

            EditorGUILayout.LabelField("◈ VALEURS SYSTÈME", _sectionStyle);
            DrawSeparator(AccentOrange, 1);

            EditorGUILayout.BeginVertical(_panelStyle);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            _serializedMetrix.Update();

            SerializedProperty prop = _serializedMetrix.GetIterator();
            bool enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (prop.name == "m_Script")
                    continue;

                EditorGUILayout.PropertyField(prop, true);
            }

            // Sauvegarde en direct dès qu'une valeur change, pas besoin de bouton "Save"
            if (_serializedMetrix.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(_metrix);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField($"SYSTEM READY // {_metrix.name}", _monoLabelStyle);
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(6);
            Rect rect = EditorGUILayout.GetControlRect(false, 26);
            Color glow = Color.Lerp(AccentCyan, Color.white, _pulse * 0.4f);
            GUIStyle glowStyle = new GUIStyle(_titleStyle) { normal = { textColor = glow } };
            EditorGUI.LabelField(rect, "◈ METRIX CORE — J.A.R.V.I.S. PROTOCOL", glowStyle);
        }

        private void DrawSeparator(Color? color = null, float height = 2f)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawRect(rect, color ?? AccentCyan);
        }
    }
}