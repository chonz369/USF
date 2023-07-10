using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public interface IStageUI : ISimpleUI
{
    UniTask Init();

    UniTask OpenStage(int stageId);
}

public class StageUI : CustomUI, IStageUI
{
    public UniTask Init() {
        return UniTask.CompletedTask;
    }

    public void OnClickStage(int stageId) {
        OpenStage(stageId).Forget();
    }

    public async UniTask OpenStage(int stageId) {
        Close();

        var loadingUI = UIManager.Instance.GetUI<ILoadingUI>();
        loadingUI.Open();

        var mainGameUI = UIManager.Instance.GetUI<IMainGameUI>();
        await mainGameUI.Init(stageId);

        await UniTask.Delay(UIUtils.MinLoadingDelayMilliseconds);

        mainGameUI.Open(this);
        loadingUI.Close();

        mainGameUI.StartGame();
    }
}