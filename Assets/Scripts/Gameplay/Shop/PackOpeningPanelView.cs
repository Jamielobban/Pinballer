using DG.Tweening;
using UnityEngine;

public class PackOpeningPanelView : MonoBehaviour
{
    [SerializeField] private PackOpeningManager packOpeningManager;
    [SerializeField] private PackChoiceButtonView[] choiceButtons;

    [SerializeField] private float choiceStagger = 0.06f;

    private bool _isAnimating;

    public bool IsAnimating => _isAnimating;

    private void Awake()
    {
        if (packOpeningManager == null)
            packOpeningManager = FindFirstObjectByType<PackOpeningManager>();
    }

    private void Start()
    {
        if (packOpeningManager != null)
            packOpeningManager.ChoicesChanged += OnChoicesChanged;

        BindButtons();
        RefreshChoices();
        AnimateChoicesIn();
    }

    private void OnDestroy()
    {
        if (packOpeningManager != null)
            packOpeningManager.ChoicesChanged -= OnChoicesChanged;

        DOTween.Kill(this);
    }

    private void BindButtons()
    {
        if (choiceButtons == null || packOpeningManager == null)
            return;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
                choiceButtons[i].Bind(this, packOpeningManager, i);
        }
    }

    private void OnChoicesChanged()
    {
        if (_isAnimating)
            return;

        RefreshChoices();
        AnimateChoicesIn();
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

    private void AnimateChoicesIn()
    {
        if (choiceButtons == null)
            return;

        _isAnimating = true;
        SetChoiceButtonsInteractable(false);

        Sequence sequence = DOTween.Sequence();
        sequence.SetTarget(this);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] == null)
                continue;

            Tween tween = choiceButtons[i].PlayAppear(i * choiceStagger);

            if (tween != null)
                sequence.Join(tween);
        }

        sequence.OnComplete(() =>
        {
            _isAnimating = false;
            SetChoiceButtonsInteractable(true);
        });
    }

    public void HandleChoiceClicked(PackChoiceButtonView clickedButton)
    {
        if (_isAnimating || clickedButton == null)
            return;

        _isAnimating = true;
        SetChoiceButtonsInteractable(false);

        Tween tween = clickedButton.PlayDisappear();

        if (tween == null)
        {
            CompleteChoiceClick(clickedButton);
            return;
        }

        tween.OnComplete(() =>
        {
            CompleteChoiceClick(clickedButton);
        });
    }

    private void CompleteChoiceClick(PackChoiceButtonView clickedButton)
    {
        if (clickedButton != null)
            clickedButton.ExecuteClick();

        _isAnimating = false;
    }

    private void SetChoiceButtonsInteractable(bool interactable)
    {
        if (choiceButtons == null)
            return;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
                choiceButtons[i].SetInteractable(interactable && !_isAnimating);
        }
    }
}