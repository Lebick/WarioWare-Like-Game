using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Unity.Netcode;


public class LoadingScreen : MonoBehaviour
{
    private IEnumerator loadingCoroutine;

    public GameObject parent;
    public TextMeshProUGUI loadingText;

    public void SetLoading(bool value)
    {
        if (value)
        {
            loadingCoroutine = Loading();
            StartCoroutine(loadingCoroutine);
        }
        else
        {
            if (loadingCoroutine != null)
                StopCoroutine(loadingCoroutine);
        }

        parent.SetActive(value);
    }

    private IEnumerator Loading()
    {
        while (true)
        {
            loadingText.text = "로딩중.";
            yield return new WaitForSeconds(1f);
            loadingText.text = "로딩중..";
            yield return new WaitForSeconds(1f);
            loadingText.text = "로딩중...";
            yield return new WaitForSeconds(1f);
        }
    }
}
