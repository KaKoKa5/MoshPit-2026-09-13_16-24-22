using System.IO;
using Core.DeckSysteme;
using GamePlay.Card;
using UnityEditor;
using UnityEngine;

namespace Core.EditorTools
{
    public class CardCreatorWindow : EditorWindow
    {
        private CardInfoData _editableCard;
        private SerializedObject _serializedCard;
        private DeckManager _targetDeckManager;
        private string _saveFolder = "Assets/Project/Data/Cards";
        private Vector2 _scroll;

        // --- Thème JARVIS ---
        private static readonly Color BackgroundColor = new Color(0.04f, 0.06f, 0.08f);
        private static readonly Color AccentCyan = new Color(0.13f, 0.85f, 1f);
        private static readonly Color AccentOrange = new Color(1f, 0.55f, 0.1f);
        private static readonly Color PanelColor = new Color(0.07f, 0.1f, 0.12f);

        private GUIStyle _titleStyle;
        private GUIStyle _sectionStyle;
        private GUIStyle _panelStyle;
        private GUIStyle _monoLabelStyle;
        private GUIStyle _buttonStyle;
        private Texture2D _panelTexture;
        private Texture2D _lineTexture;

        private double _lastRepaintTime;
        private float _pulse;

        [MenuItem("Tools/Mosh Pit/Card Creator")]
        public static void Open()
        {
            var window = GetWindow<CardCreatorWindow>("◈ CARD FORGE");
            window.minSize = new Vector2(420, 480);
        }

        private void OnEnable()
        {
            CreateNewCardInstance();
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            // Pulse lent façon arc reactor, ne redessine que ~20x/sec pour rester léger
            if (EditorApplication.timeSinceStartup - _lastRepaintTime < 0.05d)
                return;

            _lastRepaintTime = EditorApplication.timeSinceStartup;
            _pulse = (Mathf.Sin((float)EditorApplication.timeSinceStartup * 3f) + 1f) * 0.5f;
            Repaint();
        }

        private void CreateNewCardInstance()
        {
            _editableCard = CreateInstance<CardInfoData>();
            _serializedCard = new SerializedObject(_editableCard);
        }

        private void InitStyles()
        {
            if (_panelTexture == null)
                _panelTexture = MakeTexture(PanelColor);

            if (_lineTexture == null)
                _lineTexture = MakeTexture(AccentCyan);

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
                font = EditorStyles.miniLabel.font,
                normal = { textColor = new Color(0.6f, 0.9f, 1f) }
            };

            _panelStyle = new GUIStyle
            {
                normal = { background = _panelTexture },
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 4, 4)
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 32
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

            // Fond global
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), BackgroundColor);

            DrawHeader();
            DrawSeparator();

            EditorGUILayout.Space(4);

            _targetDeckManager = (DeckManager)EditorGUILayout.ObjectField(
                new GUIContent("⛓ DECK MANAGER CIBLE"), _targetDeckManager, typeof(DeckManager), true);

            _saveFolder = EditorGUILayout.TextField("📁 DOSSIER", _saveFolder);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("◈ DONNÉES DE LA CARTE", _sectionStyle);
            DrawSeparator(AccentOrange, 1);

            EditorGUILayout.BeginVertical(_panelStyle);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            _serializedCard.Update();

            SerializedProperty prop = _serializedCard.GetIterator();
            bool enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (prop.name == "m_Script")
                    continue;

                EditorGUILayout.PropertyField(prop, true);
            }

            _serializedCard.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = AccentCyan;

            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(_saveFolder)))
            {
                if (GUILayout.Button("⚡ FORGER LA CARTE", _buttonStyle))
                {
                    CreateAndAssign();
                }
            }

            GUI.backgroundColor = prevColor;

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField($"SYSTEM READY // {(_targetDeckManager != null ? "TARGET LOCKED" : "NO TARGET")}", _monoLabelStyle);
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(6);

            Rect rect = EditorGUILayout.GetControlRect(false, 26);
            Color glow = Color.Lerp(AccentCyan, Color.white, _pulse * 0.4f);

            GUIStyle glowStyle = new GUIStyle(_titleStyle) { normal = { textColor = glow } };
            EditorGUI.LabelField(rect, "◈ CARD FORGE — J.A.R.V.I.S. PROTOCOL", glowStyle);
        }

        private void DrawSeparator(Color? color = null, float height = 2f)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawRect(rect, color ?? AccentCyan);
        }

        private void CreateAndAssign()
        {
	        EnsureFolderExists(_saveFolder);

	        string fileName = string.IsNullOrEmpty(_editableCard.cardName) ? "NewCard" : _editableCard.cardName;
	        string path = AssetDatabase.GenerateUniqueAssetPath($"{_saveFolder}/{fileName}.asset");

	        AssetDatabase.CreateAsset(_editableCard, path);
	        AssetDatabase.SaveAssets();

	        if (_targetDeckManager != null)
	        {
		        AddCardToDeckManager(_targetDeckManager, _editableCard);
	        }
	        else
	        {
		        Debug.LogWarning("Aucun Deck Manager assigné : la carte a été créée mais pas ajoutée à un deck.");
	        }

	        CreateNewCardInstance();
	        Repaint();
        }

        private void AddCardToDeckManager(DeckManager deckManager, CardInfoData card)
        {
            SerializedObject so = new SerializedObject(deckManager);
            SerializedProperty masterDeckProp = so.FindProperty("<MasterDeck>k__BackingField");

            if (masterDeckProp == null)
            {
                Debug.LogError("Champ MasterDeck introuvable sur DeckManager (vérifie le nom de la propriété).");
                return;
            }

            masterDeckProp.arraySize++;
            masterDeckProp.GetArrayElementAtIndex(masterDeckProp.arraySize - 1).objectReferenceValue = card;
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(deckManager);
        }

        private static void EnsureFolderExists(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
                return;

            string[] parts = folderPath.Split('/');
            string currentPath = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string nextPath = $"{currentPath}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(nextPath))
                {
                    AssetDatabase.CreateFolder(currentPath, parts[i]);
                }
                currentPath = nextPath;
            }
        }
    }
}