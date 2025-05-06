using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : Singleton<Fade>
{
    public Image fadeImage;

    private IEnumerator currentCoroutine;

    public void SetFade(float start, float end, float time)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = Fading(start, end, time);
        StartCoroutine(currentCoroutine);
    }

    private IEnumerator Fading(float start, float end, float time)
    {
        float progress = 0f;

        Color startColor = fadeImage.color;
        Color endColor = fadeImage.color;
        startColor.a = start;
        endColor.a = end;

        while(progress <= 1f)
        {
            progress += Time.deltaTime / time;
            fadeImage.color = Color.Lerp(startColor, endColor, progress);
            yield return null;
        }
    }
}
