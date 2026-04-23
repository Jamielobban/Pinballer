using UnityEngine;

[ExecuteAlways] // runs in editor too
[RequireComponent(typeof(EdgeCollider2D))]
public class ArcCollider2D : MonoBehaviour
{
    public float width = 10f;
    public float height = 5f;
    [Range(3, 50)] public int segments = 20;

    private EdgeCollider2D edge;

    private void OnValidate()
    {
        UpdateCollider();
    }

    private void Awake()
    {
        UpdateCollider();
    }

    void UpdateCollider()
    {
        if (edge == null)
            edge = GetComponent<EdgeCollider2D>();

        if (segments < 3) segments = 3;

        Vector2[] points = new Vector2[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(Mathf.PI, 0f, t);

            float x = Mathf.Cos(angle) * width * 0.5f;
            float y = Mathf.Sin(angle) * height;

            points[i] = new Vector2(x, y);
        }

        edge.points = points;
    }
}