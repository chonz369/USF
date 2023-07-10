using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class LocalizedBase : MonoBehaviour
{
    public abstract void Refresh();
}

public class LanguageManager : IGameSystem
{
    public static LanguageManager Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<LanguageManager>();
            }
            return instance;
        }
    }

    private static LanguageManager instance;
    public string CurrentLanguage { get; private set; }

    [SerializeField]
    private LanguageConfiguration languageData;

    private Dictionary<string, string> localizedTextDictionary = new Dictionary<string, string>();

    public override async UniTask Init() {
        instance = this;

        CurrentLanguage = languageData.DefaultLanguage;

        await UpdateLanguage(CurrentLanguage);
    }

    public string LocalizeText(string gameTextType) {
        if (localizedTextDictionary.ContainsKey(gameTextType)) {
            return localizedTextDictionary[gameTextType];
        } else {
            Debug.LogWarning($"{gameTextType} does not exist in dictionary.");
            return gameTextType;
        }
    }

    public void SwitchLanguage(string language) {
        if (CurrentLanguage == language) return;

        if (!Array.Exists(languageData.SupportedLanguages, x => x == language)) {
            Debug.LogError("Unsupported language : " + language);
            return;
        }
        CurrentLanguage = language;
        localizedTextDictionary.Clear();

        UpdateLanguage(CurrentLanguage).Forget();
    }

    private void Refresh() {
        Array.ForEach(FindObjectsOfType<LocalizedBase>(), x => x.Refresh());
    }

    private async UniTask UpdateLanguage(string language) {
        //todo : improve this
        var handle = Addressables.LoadAssetAsync<TextAsset>($"Assets/LocalizedAssets/Text/SystemText");
        await handle;

        var textAsset = handle.Result;
        ParseText(textAsset.text);

        Addressables.Release(handle);

        Refresh();

        void ParseText(string text) {
            text = text.Replace("\r\n", "\n");
            string[] lines = text.Split('\n');

            int languageIndex = Array.IndexOf(lines[0].Split(','), language);

            for (int i = 1; i < lines.Length; i++) {
                if (string.IsNullOrEmpty(lines[i])) continue;
                var texts = lines[i].Split(',');

                localizedTextDictionary.Add(texts[0].Trim(), texts[languageIndex].Trim());
            }
        }
    }
}