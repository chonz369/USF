using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameSceneManager : IGameSystem
{
    private Transform gameSceneRoot;

    public static GameSceneManager Instance { get; private set; }

    public ISceneState CurrentScene { get; private set; }

    [SerializeField]
    private GameSceneRegistry gameSceneRegistry;

    public override UniTask Init() {
        Instance = this;

        gameSceneRoot = FindObjectOfType<GameRoot>()?.transform;
        if (gameSceneRoot == null) {
            var g = new GameObject();
            g.name = "GameRoot";
            g.AddComponent<GameRoot>();
            gameSceneRoot = g.transform;
        }

        return UniTask.CompletedTask;
    }

    public async UniTask LoadGameScene(int index) {
        var gameScenePrefab = gameSceneRegistry.GameScenes[index];

        var gameScene = Instantiate(gameScenePrefab, gameSceneRoot);

        await gameScene.Init();

        if (CurrentScene != null) {
            CurrentScene.Shutdown();
        }
        CurrentScene = gameScene;
    }

    public void StartGameScene() {
        CurrentScene.StartScene();
    }

    public async UniTask StartGameScene(ISceneState gameScene) {
        if (CurrentScene != null) {
            CurrentScene.Shutdown();
        }
        await gameScene.Init();
        CurrentScene = gameScene;

        StartGameScene();
    }

    public void NextStage() {
        if (CurrentScene == null) return;
    }
}