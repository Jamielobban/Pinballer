using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoundResultPanelView : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text buttonText;

    private void Awake()
    {
        if (root == null)
            root = gameObject;

        if (button != null)
            button.onClick.AddListener(OnContinueClicked);

        Hide();
    }

    private void Start()
    {
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();

        if (button != null)
            button.onClick.RemoveListener(OnContinueClicked);
    }

    private void Subscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.RoundEnded += OnRoundEnded;
        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;
    }

    private void Unsubscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.RoundEnded -= OnRoundEnded;
        GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
    }

    private void OnRoundEnded()
    {
        ShowResult();
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.ShopBuild || state == GameState.BoardEdit)
            Hide();
    }

    private void ShowResult()
    {
        if (GameBootstrap.Context == null)
            return;

        bool success = GameBootstrap.Context.Score.HasMetTarget();

        if (titleText != null)
            titleText.text = success ? "Round Complete" : "Game Over";

        if (scoreText != null)
        {
            scoreText.text =
                $"Score: {GameBootstrap.Context.Score.RoundScore} / {GameBootstrap.Context.Score.TargetScore}\n" +
                $"Total Score: {GameBootstrap.Context.Score.TotalScore}\n" +
                $"Money: {GameBootstrap.Context.Economy.CurrentMoney}";
        }

        if (buttonText != null)
            buttonText.text = success ? "Continue" : "Retry";

        root.SetActive(true);
    }

    private void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    private void OnContinueClicked()
    {
        if (GameBootstrap.Context == null)
            return;

        bool success = GameBootstrap.Context.Score.HasMetTarget();

        Hide();

        if (success)
        {
            GameBootstrap.Context.Loop.EnterShopBuild();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}