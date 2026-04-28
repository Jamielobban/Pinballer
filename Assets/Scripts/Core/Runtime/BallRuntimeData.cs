using UnityEngine;

[System.Serializable]
public class BallRuntimeData
{
    public int BallId;
    public BallDefinition Definition;

    public float SizeMultiplier = 1f;
    public float SpeedMultiplier = 1f;
    public int ValueMultiplier = 1;

    public bool IsLoaded;
    public bool IsInPlay;

    public GameObject BallObject;
}