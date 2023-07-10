using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public interface ITitleUI : ISimpleUI
{
}

public class TitleUI : CustomUI, ITitleUI
{
    public void OnClikStartGame() {
        StartGame().Forget();
    }

    public async UniTask StartGame() {
        var stageUI = UIManager.Instance.GetUI<IStageUI>();
        var loadingUI = UIManager.Instance.GetUI<ILoadingUI>();

        Close();

        loadingUI.Open();

        await stageUI.Init();

        await UniTask.Delay(UIUtils.MinLoadingDelayMilliseconds);

        stageUI.Open();

        loadingUI.Close();
    }
}