using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomConfigurations/AudioConfiguration", fileName = "AudioConfiguration")]
public class AudioConfiguration : ScriptableObject
{
    public float DefaultFadeDuration = 0.3f;
    public string BgmPath, SePath, AmbiencePath;
    public int RandomSeAudioSourceCount = 5;

    public float RandomPitchMin = 0.9f;

    [Serializable]
    public class AudioData
    {
        public int Id;
        public string AudioName;
    }

    public List<AudioData> Bgms;

    public string GetBgmName(int id) {
        var data = Bgms.Find(x => x.Id == id);

        if (data == null) {
            Debug.LogError($"Can't find bgm with id : {id}.");
            return "";
        } else {
            return data.AudioName;
        }
    }

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