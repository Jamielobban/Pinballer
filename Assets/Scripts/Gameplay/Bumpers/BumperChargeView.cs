using UnityEngine;
using UnityEngine.UI;

public class BumperChargeView : MonoBehaviour
{
    [SerializeField] private BumperView bumperView;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject root;

    private void Awake()
    {
        if (bumperView == null)
            bumperView = GetComponentInParent<BumperView>();

        if (fillImage == null)
            fillImage = GetComponent<Image>();

        if (root == null)
            root = gameObject;
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (bumperView == null || fillImage == null)
            return;

        PlaceableRuntimeData runtime = bumperView.RuntimeData;

        if (runtime == null)
        {
            fillImage.fillAmount = 0f;
            return;
        }

        float charge = runtime.GetChargePercent();
        fillImage.fillAmount = charge;

        if (root != null)
            root.SetActive(runtime.GetFinalHitsRequired() > 1);
    }
}