using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

public class GameScene : ISceneState
{
    public System.Action<int> OnGameEnded;

    public void StartGame() {
    }

    public void NotifyGameEnded(int resultId) {
        OnGameEnded?.Invoke(resultId);
    }

    //private const string path = "Assets/Prefabs/GameScene";
    //private GameSceneData gameSceneData;

    //private DefaultControls action;

    //private IGame currentGame;

    //public override async UniTask Init() {
    //    await base.Init();

    //    var handle = Addressables.InstantiateAsync(path);
    //    await handle;

    //    gameSceneData = handle.Result.GetComponent<GameSceneData>();
    //    gameSceneData.Canvas.worldCamera = Camera.main;

    //    //AudioManager.Instance.PlayBgm(gameSceneData.BgmName).Forget();

    //    action = new DefaultControls();
    //    action.GameControl.Skip.performed += Skip;
    //    action.Enable();

    //    CameraManager.Instance.PushCamera(gameSceneData.RockCamera);

    //    SwitchTo(new RollingGame());
    //}

    //public override void Update() {
    //    base.Update();

    //    currentGame?.Update();
    //}

    //public void SwitchTo(IGame game) {
    //    currentGame?.Shutdown();
    //    currentGame = game;
    //    currentGame.Init(this, gameSceneData);
    //}

    //private void Skip(InputAction.CallbackContext obj) {
    //    //GameEngine.Instance.SwitchTo(new TitleScene()).Forget();
    //}

    //public override void Shutdown() {
    //    CameraManager.Instance.PopCamera();

    //    action.Dispose();

    //    currentGame?.Shutdown();
    //    currentGame = null;

    //    if (gameSceneData != null) {
    //        //Addressables.ReleaseInstance(gameSceneData.gameObject);
    //    }
    //}

    //public void EndGame() {
    //    currentGame?.Shutdown();
    //    currentGame = null;

    //    //GameEngine.Instance.SwitchTo(new TitleScene()).Forget();
    //}

    //public interface IGame
    //{
    //    UniTask Init(GameScene manager, GameSceneData data);

    //    void Update();

    //    void Shutdown();
    //}

    //public class RollingGame : IGame
    //{
    //    private GameSceneData data;
    //    private GameScene manager;
    //    private float currentDistance;

    //    public System.Action<float> OnRollingProgressChanged;
    //    public System.Action<int> OnTriggerPuzzle;

    //    private int currentPuzzleIndex;
    //    private bool pauseProgress;

    //    public async UniTask Init(GameScene manager, GameSceneData data) {
    //        this.data = data;
    //        this.manager = manager;

    //        currentDistance = 0;
    //        currentPuzzleIndex = 0;
    //        pauseProgress = false;

    //        //var rollingGameUI = await UIManager.Instance.LoadUIAsync<RollingGameUI>("RollingGameUI");
    //        //rollingGameUI.Init(this);
    //    }

    //    public void Shutdown() {
    //        UIManager.Instance.RemoveUI("RollingGameUI");
    //        //AudioManager.Instance.StopBgm();

    //        Time.timeScale = 1;
    //    }

    //    public void ContinueRollingProgress() {
    //        pauseProgress = false;
    //    }

    //    public void Update() {
    //        if (pauseProgress) return;

    //        currentDistance += Time.deltaTime * data.RollingDistancePerSecond;

    //        OnRollingProgressChanged?.Invoke(currentDistance / data.RollingMaxDistance);

    //        if (currentDistance >= (currentPuzzleIndex + 1) * data.TriggerRollingGameDistance) {
    //            pauseProgress = true;
    //            OnTriggerPuzzle?.Invoke(currentPuzzleIndex);
    //            currentPuzzleIndex++;
    //        }

    //        if (currentDistance >= data.RollingMaxDistance) {
    //            manager.EndGame();
    //        }
    //    }
    //}
}