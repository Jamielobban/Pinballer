using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIAnimatedCard : MonoBehaviour
{
    [SerializeField] private float appearDuration = 0.22f;
    [SerializeField] private float disappearDuration = 0.16f;
    [SerializeField] private float startScale = 0.85f;
    [SerializeField] private Ease appearEase = Ease.OutBack;
    [SerializeField] private Ease disappearEase = Ease.InBack;

    private CanvasGroup _canvasGroup;
    private Button _button;
    private Tween _activeTween;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _button = GetComponent<Button>();
    }

    public void SetInteractable(bool interactable)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.interactable = interactable;
            _canvasGroup.blocksRaycasts = interactable;
        }

        if (_button != null)
            _button.interactable = interactable;
    }

    public Tween PlayAppear(float delay = 0f)
    {
        KillTween();

        transform.localScale = Vector3.one * startScale;

        if (_canvasGroup != null)
            _canvasGroup.alpha = 0f;

        SetInteractable(false);

        Sequence sequence = DOTween.Sequence();

        sequence.SetDelay(delay);
        sequence.Join(transform.DOScale(1f, appearDuration).SetEase(appearEase));

        if (_canvasGroup != null)
            sequence.Join(_canvasGroup.DOFade(1f, appearDuration));

        sequence.OnComplete(() =>
        {
            SetInteractable(true);
        });

        _activeTween = sequence;
        return sequence;
    }

    public Tween PlayDisappear(float delay = 0f)
    {
        KillTween();

        SetInteractable(false);

        Sequence sequence = DOTween.Sequence();

        sequence.SetDelay(delay);
        sequence.Join(transform.DOScale(0f, disappearDuration).SetEase(disappearEase));

        if (_canvasGroup != null)
            sequence.Join(_canvasGroup.DOFade(0f, disappearDuration));

        _activeTween = sequence;
        return sequence;
    }

    public void KillTween()
    {
        if (_activeTween != null && _activeTween.IsActive())
            _activeTween.Kill();

        transform.DOKill();
        if (_canvasGroup != null)
            _canvasGroup.DOKill();
    }

    private void OnDestroy()
    {
        KillTween();
    }
}