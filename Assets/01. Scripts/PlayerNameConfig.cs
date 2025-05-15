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
            Debug.Log($"�̸� ���� ����: '{updated}'");
        }
        catch (AuthenticationException authEx)
        {
            Debug.LogError($"���� ����: {authEx.Message}");
        }
        catch (RequestFailedException reqEx)
        {
            Debug.LogError($"��û ����: {reqEx.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"����ġ ���� ����: {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        nameInputField.onEndEdit.RemoveListener(OnNameEditEnd);
    }
}