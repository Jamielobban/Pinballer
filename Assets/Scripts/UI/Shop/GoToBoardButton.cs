using UnityEngine;
using UnityEngine.UI;

public class GoToBoardButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(GoToBoard);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(GoToBoard);
    }

    private void GoToBoard()
    {
        if (GameBootstrap.Context == null)
            return;

        if (!GameBootstrap.Context.StateMachine.IsInState(GameState.ShopBuild))
            return;

        GameBootstrap.Context.Loop.EnterBoardEdit();
    }
}