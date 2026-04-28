using UnityEngine;

public class UIStaggerAppearController : MonoBehaviour
{
    [SerializeField] private UIAppearTween[] items;
    [SerializeField] private float staggerDelay = 0.05f;

    public void Play()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                items[i].Play(i * staggerDelay);
            }
        }
    }

    // Useful if items are spawned dynamically
    public void PlayFromChildren()
    {
        UIAppearTween[] children = GetComponentsInChildren<UIAppearTween>(true);

        for (int i = 0; i < children.Length; i++)
        {
            children[i].Play(i * staggerDelay);
        }
    }
}