using System.Collections;
using UnityEngine;

public class Pixelate : Singleton<Pixelate>
{
    public Material pixelateMat;

    private IEnumerator lerpCoroutine;

    public void SetPixelate(float targetAmount)
    {
        if (lerpCoroutine != null)
            StopCoroutine(lerpCoroutine);

        pixelateMat.SetFloat("_pixelSize", targetAmount);
    }

    public void SetLerpPixelate(float startAmount, float targetAmount, float time)
    {
        if (lerpCoroutine != null)
            StopCoroutine(lerpCoroutine);

        lerpCoroutine = LerpPixelate(startAmount, targetAmount, time);
        StartCoroutine(lerpCoroutine);
    }

    private IEnumerator LerpPixelate(float startAmount, float targetAmount, float time)
    {
        float startValue = startAmount;

        float progress = 0f;

        while(progress <= 1f)
        {
            progress += Time.deltaTime / time;
            pixelateMat.SetFloat("_pixelSize", Mathf.Lerp(startValue, targetAmount, progress));
            yield return null;
        }
    }
}
