using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using USF.Utils;

namespace USF.UI.Utils
{
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
            public AudioClip clip;
            public bool allowSkip = false;
        }

        [SerializeField]
        private FadeTextureInfo[] fadeTextures;

        public bool IsPlaying { get { return isPlaying; } }
        private bool isPlaying;

        private CompositeDisposable coPlayCompositeDisposable;

        [SerializeField]
        private bool debugSkipAll;

        public void Play() {
            StartCoroutine(CoPlay());
        }

        private IEnumerator CoPlay() {
            if (debugSkipAll) {
                yield break;
            }

            isPlaying = true;

            coPlayCompositeDisposable = new CompositeDisposable();

            var eventTrigger = this.gameObject.AddComponent<ObservableEventTrigger>();

            var isInput = false;

            Observable.EveryLateUpdate().Subscribe(x => isInput = false).AddTo(coPlayCompositeDisposable);

            eventTrigger.OnPointerClickAsObservable().Subscribe(x => isInput = true).AddTo(coPlayCompositeDisposable);

            RawImage rawImage = GetComponent<RawImage>();
            rawImage.enabled = true;
            rawImage.color = ColorUtils.ClearWhite;

            foreach (FadeTextureInfo info in fadeTextures) {
                rawImage.texture = info.texture;

                if (!string.IsNullOrEmpty(info.videoPath)) {
                    var handle = Addressables.LoadAssetAsync<VideoClip>(info.videoPath);

                    yield return handle;

                    var video = handle.Result;

                    yield return UniTask.ToCoroutine(() => MovieSystem.Instance.PrepareVideo(video));

                    rawImage.DOFade(1, info.fadeInTime);

                    MovieSystem.Instance.PlayVideo().Forget();

                    while (MovieSystem.Instance.IsPlaying) {
                        yield return null;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        if (Keyboard.current != null && Keyboard.current.tabKey.isPressed) {
                            MovieSystem.Instance.StopVideo();
                        }
#endif
                        if (MovieSystem.Instance.ClipLength - MovieSystem.Instance.CurrentPlayTime < info.fadeOutTime) {
                            yield return rawImage.DOFade(0, info.fadeOutTime).WaitForCompletion();

                            MovieSystem.Instance.StopVideo();
                        } else if (isInput && info.allowSkip) {
                            yield return rawImage.DOFade(0, info.fadeOutTime).WaitForCompletion();

                            MovieSystem.Instance.StopVideo();
                        }
                    }

                    MovieSystem.Instance.StopVideo();

                    MovieSystem.Instance.Release();

                    Addressables.Release(handle);
                } else if (info.texture) {
                    yield return rawImage.DOFade(1, info.fadeInTime).WaitForCompletion();

                    if (info.clip != null) {
                        AudioSystem.Instance.PlayBgm(info.clip);
                    }

                    float time = 0;

                    time = 0;
                    while (!isInput || !info.allowSkip) {
                        yield return null;
                        time += Time.deltaTime;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        if (Keyboard.current != null && Keyboard.current.tabKey.isPressed) {
                            time = info.duration;
                        }
#endif

                        if (time >= info.duration) break;
                    }

                    yield return rawImage.DOFade(0, info.fadeInTime).WaitForCompletion();
                }
            }
            isPlaying = false;

            coPlayCompositeDisposable.Dispose();
            coPlayCompositeDisposable = null;
            Destroy(eventTrigger);
            rawImage.enabled = false;
        }

        private void OnDestroy() {
            if (coPlayCompositeDisposable != null) {
                coPlayCompositeDisposable.Dispose();
            }
        }
    }
}