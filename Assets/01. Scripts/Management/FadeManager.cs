using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class FadeManager : Singleton<FadeManager>
{
    public GameObject parent;

    public Image fadeImage;
    private Material fadeMaterial;

    private IEnumerator fadeCoroutine;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        fadeMaterial = fadeImage.material;
        fadeMaterial.SetFloat("_Progress", 0);
        parent.SetActive(false);
    }

    /// <summary>
    /// true : 가림
    /// false : 풀림
    /// </summary>
    public void SetFade(bool value, float time = 1, Action endAction = null)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = Fading(value ? 1 : 0, time, endAction);
        StartCoroutine(fadeCoroutine);
    }

    private IEnumerator Fading(float endValue, float time, Action endAction)
    {
        parent.SetActive(true);

        float start = fadeMaterial.GetFloat("_Progress");

        float progress = 0f;

        while(progress <= 1f)
        {
            progress += Time.deltaTime / time;

            float value = Mathf.Lerp(start, endValue, progress);

            value = 1 - Mathf.Pow(1 - value, 3);

            fadeMaterial.SetFloat("_Progress", value);

            yield return null;
        }

        parent.SetActive(endValue != 0);

        endAction?.Invoke();

        yield break;
    }
}
