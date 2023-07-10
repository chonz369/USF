using Cysharp.Threading.Tasks;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.AddressableAssets;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioManager : IGameSystem
{
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    private AudioSource bgmAudioSource, seAudioSource, ambienceAudioSource;

    [SerializeField]
    private AudioConfiguration audioConfiguration;

    private AssetLoader<AudioClip> audioLoader;

    public float MasterVolume {
        get {
            return masterVolume;
        }
        set {
            audioMixer.SetFloat("MasterVolume", Mathf.Clamp(Mathf.Log(Mathf.Max(0.001f, value)) * 20, -80, 0));
            masterVolume = value;
        }
    }

    private float masterVolume;

    public float SeVolume {
        get {
            return seVolume.Value;
        }
        set {
            audioMixer.SetFloat("SeVolume", Mathf.Clamp(Mathf.Log(Mathf.Max(0.001f, value)) * 20, -80, 0));
            PlayerPrefs.SetFloat("SeVolume", value);
            seVolume.Value = value;
        }
    }

    public float BgmVolume {
        get => bgmVolume.Value;
        set {
            audioMixer.SetFloat("BgmVolume", Mathf.Clamp(Mathf.Log(Mathf.Max(0.001f, value)) * 20, -80, 0));
            PlayerPrefs.SetFloat("BgmVolume", value);
            bgmVolume.Value = value;
        }
    }

    [SerializeField]
    private FloatVariable bgmVolume, seVolume;

    [SerializeField]
    private AudioMixer audioMixer;

    public override UniTask Init() {
        Instance = this;

        audioLoader = new AssetLoader<AudioClip>();

        LoadSavedSettings();

        return UniTask.CompletedTask;
    }

    private void LoadSavedSettings() {
        BgmVolume = PlayerPrefs.GetFloat("BgmVolume", 1);
        SeVolume = PlayerPrefs.GetFloat("SeVolume", 1);
    }

    public async UniTask PlayAmbienceAsync(string ambienceName, float? fadeDuration = null) {
        if (fadeDuration == null) {
            fadeDuration = audioConfiguration.DefaultFadeDuration;
        }

        var clip = await audioLoader.LoadAndHoldAsync(audioConfiguration.GetAmbiencePath(ambienceName), this);

        ambienceAudioSource.DOKill();
        ambienceAudioSource.clip = clip;
        ambienceAudioSource.loop = true;

        ambienceAudioSource.DOFade(1, fadeDuration.Value).From(0);
        ambienceAudioSource.Play();
    }

    public async UniTask PlayBgmAsync(string bgmName, float? fadeDuration = null) {
        if (fadeDuration == null) {
            fadeDuration = audioConfiguration.DefaultFadeDuration;
        }

        var clip = await audioLoader.LoadAndHoldAsync(audioConfiguration.GetBgmPath(bgmName), this);

        PlayBgm(clip, fadeDuration.Value);
    }

    public async UniTask StopBgmAsync(string bgmName, float? fadeDuration = null) {
        if (fadeDuration == null) {
            fadeDuration = audioConfiguration.DefaultFadeDuration;
        }

        bgmAudioSource.DOKill();
        bgmAudioSource.clip = null;

        bgmAudioSource.DOFade(0, fadeDuration.Value).OnComplete(() => bgmAudioSource.Stop());

        audioLoader.UnloadAsset(audioConfiguration.GetBgmPath(bgmName), this);
    }

    public void PlayBgm(string bgmName) {
        PlayBgmAsync(bgmName).Forget();
    }

    public void PlayBgm(AudioClip clip) {
        PlayBgm(clip, audioConfiguration.DefaultFadeDuration);
    }

    public void PlayBgm(AudioClip clip, float fadeDuration) {
        bgmAudioSource.DOKill();
        bgmAudioSource.clip = clip;
        bgmAudioSource.loop = true;

        bgmAudioSource.DOFade(1, fadeDuration).From(0);
        bgmAudioSource.Play();
    }

    public void StopBgm() {
        bgmAudioSource.Stop();
    }

    public void PlaySe(AudioClip clip) {
        seAudioSource.PlayOneShot(clip);
    }

    public async UniTask PlaySeAsync(string seName) {
        var clip = await audioLoader.LoadAsync(audioConfiguration.GetSePath(seName));

        seAudioSource.PlayOneShot(clip);
    }
}