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

        bool canReturn =
            GameBootstrap.Context.StateMachine.IsInState(GameState.BoardEdit) ||
            GameBootstrap.Context.StateMachine.IsInState(GameState.PackOpening);

        if (!canReturn)
        {
            Debug.Log(GameBootstrap.Context.StateMachine.CurrentState);
            Debug.Log("Cannot go back to shop from this state.");
            return;
        }

        GameBootstrap.Context.Loop.EnterShopBuild();
    }
}