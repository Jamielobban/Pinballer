using UnityEngine;

public enum PlaceablePartType
{
    Bumper,
    Peg,
    Wall,
    Jackpot
}

[CreateAssetMenu(menuName = "Game/Build/Placeable Part Definition")]
public class PlaceablePartDefinition : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public PlaceablePartType PartType;

    [Header("Shop")]
    public int BaseCost = 10;

    [Header("Prefab")]
    public GameObject Prefab;
}