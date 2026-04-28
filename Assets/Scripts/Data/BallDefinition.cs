using UnityEngine;

[CreateAssetMenu(menuName = "Game/Balls/Ball Definition")]
public class BallDefinition : ScriptableObject
{
    [Header("Identity")]
    public string Id;
    public string DisplayName;

    [Header("Prefab")]
    public BallView Prefab;

    [Header("Visuals")]
    public Sprite Sprite;
    public Color Tint = Color.white;
    public PhysicsMaterial2D PhysicsMaterial;

    [Header("Physics")]
    public float Mass = 1f;
    public float GravityScale = 1f;
    public float LinearDrag = 0f;
    public float AngularDrag = 0.05f;
    public float SizeMultiplier = 1f;
    public float Bounciness = 0.9f;
    public float Friction = 0f;

    [Header("Scoring")]
    public int ValueMultiplier = 1;

    [Header("Shop")]
    public int BaseCost = 25;
}