using Unity.Netcode;
using UnityEngine;

public class RoomInfo : NetworkBehaviour
{
    private NetworkVariable<string> networkPassword = new NetworkVariable<string>(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);


    private string hostPassword = "secret123";

    public override void OnNetworkSpawn()
    {
        if (IsServer || IsHost)
        {
            // ȣ��Ʈ�� ��Ʈ��ũ ��й�ȣ�� ����
            networkPassword.Value = hostPassword;
            Debug.Log("����: ��й�ȣ ���� �Ϸ�");
        }

        if (IsClient && !IsHost)
        {
            // Ŭ���̾�Ʈ�� ��й�ȣ�� ����� ������ �˸� ����
            networkPassword.OnValueChanged += OnPasswordChanged;

            // Ŭ���̾�Ʈ�� ���� ��� ��й�ȣ ��û
            RequestPasswordServerRpc();

            Debug.Log("Ŭ���̾�Ʈ: ��й�ȣ ��û ��...");
        }
    }

    private void OnPasswordChanged(string previousValue, string newValue)
    {
        Debug.Log($"��й�ȣ ����: {newValue}");
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPasswordServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        SendPasswordClientRpc(hostPassword, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }

    // ������ Ŭ���̾�Ʈ���� ��й�ȣ ����
    [ClientRpc]
    private void SendPasswordClientRpc(string password, ClientRpcParams clientRpcParams = default)
    {
        if (!IsServer)
        {
            Debug.Log($"�����κ��� ��й�ȣ ����: {password}");
            // ���⼭ Ŭ���̾�Ʈ�� ��й�ȣ�� �����ϰų� ����� �� �ֽ��ϴ�
        }
    }
}