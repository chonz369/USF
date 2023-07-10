using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.AddressableAssets;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityAtoms.BaseAtoms;
using UnityAtoms;

[RequireComponent(typeof(RawImage))]
public class FadeTextureStream : MonoBehaviour
{
    [System.Serializable]
    public class FadeTextureInfo
    {
        public Texture texture;
        public string videoPath;
        public float fadeInTime = 0.5f;
        public float duration = 3.0f;
        public float fadeOutTime = 0.5f;
        public AudioClipVariable clipVariable;
        public bool allowSkip = false;
    }

    [SerializeField]
    private FadeTextureInfo[] fadeTextures;

    public bool IsPlaying { get { return isPlaying; } }
    private bool isPlaying;

    [SerializeField]
    private VideoPlayer videoPlayer;

    private CompositeDisposable compositeDisposable;

    [SerializeField]
    private AudioClipEvent playAudioEvent;

    [SerializeField]
    private bool debugSkipAll;

    private void Awake() {
        RawImage rawImage = GetComponent<RawImage>();
        rawImage.enabled = false;
    }

    public void Play() {
        StartCoroutine(CoPlay());
    }

    private IEnumerator CoPlay() {
        if (debugSkipAll) {
            yield break;
        }

        isPlaying = true;

        compositeDisposable = new CompositeDisposable();

        var eventTrigger = this.gameObject.AddComponent<ObservableEventTrigger>();

        var isInput = false;

        Observable.EveryLateUpdate().Subscribe(x => isInput = false).AddTo(compositeDisposable);

        eventTrigger.OnPointerClickAsObservable().Subscribe(x => isInput = true).AddTo(compositeDisposable);

        RawImage rawImage = GetComponent<RawImage>();
        rawImage.enabled = true;
        rawImage.color = ColorUtils.ClearWhite;

        foreach (FadeTextureInfo info in fadeTextures) {
            rawImage.texture = info.texture;

            if (!string.IsNullOrEmpty(info.videoPath)) {
                var handle = Addressables.LoadAssetAsync<VideoClip>(info.videoPath);

                yield return handle;

                var video = handle.Result;

                videoPlayer.clip = video;
                videoPlayer.Prepare();

                yield return new WaitUntil(() => videoPlayer.isPrepared);

                rawImage.DOFade(1, info.fadeInTime);

                videoPlayer.Play();

                while (videoPlayer.isPlaying) {
                    yield return null;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    if (Keyboard.current != null && Keyboard.current.tabKey.isPressed) {
                        videoPlayer.Stop();
                    }
#endif
                    if (videoPlayer.clip.length - videoPlayer.time < info.fadeOutTime) {
                        yield return rawImage.DOFade(0, info.fadeOutTime).WaitForCompletion();

                        videoPlayer.Stop();
                    } else if (isInput && info.allowSkip) {
                        yield return rawImage.DOFade(0, info.fadeOutTime).WaitForCompletion();

                        videoPlayer.Stop();
                    }
                }

                videoPlayer.Stop();

                videoPlayer.targetTexture.Release();

                Addressables.Release(handle);
            } else if (info.texture) {
                yield return rawImage.DOFade(1, info.fadeInTime).WaitForCompletion();

                if (info.clipVariable != null) {
                    playAudioEvent.Raise(info.clipVariable.Value);
                }

                float time = 0;

                time = 0;
                while (!isInput || !info.allowSkip) {
                    yield return null;
                    time += Time.deltaTime;
                    if (time > info.duration) break;
                }

                yield return rawImage.DOFade(0, info.fadeInTime).WaitForCompletion();
            }
        }
        isPlaying = false;

        Destroy(eventTrigger);
        compositeDisposable.Dispose();
        compositeDisposable = null;

        rawImage.enabled = false;
    }

    private void OnDestroy() {
        if (compositeDisposable != null) {
            compositeDisposable.Dispose();
        }
    }
}