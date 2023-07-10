using UnityEngine;

[CreateAssetMenu(menuName = "CustomConfigurations/AudioConfiguration", fileName = "AudioConfiguration")]
public class AudioConfiguration : ScriptableObject
{
    public float DefaultFadeDuration = 0.3f;
    public string BgmPath, SePath, AmbiencePath;

    public string GetBgmPath(string bgmName) {
        if (BgmPath.EndsWith("/")) {
            return BgmPath + bgmName;
        } else {
            return BgmPath + "/" + bgmName;
        }
    }

    public string GetSePath(string seName) {
        if (SePath.EndsWith("/")) {
            return SePath + seName;
        } else {
            return SePath + "/" + seName;
        }
    }

    public string GetAmbiencePath(string ambienceName) {
        if (AmbiencePath.EndsWith("/")) {
            return AmbiencePath + ambienceName;
        } else {
            return AmbiencePath + "/" + ambienceName;
        }
    }
}