using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIJuicyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.04f;
    [SerializeField] private float pressScale = 0.96f;
    [SerializeField] private float duration = 0.08f;

    private Vector3 _baseScale;

    private void Awake()
    {
        _baseScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(_baseScale * hoverScale, duration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(_baseScale, duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(_baseScale * pressScale, duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(_baseScale * hoverScale, duration).SetEase(Ease.OutBack);
    }
}