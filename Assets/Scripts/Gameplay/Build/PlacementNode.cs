using UnityEngine;

public class PlacementNode : MonoBehaviour
{
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color occupiedColor = Color.gray;

    public bool IsOccupied { get; private set; }

    private GameObject _placedObject;

    private void Awake()
    {
        RefreshVisual(false);
    }

    public bool CanPlace()
    {
        return !IsOccupied;
    }

    public void Place(GameObject prefab)
    {
        if (prefab == null || IsOccupied)
            return;

        _placedObject = Instantiate(prefab, transform.position, Quaternion.identity);
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