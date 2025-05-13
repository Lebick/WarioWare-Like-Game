using UnityEngine;
using System.Collections;

public class ReadyScreen : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(PrintMessage());
    }

    private IEnumerator PrintMessage()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("백학도령 멍청이");
    }
}
