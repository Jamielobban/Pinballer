using UnityEngine;

public class PlacementNode : MonoBehaviour
{
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color occupiedColor = Color.gray;

    public bool IsOccupied { get; private set; }

    private GameObject _placedObject;

    public PlaceableRuntimeData RuntimeData { get; private set; }
    public GameObject PlacedObject => _placedObject;

    private void Awake()
    {
        RefreshVisual(false);
    }

    public bool CanPlace()
    {
        return !IsOccupied;
    }

    public void Place(InventoryItem item)
    {
        if (item == null || item.PartDefinition == null || item.PartDefinition.Prefab == null || IsOccupied)
            return;

        PlaceableRuntimeData runtime = new PlaceableRuntimeData
        {
            Definition = item.PartDefinition
        };

        if (item.PartDefinition.DefaultModifiers != null)
            runtime.Modifiers.AddRange(item.PartDefinition.DefaultModifiers);

        if (item.Modifiers != null)
            runtime.Modifiers.AddRange(item.Modifiers);

        _placedObject = Instantiate(item.PartDefinition.Prefab, transform.position, Quaternion.identity);
        runtime.Instance = _placedObject;

        RuntimeData = runtime;

        IPlaceableView view = _placedObject.GetComponent<IPlaceableView>();
        if (view != null)
            view.Initialize(runtime);

        IsOccupied = true;
        RefreshVisual(false);
    }

    public void SetHighlighted(bool highlighted)
    {
        RefreshVisual(highlighted);
    }

    private void RefreshVisual(bool highlighted)
    {
        if (visual == null)
            return;

        if (IsOccupied)
            visual.color = occupiedColor;
        else if (highlighted)
            visual.color = selectedColor;
        else
            visual.color = normalColor;
    }
}