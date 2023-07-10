using UnityEngine;

[CreateAssetMenu(menuName = "Registries/GameSceneRegistry", fileName = "GameSceneRegistry")]
public class GameSceneRegistry : ScriptableObject
{
    public ISceneState[] GameScenes;
}