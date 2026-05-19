using DG.Tweening;
using UnityEngine;

public class ShopPanelView : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private ShopPackButtonView[] packButtons;

    [SerializeField] private float packStagger = 0.05f;

    private bool _isAnimating;

    public bool IsAnimating => _isAnimating;

    private void Awake()
    {
        if (shopManager == null)
            shopManager = FindFirstObjectByType<ShopManager>();
    }

    private void OnEnable()
    {
        RefreshAndAnimate();
    }

    private void Start()
    {
        if (shopManager != null)
            shopManager.PacksChanged += OnPacksChanged;

        BindButtons();
        RefreshAndAnimate();
    }

    private void OnDestroy()
    {
        if (shopManager != null)
            shopManager.PacksChanged -= OnPacksChanged;

        DOTween.Kill(this);
    }

    private void BindButtons()
    {
        if (packButtons == null || shopManager == null)
            return;

        for (int i = 0; i < packButtons.Length; i++)
        {
            if (packButtons[i] != null)
                packButtons[i].Bind(this, shopManager, i);
        }
    }

    private void OnPacksChanged()
    {
        RefreshAndAnimate();
    }

    private void RefreshAndAnimate()
    {
        if (!isActiveAndEnabled)
            return;

        DOTween.Kill(this);

        RefreshPacks();
        AnimatePacksIn();
    }

    private void RefreshPacks()
    {
        if (packButtons == null)
            return;

        for (int i = 0; i < packButtons.Length; i++)
        {
            if (packButtons[i] != null)
                packButtons[i].Refresh();
        }
    }

    private void AnimatePacksIn()
    {
        if (packButtons == null)
            return;

        _isAnimating = true;
        SetPackButtonsInteractable(false);

        Sequence sequence = DOTween.Sequence();
        sequence.SetTarget(this);

        for (int i = 0; i < packButtons.Length; i++)
        {
            if (packButtons[i] == null)
                continue;

            Tween tween = packButtons[i].PlayAppear(i * packStagger);

            if (tween != null)
                sequence.Join(tween);
        }

        sequence.OnComplete(() =>
        {
            _isAnimating = false;
            RefreshPacks();
        });
    }

    public void HandlePackClicked(ShopPackButtonView clickedButton)
    {
        if (_isAnimating || clickedButton == null)
            return;

        _isAnimating = true;
        SetPackButtonsInteractable(false);

        Tween tween = clickedButton.PlayDisappear();

        if (tween == null)
        {
            CompletePackClick(clickedButton);
            return;
        }

        tween.OnComplete(() =>
        {
            CompletePackClick(clickedButton);
        });
    }

    private void CompletePackClick(ShopPackButtonView clickedButton)
    {
        _isAnimating = false;

        if (clickedButton != null)
            clickedButton.ExecuteClick();
    }

    private void SetPackButtonsInteractable(bool interactable)
    {
        if (packButtons == null)
            return;

        for (int i = 0; i < packButtons.Length; i++)
        {
            if (packButtons[i] != null)
                packButtons[i].SetInteractable(interactable);
        }
    }
}