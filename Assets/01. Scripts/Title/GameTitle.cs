using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameTitle : MonoBehaviour
{
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    public TMP_Text titleText;

    public Vector2 wordSpawnCenter;
    public float wordSpawnRange;

    [Range(1, 100)]
    public int titleTextStep;

    public float titleTextDuration;

    private List<WordPosition> wordPositions = new();

    public Animator pressAnyKey;

    private bool isEndTitleTextAnim;
    private bool isStart;

    private void Start()
    {
        titleText.ForceMeshUpdate();
        StartCoroutine(TextAnimation());
        Pixelate.instance.SetPixelate(2048);
    }

    #region Ÿ��Ʋ �ؽ�Ʈ �ִϸ��̼�

    private IEnumerator TextAnimation()
    {
        titleText.ForceMeshUpdate();

        TMP_TextInfo textInfo = titleText.textInfo;

        if (textInfo.characterCount == 0)
        {
            Debug.LogError("�ؽ�Ʈ �־��");
            yield break;
        }

        for (int i=0; i<textInfo.characterCount; i++)
        {
            wordPositions.Add(new WordPosition());
            WordPosition word = wordPositions.Last();

            Vector2 startPos = wordSpawnCenter + (Random.insideUnitCircle.normalized * wordSpawnRange);
            Vector2 endPos = GetCurrentPositoin(textInfo.characterInfo[i]);

            word.stepPoses.Add(startPos);
            for(int j=1; j<titleTextStep; j++)
                word.stepPoses.Add(Vector2.Lerp(startPos, endPos, 1f / titleTextStep * j));

            word.stepPoses.Add(endPos);
        }

        float progress = 0f;
        float subProgress = 0f;

        while(progress <= 1f)
        {
            progress += Time.deltaTime / titleTextDuration;
            subProgress += Time.deltaTime * titleTextStep / titleTextDuration;

            if (subProgress >= 1f)
                subProgress -= 1f;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                WordPosition word = wordPositions[i];

                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                    continue;

                int currentStep = 0;
                for(int j=1; j<=titleTextStep; j++)
                {
                    if (progress > (1f / titleTextStep) * j)
                        currentStep++;
                    else
                        break;
                }

                Vector2 finalPos;

                if (currentStep != titleTextStep)
                    finalPos = GetFinalPos(word.stepPoses[currentStep], word.stepPoses[currentStep + 1], subProgress);
                else
                    finalPos = word.stepPoses.Last();

                SetPosition(textInfo, charInfo, finalPos);
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                titleText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            yield return null;
        }

        EndTitleTextAnimation();
        yield break;
    }

    private Vector2 GetFinalPos(Vector3 p1, Vector3 p3, float progress)
    {
        Vector3 p2 = p1 + Quaternion.Euler(0, 0, 60f * Mathf.Sign(p1.x)) * (p3 - p1);

        Vector3 p4 = Vector3.Lerp(p1, p2, progress);
        Vector3 p5 = Vector3.Lerp(p2, p3, progress);
        return Vector3.Lerp(p4, p5, progress);
    }

    private Vector2 GetCurrentPositoin(TMP_CharacterInfo charInfo)
    {
        Vector3 bottomLeft = charInfo.bottomLeft;
        Vector3 topRight = charInfo.topRight;
        Vector3 center = (bottomLeft + topRight) / 2;

        Vector3 worldPos = titleText.transform.TransformPoint(center);

        return worldPos;
    }

    private void SetPosition(TMP_TextInfo textInfo, TMP_CharacterInfo charInfo, Vector3 worldCenterPos)
    {
        int vIdx = charInfo.vertexIndex;
        int mIdx = charInfo.materialReferenceIndex;
        var verts = textInfo.meshInfo[mIdx].vertices;

        Vector3 center = (charInfo.bottomLeft + charInfo.topRight) * 0.5f;
        Vector3 offsetBL = charInfo.bottomLeft - center;
        Vector3 offsetTL = new Vector3(charInfo.bottomLeft.x, charInfo.topRight.y) - center;
        Vector3 offsetTR = charInfo.topRight - center;
        Vector3 offsetBR = new Vector3(charInfo.topRight.x, charInfo.bottomLeft.y) - center;

        Vector3 localCenter = titleText.transform.InverseTransformPoint(worldCenterPos);

        verts[vIdx + 0] = localCenter + offsetBL;
        verts[vIdx + 1] = localCenter + offsetTL;
        verts[vIdx + 2] = localCenter + offsetTR;
        verts[vIdx + 3] = localCenter + offsetBR;
    }

    #endregion

    private void EndTitleTextAnimation()
    {
        pressAnyKey.enabled = true;
        isEndTitleTextAnim = true;
    }

    private void Update()
    {
        if (!isEndTitleTextAnim || isStart) return;

        if (Input.anyKey)
        {
            if (Input.GetMouseButton(0) && IsPointerOverUI())
                return;

            isStart = true;

            Pixelate.instance.SetLerpPixelate(500f, 5f, 1f);
            Fade.instance.SetFade(0f, 1f, 1f);
        }
    }

    private bool IsPointerOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        return results.Count > 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(wordSpawnCenter, wordSpawnRange);
    }
}

public class WordPosition
{
    public List<Vector2> stepPoses = new();
}