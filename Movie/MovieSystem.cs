using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using USF.Core;

public class MovieSystem : MonoBehaviour, GameSystem
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    public static MovieSystem Instance { get; private set; }

    public bool IsPlaying => videoPlayer.isPlaying;

    public double CurrentPlayTime => videoPlayer.time;
    public double ClipLength => videoPlayer.length;

    public UniTask Init(GameEngine gameEngine) {
        Instance = this;

        return UniTask.CompletedTask;
    }

    public async UniTask PrepareVideo(VideoClip videoClip) {
        videoPlayer.clip = videoClip;

        videoPlayer.Prepare();

        await UniTask.WaitUntil(() => videoPlayer.isPrepared);
    }

    private CancellationTokenSource cts;

    public async UniTask PlayVideo(float fadeDuration = 0) {
        videoPlayer.Play();

        cts = new CancellationTokenSource();
        await UniTask.WaitWhile(() => videoPlayer.isPlaying
#if UNITY_EDITOR
            && !Keyboard.current.tabKey.isPressed
#endif
            , cancellationToken: cts.Token);

        videoPlayer.Stop();
    }

    public void StopVideo() {
        videoPlayer.Stop();
    }

    public void Release() {
        videoPlayer.targetTexture.Release();
    }

    private void OnDestroy() {
        if (cts != null) cts.Cancel();
    }
}