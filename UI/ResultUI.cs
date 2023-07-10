using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public interface IResultUI : ISimpleUI
{
    public UniTask Init(int stageId, int outcome);
}

public class ResultUI : CustomUI, IResultUI
{
    private int stageId, outcome;

    [SerializeField]
    private TextMeshProUGUI resultText;

    public async UniTask Init(int stageId, int outcome) {
        this.stageId = stageId;
        this.outcome = outcome;

        resultText.text = outcome < 10 ? "Win!" : "Lose~";
    }

    public void RestartGame() {
        var stageUI = UIManager.Instance.GetUI<IStageUI>();

        stageUI.OpenStage(stageId);

        Close();
    }

    public void ShowHint() {
        var hintUI = UIManager.Instance.GetUI<IHintUI>();

        hintUI.Init(stageId);

        hintUI.Open();
    }

    public override void Back() {
        OpenStageUI().Forget();

        async UniTask OpenStageUI() {
            Close();

            var loadingUI = UIManager.Instance.GetUI<ILoadingUI>();

            loadingUI.Open();

            var stageUI = UIManager.Instance.GetUI<IStageUI>();

            await UniTask.Delay(UIUtils.MinLoadingDelayMilliseconds);

            stageUI.Open();

            loadingUI.Close();
        }
    }
}