using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private Camera worldCamera;
    [SerializeField] private LayerMask nodeLayerMask;
    [SerializeField] private PlacementNode[] nodes;

    private bool _isPlacementMode;

    private void Awake()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;
    }

    private void Start()
    {
        if (nodes == null || nodes.Length == 0)
            nodes = FindObjectsByType<PlacementNode>(FindObjectsSortMode.None);

        GameBootstrap.Context.Signals.InventoryItemSelected += OnInventoryItemSelected;
        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.InventoryItemSelected -= OnInventoryItemSelected;
        GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
    }

    private void Update()
    {
        if (!_isPlacementMode)
            return;

        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryClickNodeUnderMouse();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelPlacementMode();
        }
    }

    private void TryClickNodeUnderMouse()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = worldCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 point = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);

        Collider2D hit = Physics2D.OverlapPoint(point, nodeLayerMask);

        if (hit == null)
        {
            Debug.Log("Clicked, but no node under mouse.");
            return;
        }

        PlacementNode node = hit.GetComponent<PlacementNode>();

        if (node == null)
        {
            node = hit.GetComponentInParent<PlacementNode>();
        }

        if (node == null)
        {
            Debug.Log("Clicked collider, but it has no PlacementNode.");
            return;
        }

        TryPlaceAtNode(node);
    }

    private void OnInventoryItemSelected(InventoryItem item)
    {
        _isPlacementMode =
            item != null &&
            GameBootstrap.Context.StateMachine.IsInState(GameState.BoardEdit);

        HighlightNodes(_isPlacementMode);

       // Debug.Log(_isPlacementMode
            //? "Entered placement mode."
            //: "Exited placement mode.");
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state != GameState.BoardEdit)
        {
            CancelPlacementMode();
        }
    }

    public void TryPlaceAtNode(PlacementNode node)
    {
        if (!_isPlacementMode)
        {
            Debug.Log("Not in placement mode.");
            return;
        }

        InventoryItem selectedItem = GameBootstrap.Context.Inventory.SelectedItem;

        if (selectedItem == null || selectedItem.PartDefinition == null)
        {
            Debug.Log("No selected item.");
            return;
        }

        if (node == null || !node.CanPlace())
        {
            Debug.Log("Invalid or occupied node.");
            return;
        }

        Debug.Log("Placing: " + selectedItem.PartDefinition.DisplayName);

        node.Place(selectedItem.PartDefinition.Prefab);

        GameBootstrap.Context.Inventory.RemoveItem(selectedItem);

        CancelPlacementMode();
    }

    private void CancelPlacementMode()
    {
        _isPlacementMode = false;
        HighlightNodes(false);

        if (GameBootstrap.Context != null)
            GameBootstrap.Context.Inventory.ClearSelection();
    }

    private void HighlightNodes(bool highlighted)
    {
        if (nodes == null)
            return;

        foreach (PlacementNode node in nodes)
        {
            if (node != null && node.CanPlace())
                node.SetHighlighted(highlighted);
        }
    }
}