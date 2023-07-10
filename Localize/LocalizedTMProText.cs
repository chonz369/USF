using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedTMProText : LocalizedBase
{
    [SerializeField]
    private string key;

    public string Key => key;

    private string currentLanguage = "";

    private void OnEnable() {
        Refresh();
    }

    public override void Refresh() {
        if (currentLanguage == LanguageManager.Instance.CurrentLanguage) return;
        var text = GetComponent<TextMeshProUGUI>();
        text.text = LanguageManager.Instance.LocalizeText(key);

        currentLanguage = LanguageManager.Instance.CurrentLanguage;
    }
}