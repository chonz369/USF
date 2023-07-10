using UnityEngine;

public class GameSceneData : ISceneData
{
    [SerializeField]
    private Canvas canvas;

    public Canvas Canvas => canvas;

    [SerializeField]
    private Camera rockCamera;

    public Camera RockCamera => rockCamera;

    [SerializeField]
    private int rollingMaxDistance, triggerRollingGameDistance, rollingDistancePerSecond;

    public int RollingMaxDistance => rollingMaxDistance;
    public int TriggerRollingGameDistance => triggerRollingGameDistance;
    public int RollingDistancePerSecond => rollingDistancePerSecond;
}