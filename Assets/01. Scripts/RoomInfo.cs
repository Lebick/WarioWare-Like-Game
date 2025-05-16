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
            // 호스트만 네트워크 비밀번호를 설정
            networkPassword.Value = hostPassword;
            Debug.Log("서버: 비밀번호 설정 완료");
        }

        if (IsClient && !IsHost)
        {
            // 클라이언트는 비밀번호가 변경될 때마다 알림 받음
            networkPassword.OnValueChanged += OnPasswordChanged;

            // 클라이언트는 접속 즉시 비밀번호 요청
            RequestPasswordServerRpc();

            Debug.Log("클라이언트: 비밀번호 요청 중...");
        }
    }

    private void OnPasswordChanged(string previousValue, string newValue)
    {
        Debug.Log($"비밀번호 받음: {newValue}");
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

    // 서버가 클라이언트에게 비밀번호 전송
    [ClientRpc]
    private void SendPasswordClientRpc(string password, ClientRpcParams clientRpcParams = default)
    {
        if (!IsServer)
        {
            Debug.Log($"서버로부터 비밀번호 받음: {password}");
            // 여기서 클라이언트가 비밀번호를 저장하거나 사용할 수 있습니다
        }
    }
}