using UnityEngine;
using DG.Tweening;

public interface ILoadingUI : ISimpleUI
{
}

public class LoadingUI : CustomUI, ILoadingUI
{
    //[SerializeField]
    //private float fadeDuration = 0.35f;

    //public override void Init() {
    //    base.Init();

    //    // GameEngine.Instance.OnSceneLoadingStart += Show;
    //    //GameEngine.Instance.OnSceneLoadingEnd += Hide;
    //}

    public void Show() {
        canvasGroup.DOFade(1, fadeDuration).SetUpdate(true);
    }

    public void Hide() {
        canvasGroup.DOFade(0, fadeDuration).SetUpdate(true);
    }
}