using UnityEngine;
using UnityEngine.UI;

public class BackToShopButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(BackToShop);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(BackToShop);
    }

    private void BackToShop()
    {
        if (GameBootstrap.Context == null)
            return;

        if (!GameBootstrap.Context.StateMachine.IsInState(GameState.BoardEdit))
            return;

        GameBootstrap.Context.Loop.EnterShopBuild();
    }
}