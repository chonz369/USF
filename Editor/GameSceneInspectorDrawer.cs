using Cysharp.Threading.Tasks;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ISceneState), true)]
public class GameSceneInspectorDrawer : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Run Scene")) {
            if (!Application.isPlaying) return;

            FindObjectOfType<GameSceneManager>().StartGameScene((ISceneState)target).Forget();
        }
    }
}