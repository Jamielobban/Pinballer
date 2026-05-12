using UnityEngine;

public class PackOpeningPanelView : MonoBehaviour
{
    [SerializeField] private PackOpeningManager packOpeningManager;
    [SerializeField] private PackChoiceButtonView[] choiceButtons;

    private void Awake()
    {
        if (packOpeningManager == null)
            packOpeningManager = FindFirstObjectByType<PackOpeningManager>();
    }

    private void Start()
    {
        if (packOpeningManager != null)
            packOpeningManager.ChoicesChanged += RefreshChoices;

        BindButtons();
        RefreshChoices();
    }

    private void OnDestroy()
    {
        if (packOpeningManager != null)
            packOpeningManager.ChoicesChanged -= RefreshChoices;
    }

    private void BindButtons()
    {
        if (choiceButtons == null || packOpeningManager == null)
            return;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
                choiceButtons[i].Bind(packOpeningManager, i);
        }
    }

    private void RefreshChoices()
    {
        if (choiceButtons == null)
            return;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
                choiceButtons[i].Refresh();
        }
    }
}