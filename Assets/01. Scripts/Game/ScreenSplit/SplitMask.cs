using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SplitMask : MonoBehaviour
{
    private QuadGraphic parentGraphic;

    private void Start()
    {
        parentGraphic = GetComponentInParent<QuadGraphic>();
    }

    private void Update()
    {
        SetCenter();
        SetSize();
    }

    private void SetCenter()
    {
        Vector2 center;

        if (parentGraphic.c == parentGraphic.d)
            center = (parentGraphic.a + parentGraphic.b + parentGraphic.c) / 3f;
        else if (parentGraphic.b == parentGraphic.c)
            center = (parentGraphic.a + parentGraphic.b + parentGraphic.d) / 3f;
        else
            center = (parentGraphic.a + parentGraphic.b + parentGraphic.c + parentGraphic.d) / 4f;

        GetComponent<RectTransform>().position = center;
    }

    private void SetSize()
    {
        //List<Vector2> poses = new List<Vector2>()
        //{
        //    parentGraphic.a,
        //    parentGraphic.b,
        //    parentGraphic.c,
        //    parentGraphic.d
        //};

        //float xMin = poses.OrderBy(a => a.x).First().x;
        //float xMax = poses.OrderBy(a => a.x).Last().x;
        //float yMin = poses.OrderBy(a => a.y).First().y;
        //float yMax = poses.OrderBy(a => a.y).Last().y;

        //float width = xMax - xMin;
        //float height = yMax - yMin;

        //GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
    }
}
