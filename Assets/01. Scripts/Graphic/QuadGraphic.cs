using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class QuadGraphic : Graphic
{
    public Vector2 a, b, c, d;
    public Color quadColor = Color.green;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        UIVertex vert = UIVertex.simpleVert;
        vert.color = quadColor;

        vert.position = a; vh.AddVert(vert);
        vert.position = b; vh.AddVert(vert);
        vert.position = c; vh.AddVert(vert);
        vert.position = d; vh.AddVert(vert);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(0, 2, 3);
    }

    public void SetQuad(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        SetVerticesDirty(); // UI 다시 그리기 트리거
    }

    public void SetColor(Color color)
    {
        this.quadColor = color;
        SetVerticesDirty();
    }
}