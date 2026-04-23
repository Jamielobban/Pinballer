using UnityEngine;

[CreateAssetMenu(menuName = "Game/Machine/Machine Part Definition")]
public class MachinePartDefinition : ScriptableObject
{
    public string Id;
    public string DisplayName;
    [TextArea] public string Description;
    public MachinePartType PartType;
}