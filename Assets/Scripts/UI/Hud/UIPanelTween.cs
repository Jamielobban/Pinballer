using DG.Tweening;
using UnityEngine;

public class UIPanelTween : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;
    [SerializeField] private float duration = 0.18f;
    [SerializeField] private Vector2 hiddenOffset = new Vector2(0f, -30f);

    private Vector2 _shownPosition;
    private Vector2 _hiddenPosition;

    private void Awake()
    {
        if (panel == null)
            panel = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        _shownPosition = panel.anchoredPosition;
        _hiddenPosition = _shownPosition + hiddenOffset;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        panel.DOKill();
        canvasGroup.DOKill();

        panel.anchoredPosition = _hiddenPosition;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        panel.DOAnchorPos(_shownPosition, duration).SetEase(Ease.OutBack);
        canvasGroup.DOFade(1f, duration);
    }

    public void Hide()
    {
        panel.DOKill();
        canvasGroup.DOKill();

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        panel.DOAnchorPos(_hiddenPosition, duration).SetEase(Ease.InBack);
        canvasGroup.DOFade(0f, duration)
            .OnComplete(() => gameObject.SetActive(false));
    }
}