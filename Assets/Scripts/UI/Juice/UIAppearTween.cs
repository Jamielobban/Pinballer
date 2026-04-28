using DG.Tweening;
using UnityEngine;

public class UIAppearTween : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float duration = 0.18f;
    [SerializeField] private float delay = 0f;

    private Vector3 _baseScale;

    private void Awake()
    {
        _baseScale = transform.localScale;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Play(float extraDelay = 0f)
    {
        transform.DOKill();

        if (canvasGroup != null)
            canvasGroup.DOKill();

        transform.localScale = Vector3.zero;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        transform
            .DOScale(_baseScale, duration)
            .SetDelay(delay + extraDelay)
            .SetEase(Ease.OutBack);

        if (canvasGroup != null)
        {
            canvasGroup
                .DOFade(1f, duration * 0.8f)
                .SetDelay(delay + extraDelay);
        }
    }
}