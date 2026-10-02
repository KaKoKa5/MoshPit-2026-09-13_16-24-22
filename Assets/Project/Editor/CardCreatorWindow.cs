using Core.DeckSysteme;
using GamePlay.Card;
using UnityEditor;
using UnityEngine;

namespace Core.EditorTools
{
    public class CardCreatorWindow : EditorWindow
    {
        private enum Mode { Card, Combo }
        private Mode mode = Mode.Card;

        // --- Carte ---
        private CardInfoData editableCard;
        private SerializedObject serializedCard;
        private MonoBehaviour targetDeckOwner;
        private string saveFolder = "Assets/Project/Data/Cards";

        // --- Combo ---
        private ComboData editableCombo;
        private SerializedObject serializedCombo;
        private ComboLibrary targetComboLibrary;
        private string comboSaveFolder = "Assets/Project/Data/Combos";

        private Vector2 scroll;

        // --- Thème JARVIS ---
        private static readonly Color BackgroundColor = new Color(0.04f, 0.06f, 0.08f);
        private static readonly Color AccentCyan = new Color(0.13f, 0.85f, 1f);
        private static readonly Color AccentOrange = new Color(1f, 0.55f, 0.1f);
        private static readonly Color PanelColor = new Color(0.07f, 0.1f, 0.12f);

        private GUIStyle titleStyle;
        private GUIStyle sectionStyle;
        private GUIStyle panelStyle;
        private GUIStyle monoLabelStyle;
        private GUIStyle buttonStyle;
        private Texture2D panelTexture;

        private double lastRepaintTime;
        private float pulse;

        [MenuItem("Tools/Mosh Pit/Card Creator")]
        public static void Open()
        {
            var window = GetWindow<CardCreatorWindow>("◈ CARD FORGE");
            window.minSize = new Vector2(420, 480);
        }

        private void OnEnable()
        {
            CreateNewCardInstance();
            CreateNewComboInstance();
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (EditorApplication.timeSinceStartup - lastRepaintTime < 0.05d)
                return;

            lastRepaintTime = EditorApplication.timeSinceStartup;
            pulse = (Mathf.Sin((float)EditorApplication.timeSinceStartup * 3f) + 1f) * 0.5f;
            Repaint();
        }

        private void CreateNewCardInstance()
        {
            editableCard = CreateInstance<CardInfoData>();
            serializedCard = new SerializedObject(editableCard);
        }

        private void CreateNewComboInstance()
        {
            editableCombo = CreateInstance<ComboData>();
            serializedCombo = new SerializedObject(editableCombo);
        }

        private void InitStyles()
        {
            if (panelTexture == null)
                panelTexture = MakeTexture(PanelColor);

            titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = AccentCyan }
            };

            sectionStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 11,
                normal = { textColor = AccentOrange }
            };

            monoLabelStyle = new GUIStyle(EditorStyles.label)
            {
                font = EditorStyles.miniLabel.font,
                normal = { textColor = new Color(0.6f, 0.9f, 1f) }
            };

            panelStyle = new GUIStyle
            {
                normal = { background = panelTexture },
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 4, 4)
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
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
            if (editableCard == null || serializedCard == null || serializedCard.targetObject == null)
                CreateNewCardInstance();

            if (editableCombo == null || serializedCombo == null || serializedCombo.targetObject == null)
                CreateNewComboInstance();

            InitStyles();

            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), BackgroundColor);

            DrawHeader();
            DrawSeparator();
            EditorGUILayout.Space(6);

            mode = (Mode)GUILayout.Toolbar((int)mode, new[] { "◈ CARTE", "◈ COMBO" });
            EditorGUILayout.Space(8);

            if (mode == Mode.Card)
                DrawCardMode();
            else
                DrawComboMode();
        }

        // ============== MODE CARTE ==============

        private void DrawCardMode()
        {
            EditorGUILayout.HelpBox("Cible un DeckManager (deck joueur) ou un EnemyController (deck ennemi).", MessageType.None);

            Object newTarget = EditorGUILayout.ObjectField(
                new GUIContent("⛓ DECK CIBLE"), targetDeckOwner, typeof(MonoBehaviour), true);

            targetDeckOwner = ValidateDeckTarget(newTarget);

            saveFolder = EditorGUILayout.TextField("📁 DOSSIER", saveFolder);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("◈ DONNÉES DE LA CARTE", sectionStyle);
            DrawSeparator(AccentOrange, 1);

            EditorGUILayout.BeginVertical(panelStyle);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            serializedCard.Update();
            DrawAllProperties(serializedCard);
            serializedCard.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = AccentCyan;

            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(saveFolder)))
            {
                if (GUILayout.Button("⚡ FORGER LA CARTE", buttonStyle))
                    CreateAndAssignCard();
            }

            GUI.backgroundColor = prevColor;

            EditorGUILayout.Space(4);
            string status = targetDeckOwner != null ? $"TARGET LOCKED // {targetDeckOwner.GetType().Name}" : "NO TARGET";
            EditorGUILayout.LabelField($"SYSTEM READY // {status}", monoLabelStyle);
        }

        private static MonoBehaviour ValidateDeckTarget(Object candidate)
        {
            if (candidate == null)
                return null;

            var behaviour = candidate as MonoBehaviour;

            if (behaviour == null)
                return null;

            SerializedObject so = new SerializedObject(behaviour);
            SerializedProperty deckProp = so.FindProperty("masterDeck");

            if (deckProp == null)
            {
                Debug.LogWarning($"[Card Forge] {behaviour.GetType().Name} n'a pas de champ 'masterDeck', cible ignorée.");
                return null;
            }

            return behaviour;
        }

        private void CreateAndAssignCard()
        {
            EnsureFolderExists(saveFolder);

            string fileName = string.IsNullOrEmpty(editableCard.cardName) ? "NewCard" : editableCard.cardName;
            string path = AssetDatabase.GenerateUniqueAssetPath($"{saveFolder}/{fileName}.asset");

            AssetDatabase.CreateAsset(editableCard, path);
            AssetDatabase.SaveAssets();

            if (targetDeckOwner != null)
                AddCardToDeck(targetDeckOwner, editableCard);
            else
                Debug.LogWarning("Aucune cible assignée : la carte a été créée mais pas ajoutée à un deck.");

            CreateNewCardInstance();
            Repaint();
        }

        private void AddCardToDeck(MonoBehaviour owner, CardInfoData card)
        {
            SerializedObject so = new SerializedObject(owner);
            SerializedProperty masterDeckProp = so.FindProperty("masterDeck");

            if (masterDeckProp == null)
            {
                Debug.LogError($"Champ masterDeck introuvable sur {owner.GetType().Name}.");
                return;
            }

            masterDeckProp.arraySize++;
            masterDeckProp.GetArrayElementAtIndex(masterDeckProp.arraySize - 1).objectReferenceValue = card;
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(owner);
        }

        // ============== MODE COMBO ==============

        private void DrawComboMode()
        {
            EditorGUILayout.HelpBox("Cible une ComboLibrary pour y ajouter le combo créé.", MessageType.None);

            targetComboLibrary = (ComboLibrary)EditorGUILayout.ObjectField(
                new GUIContent("⛓ LIBRAIRIE CIBLE"), targetComboLibrary, typeof(ComboLibrary), true);

            comboSaveFolder = EditorGUILayout.TextField("📁 DOSSIER", comboSaveFolder);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("◈ DONNÉES DU COMBO", sectionStyle);
            DrawSeparator(AccentOrange, 1);

            EditorGUILayout.BeginVertical(panelStyle);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            serializedCombo.Update();
            DrawAllProperties(serializedCombo);
            serializedCombo.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = AccentCyan;

            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(comboSaveFolder)))
            {
                if (GUILayout.Button("⚡ FORGER LE COMBO", buttonStyle))
                    CreateAndAssignCombo();
            }

            GUI.backgroundColor = prevColor;

            EditorGUILayout.Space(4);
            string status = targetComboLibrary != null ? "TARGET LOCKED // ComboLibrary" : "NO TARGET";
            EditorGUILayout.LabelField($"SYSTEM READY // {status}", monoLabelStyle);
        }

        private void CreateAndAssignCombo()
        {
            EnsureFolderExists(comboSaveFolder);

            string fileName = string.IsNullOrEmpty(editableCombo.ComboName) ? "NewCombo" : editableCombo.ComboName;
            string path = AssetDatabase.GenerateUniqueAssetPath($"{comboSaveFolder}/{fileName}.asset");

            AssetDatabase.CreateAsset(editableCombo, path);
            AssetDatabase.SaveAssets();

            if (targetComboLibrary != null)
                AddComboToLibrary(targetComboLibrary, editableCombo);
            else
                Debug.LogWarning("Aucune librairie assignée : le combo a été créé mais pas ajouté.");

            CreateNewComboInstance();
            Repaint();
        }

        private void AddComboToLibrary(ComboLibrary library, ComboData combo)
        {
            SerializedObject so = new SerializedObject(library);
            SerializedProperty combosProp = so.FindProperty("combos");

            if (combosProp == null)
            {
                Debug.LogError("Champ 'combos' introuvable sur ComboLibrary.");
                return;
            }

            combosProp.arraySize++;
            combosProp.GetArrayElementAtIndex(combosProp.arraySize - 1).objectReferenceValue = combo;
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(library);
        }

        // ============== COMMUN ==============

        private static void DrawAllProperties(SerializedObject so)
        {
            SerializedProperty prop = so.GetIterator();
            bool enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (prop.name == "m_Script")
                    continue;

                EditorGUILayout.PropertyField(prop, true);
            }
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(6);

            Rect rect = EditorGUILayout.GetControlRect(false, 26);
            Color glow = Color.Lerp(AccentCyan, Color.white, pulse * 0.4f);

            GUIStyle glowStyle = new GUIStyle(titleStyle) { normal = { textColor = glow } };
            EditorGUI.LabelField(rect, "◈ CARD FORGE — J.A.R.V.I.S. PROTOCOL", glowStyle);
        }

        private void DrawSeparator(Color? color = null, float height = 2f)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawRect(rect, color ?? AccentCyan);
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
                    AssetDatabase.CreateFolder(currentPath, parts[i]);

                currentPath = nextPath;
            }
        }
    }
}