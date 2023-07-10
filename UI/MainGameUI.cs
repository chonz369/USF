using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public interface IMainGameUI : ISimpleUI
{
    UniTask Init(int stageId);

    void StartGame();
}

public class MainGameUI : CustomUI, IMainGameUI
{
    [SerializeField]
    private GameScene gamePrefab;

    private GameScene currentGame;

    private int stageId;

    public UniTask Init(int stageId) {
        Debug.Assert(currentGame == null);

        this.stageId = stageId;

        currentGame = Instantiate(gamePrefab, transform);
        currentGame.OnGameEnded += EndGame;
        return UniTask.CompletedTask;
    }

    public void StartGame() {
        currentGame.StartGame();
    }

    private void Update() {
        if (IsVisible && Input.GetKeyDown(KeyCode.Space)) {
            EndGame(0);
        }
    }

    public void EndGame(int outcome) {
        GameObject.Destroy(currentGame.gameObject);
        currentGame = null;
        Close();

        var resultUI = UIManager.Instance.GetUI<IResultUI>();
        resultUI.Init(stageId, outcome);
        resultUI.Open();
    }

    public override void Back() {
        Transition().Forget();
        GameObject.Destroy(currentGame.gameObject);
        currentGame = null;
        async UniTask Transition() {
            var loadingUI = UIManager.Instance.GetUI<ILoadingUI>();
            loadingUI.Open();

            await UniTask.Delay(UIUtils.MinLoadingDelayMilliseconds);

            base.Back();

            loadingUI.Close();
        }
    }

    public void Restart() {
    }

    public void ShowHint() {
    }
}