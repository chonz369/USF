using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

public class MovieManager : IGameSystem
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    [SerializeField]
    private RawImage renderImage;

    public static MovieManager Instance { get; private set; }

    public override UniTask Init() {
        Instance = this;

        return UniTask.CompletedTask;
    }

    public async UniTask PrepareVideo(VideoClip videoClip) {
        videoPlayer.clip = videoClip;

        videoPlayer.Prepare();

        await UniTask.WaitUntil(() => videoPlayer.isPrepared);
    }

    public async UniTask PlayVideo(float fadeDuration = 0) {
        renderImage.color = ColorUtils.ClearWhite;

        renderImage.enabled = true;

        videoPlayer.Play();

        renderImage.DOFade(1, fadeDuration).From(0);

        await UniTask.WaitWhile(() => videoPlayer.isPlaying
#if UNITY_EDITOR
            && !Input.GetKey(KeyCode.Tab)
#endif
            );

        videoPlayer.Stop();

        renderImage.enabled = false;
    }
}