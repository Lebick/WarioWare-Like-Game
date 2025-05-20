using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SplitMask : MonoBehaviour
{
    private QuadGraphic parentGraphic;

    private RectTransform rect;


    private void Start()
    {
        parentGraphic = GetComponentInParent<QuadGraphic>();
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        SetSize();
        SetCenter();
    }

    private void SetCenter()
    {
        List<Vector2> poses = new List<Vector2>()
        {
            parentGraphic.a,
            parentGraphic.b,
            parentGraphic.c,
            parentGraphic.d
        };

        float xMin = poses.Min(p => p.x);
        float xMax = poses.Max(p => p.x);
        float yMin = poses.Min(p => p.y);
        float yMax = poses.Max(p => p.y);

        Vector2 center = new Vector2((xMin + xMax) / 2f, (yMin + yMax) / 2f) * GamePlayManager.instance.splitCanvas.localScale;
        rect.position = center;

        rect.anchoredPosition -= rect.sizeDelta / 2f;
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
        rect.sizeDelta = new Vector2(Screen.width, Screen.height);
    }
}
