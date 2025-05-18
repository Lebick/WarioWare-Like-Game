using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SplitTest : MonoBehaviour
{
    public bool isLerp;

    private float maxDistance;

    public int splitValue;

    public List<float> angles = new();
    public List<float> currentAngles = new();

    public GameObject maskPrefab;
    public List<QuadGraphic> masks = new();

    void Update()
    {
        float h = Screen.width / 2;
        float v = Screen.height / 2f;

        maxDistance = Mathf.Sqrt(Mathf.Pow(h, 2) + Mathf.Pow(v, 2));

        if (angles.Count != splitValue)
        {
            if (angles.Count > splitValue) //플레이어가 줄어든 상태
                DecreaseSplit();
            else //플레이어가 늘어난 상태
                IncreaseSplit();

            angles = GetAngle();
        }

        UpdateCurrentAngle();
    }

    private void DecreaseSplit()
    {
        for (int i = 0; i < angles.Count - splitValue; i++)
        {
            currentAngles.RemoveAt(currentAngles.Count - 1);

            GameObject temp = masks[^1].gameObject;
            masks.RemoveAt(masks.Count - 1);

            Destroy(temp);
        }
    }

    private void IncreaseSplit()
    {
        for (int i = 0; i < splitValue - angles.Count; i++)
        {
            if (currentAngles.Count < 1)
                currentAngles.Add(90);
            else
                currentAngles.Insert(1, 90);

            QuadGraphic mask = Instantiate(maskPrefab, transform).GetComponent<QuadGraphic>();
            mask.quadColor = new Color(Random.value, Random.value, Random.value);
            masks.Add(mask);
        }
    }

    private void UpdateCurrentAngle()
    {
        Vector3 a;
        Vector3 b;
        Vector3 d;
        Vector3 c;

        if (splitValue > 2)
        {
            for (int i = 0; i < currentAngles.Count; i++)
            {
                if (isLerp)
                    currentAngles[i] = Mathf.Lerp(currentAngles[i], angles[i], Time.deltaTime * 5f);
                else
                    currentAngles[i] = angles[i];

                a = transform.position;
                b = GetEndPos(currentAngles[i]);
                d = GetEndPos(currentAngles[i == currentAngles.Count - 1 ? 0 : i + 1]);
                c = new Vector2(Mathf.Abs(b.x - a.x) > Mathf.Abs(d.x - a.x) ? b.x : d.x, Mathf.Abs(b.y - a.y) > Mathf.Abs(d.y - a.y) ? b.y : d.y);

                DrawQuad(i, a, b, c, d);
            }
        }
        else
        {
            if (splitValue == 1)
            {
                a = new Vector3(0, 0);
                b = new Vector3(0, Screen.height);
                c = new Vector3(Screen.width, Screen.height);
                d = new Vector3(Screen.width, 0);
                DrawQuad(0, a, b, c, d);
            }

            if (splitValue == 2)
            {
                a = new Vector3(0, 0);
                b = new Vector3(0, Screen.height);
                c = new Vector3(Screen.width / 2, Screen.height);
                d = new Vector3(Screen.width / 2, 0);
                DrawQuad(0, a, b, c, d);

                Vector3 padding = new Vector3(Screen.width / 2, 0);

                DrawQuad(1, a + padding, b + padding, c + padding, d + padding);
            }

            for (int i = 0; i < currentAngles.Count; i++)
                currentAngles[i] = angles[i];
        }
    }

    private void DrawQuad(int i, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        b = ScreenClamp(b);
        c = ScreenClamp(c);
        d = ScreenClamp(d);

        masks[i].SetQuad(a, b, c, d);
    }

    private Vector3 ScreenClamp(Vector3 value)
    {
        value.x = Mathf.Clamp(value.x, 0, Screen.width);
        value.y = Mathf.Clamp(value.y, 0, Screen.height);

        return value;
    }

    private List<float> GetAngle()
    {
        List<float> temp = new();

        float angle = 360f / splitValue;

        for (int i = splitValue; i >= 1; i--)
        {
            float currentAngle = angle * i + 90;
            currentAngle -= 360;
            temp.Add(currentAngle);
        }

        return temp;
    }

    private Vector3 GetEndPos(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
        Vector3 end = transform.position + dir * maxDistance;

        return end;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        for (int i = 0; i < splitValue; i++)
        {
            Gizmos.DrawLine(transform.position, GetEndPos(currentAngles[i]));
        }
    }
}
