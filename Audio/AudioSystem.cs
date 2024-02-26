using Cysharp.Threading.Tasks;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.AddressableAssets;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Audio;
using MoreMountains.Tools;
using USF.Core;

public class AudioSystem : MonoBehaviour, GameSystem
{
    public static AudioSystem Instance { get; private set; }

    [SerializeField]
    private AudioSource bgmAudioSource, ambienceAudioSource;

    [SerializeField]
    private AudioConfiguration audioConfiguration;

    [SerializeField]
    private Transform seSourceRoot;

    private AudioSource[] seAudioSources;

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

    public float AmbienceVolume {
        get => ambienceVolume.Value;
        set {
            audioMixer.SetFloat("AmbienceVolume", Mathf.Clamp(Mathf.Log(Mathf.Max(0.001f, value)) * 20, -80, 0));
            PlayerPrefs.SetFloat("AmbienceVolume", value);
            ambienceVolume.Value = value;
        }
    }

    [SerializeField]
    private FloatVariable bgmVolume, seVolume, ambienceVolume;

    [SerializeField]
    private AudioMixer audioMixer;

    public UniTask Init(GameEngine gameEngine) {
        Instance = this;

        audioLoader = new AssetLoader<AudioClip>();

        seAudioSources = new AudioSource[audioConfiguration.RandomSeAudioSourceCount];
        for (int i = 0; i < audioConfiguration.RandomSeAudioSourceCount; i++) {
            var g = new GameObject();
            g.name = $"SeAudioSource ({i})";
            var au = g.AddComponent<AudioSource>();
            au.pitch = 1 - i * (1 - audioConfiguration.RandomPitchMin) / audioConfiguration.RandomSeAudioSourceCount;
            au.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Se")[0];
            g.transform.SetParent(seSourceRoot);

            seAudioSources[i] = au;
        }

        LoadSavedSettings();

        return UniTask.CompletedTask;
    }

    private void LoadSavedSettings() {
        BgmVolume = PlayerPrefs.GetFloat("BgmVolume", 1);
        SeVolume = PlayerPrefs.GetFloat("SeVolume", 1);
        AmbienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1);
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

    public async UniTask PlayBgmAsync(int id, float? fadeDuration = null) {
        await PlayBgmAsync(audioConfiguration.GetBgmName(id), fadeDuration);
    }

    public async UniTask PlayBgmAsync(string bgmName, float? fadeDuration = null) {
        if (fadeDuration == null) {
            fadeDuration = audioConfiguration.DefaultFadeDuration;
        }

        var clip = await audioLoader.LoadAndHoldAsync(audioConfiguration.GetBgmPath(bgmName), this);

        PlayBgm(clip, fadeDuration.Value);
    }

    public UniTask StopBgmAsync(string bgmName, float? fadeDuration = null) {
        if (fadeDuration == null) {
            fadeDuration = audioConfiguration.DefaultFadeDuration;
        }

        bgmAudioSource.DOKill();
        bgmAudioSource.clip = null;

        bgmAudioSource.DOFade(0, fadeDuration.Value).OnComplete(() => bgmAudioSource.Stop());

        audioLoader.UnloadAsset(audioConfiguration.GetBgmPath(bgmName), this);

        return UniTask.CompletedTask;
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
        seAudioSources[0].PlayOneShot(clip);
    }

    public void PlaySeRandomPitch(AudioClip clip) {
        seAudioSources[Random.Range(0, audioConfiguration.RandomSeAudioSourceCount)].PlayOneShot(clip);
    }

    public async UniTask PlaySeAsync(string seName, bool randomPitch) {
        var clip = await audioLoader.LoadAsync(audioConfiguration.GetSePath(seName));

        if (randomPitch) {
            PlaySeRandomPitch(clip);
        } else {
            PlaySe(clip);
        }
    }

    private void OnDestroy() {
        audioLoader.UnloadAll();
    }
}