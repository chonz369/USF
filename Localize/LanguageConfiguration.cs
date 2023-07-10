using UnityEngine;

[CreateAssetMenu(menuName = "CustomConfigurations/LanguageConfiguration", fileName = "LanguageConfiguration")]
public class LanguageConfiguration : ScriptableObject
{
    public string DefaultLanguage = "en";

    public string[] SupportedLanguages;

    public TextAsset[] TextFiles;
}