using UnityEngine;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System;

public class PlayerNameConfig : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;

    private void Start()
    {
        nameInputField.onEndEdit.AddListener(OnNameEditEnd);
    }

    private async void OnNameEditEnd(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return;

        try
        {
            string updated = await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
            Debug.Log($"이름 변경 성공: '{updated}'");
        }
        catch (AuthenticationException authEx)
        {
            Debug.LogError($"인증 오류: {authEx.Message}");
        }
        catch (RequestFailedException reqEx)
        {
            Debug.LogError($"요청 실패: {reqEx.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"예기치 않은 오류: {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        nameInputField.onEndEdit.RemoveListener(OnNameEditEnd);
    }
}