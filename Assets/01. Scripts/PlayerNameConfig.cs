using UnityEngine;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System;
using System.Threading.Tasks;
using Unity.Multiplayer.Widgets;

public class PlayerNameConfig : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;

    //private async Task WaitInitilization()
    //{
    //    while (!WidgetServiceInitialization.IsInitialized)
    //        await Task.Delay(100);
    //}

    private async void Start()
    {
        //await WaitInitilization();

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        try
        {
            string fullName = await AuthenticationService.Instance.GetPlayerNameAsync();

            nameInputField.text = fullName.Contains("#")
                ? fullName.Split('#')[0]
                : fullName;
        }
        catch (AuthenticationException authEx)
        {
            Debug.LogError($"�̸� ��ȸ ���� ����: {authEx.Message}");
        }
        catch (RequestFailedException reqEx)
        {
            Debug.LogError($"�̸� ��ȸ ��û ����: {reqEx.Message}");
        }


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