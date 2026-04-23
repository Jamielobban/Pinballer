using UnityEngine;

[System.Serializable]
public struct HitEventData
{
    public string SourceId;
    public HitSourceType SourceType;
    public int BaseValue;
    public int FinalValue;
    public Vector2 Position;
    public BallRuntimeData Ball;
}