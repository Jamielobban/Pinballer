using UnityEngine;
using TMPro;

public class PlaceableInspectorUI : MonoBehaviour
{
    public static PlaceableInspectorUI Instance { get; private set; }

    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text modifiersText;

    private void Awake()
    {
        Instance = this;

        if (root != null)
            root.SetActive(false);
    }

    public void Show(PlaceableRuntimeData runtime)
    {
        if (runtime == null || runtime.Definition == null)
            return;

        if (root != null)
            root.SetActive(true);

        titleText.text = runtime.Definition.DisplayName;

        statsText.text =
            "Value: " + runtime.GetFinalValue() +
            "\nForce: " + runtime.GetFinalForce();

        if (runtime.Modifiers == null || runtime.Modifiers.Count == 0)
        {
            modifiersText.text = "Modifiers:\nNone";
            return;
        }

        string text = "Modifiers:";

        for (int i = 0; i < runtime.Modifiers.Count; i++)
        {
            ModifierDefinition mod = runtime.Modifiers[i];

            if (mod == null)
                continue;

            text += "\n- " + mod.DisplayName + " (" + mod.Type + ")";
        }

        modifiersText.text = text;
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }
}