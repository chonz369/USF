using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class MainToolkitWindow : EditorWindow
{
    private GameSceneRegistry gameSceneRegistry
        => _gameSceneRegistry ?? (_gameSceneRegistry = Resources.Load<GameSceneRegistry>("Registries/GameSceneRegistry"));

    private GameSceneRegistry _gameSceneRegistry;

    [MenuItem("Tools/MainToolkit")]
    public static void ShowWindow() {
        GetWindow<MainToolkitWindow>("MainToolkit");
    }

    private int tab = 0;

    private GameSceneManager gameSceneManager =>
        _gameSceneManager ?? (_gameSceneManager = FindObjectOfType<GameSceneManager>());

    private GameSceneManager _gameSceneManager;

    private bool isPlayMode;
    private string currentSceneName;
    private int currentSceneStage;

    private void OnGUI() {
        tab = GUILayout.Toolbar(tab, new string[] { "GameFlow" });

        isPlayMode = Application.isPlaying;

        switch (tab) {
            case 0:
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            currentSceneName = gameSceneManager.CurrentScene == null ? "None" : gameSceneManager.CurrentScene.name;
            currentSceneStage = gameSceneManager.CurrentScene == null ? 0 : gameSceneManager.CurrentScene.Stage;
            GUILayout.Label($"Transition To Scenes", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            GUILayout.Label($"Current Scene : {currentSceneName}");

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Current Stage : {currentSceneStage}");
            if (GUILayout.Button("Next Stage")) {
                _gameSceneManager.NextStage();
            }
            GUILayout.EndHorizontal();

            //GUILayout.BeginVertical();
            for (int i = 0; i < gameSceneRegistry.GameScenes.Length; i++) {
                if (gameSceneRegistry.GameScenes[i] != null) {
                    if (GUILayout.Button(gameSceneRegistry.GameScenes[i].name)) {
                        RunGameScene(i).Forget();
                    }
                }
            }
            //GUILayout.EndVertical();

            EditorGUILayout.EndVertical();
            break;
        }
    }

    private async UniTask RunGameScene(int index) {
        if (!isPlayMode) return;

        await gameSceneManager.LoadGameScene(index);
        gameSceneManager.StartGameScene();
    }

    //public override void OnInspectorGUI() {
    //    base.OnInspectorGUI();
    //    var t = target as ISceneState;
    //    if (GUILayout.Button("Start Scene")) {
    //        t.Init().Forget();
    //        t.StartScene();
    //    }
    //}

    private GameObject current;

    private void OnFocus() {
        current = Selection.activeGameObject;
    }

    private void OnLostFocus() {
        current = null;
    }
}