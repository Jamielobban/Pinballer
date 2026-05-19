using System;
using UnityEngine;

[Serializable]
public class BallRuntimeData
{
    public int BallId;
    public string InstanceId;

    public BallDefinition Definition;
    public GameObject BallObject;

    public bool IsLoaded;
    public bool IsInPlay;

    public float SizeMultiplier = 1f;
    public float SpeedMultiplier = 1f;
    public float ValueMultiplier = 1f;

    public int RemainingRevives;
    public int HitCount;
    public int RemainingSplits;

    public BallRuntimeData()
    {
        InstanceId = Guid.NewGuid().ToString();
    }

    public BallRuntimeData(BallDefinition definition)
    {
        InstanceId = Guid.NewGuid().ToString();
        Definition = definition;

        if (definition != null)
        {
            SizeMultiplier = definition.SizeMultiplier;
            ValueMultiplier = definition.ValueMultiplier;
        }

        SpeedMultiplier = 1f;
    }
}