using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;

public class MultiRoom : NetworkBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private const int MAX_PLAYERS = 4;
    private const string RELAY_JOIN_CODE_KEY = "RelayJoinCode";
    private string lobbyId;
    private Lobby currentLobby;
    private float lobbyUpdateTimer;
    private const float LOBBY_UPDATE_INTERVAL = 1.5f;
    private int myIndex = -1;

    private void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        joinButton.onClick.AddListener(StartClient);
        _ = InitializeUnityServices();
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            statusText.text = "서비스 초기화 완료";
        }
        catch (System.Exception)
        {
            statusText.text = "서비스 초기화 실패";
        }
    }

    private async void StartHost()
    {
        try
        {
            // Relay 할당 생성
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYERS);
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // 로비 생성
            string lobbyName = $"Room_{Random.Range(0, 10000)}";
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { RELAY_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode) }
                },
                IsPrivate = true,
                Player = GetPlayer()
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYERS, options);
            lobbyId = currentLobby.Id;

            // Netcode 설정
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );
            
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            
            NetworkManager.Singleton.StartHost();
            myIndex = 0;
            
            statusText.text = $"방 생성 완료\n참가 코드: {relayJoinCode}";
        }
        catch (System.Exception)
        {
            statusText.text = "방 생성 실패";
        }
    }

    private async void StartClient()
    {
        try
        {
            string joinCode = joinCodeInput.text.ToUpper();
            if (string.IsNullOrEmpty(joinCode))
            {
                statusText.text = "참가 코드를 입력하세요";
                return;
            }

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            NetworkManager.Singleton.StartClient();
            statusText.text = "연결 중...";
        }
        catch (System.Exception)
        {
            statusText.text = "참가 실패";
        }
    }

    private void Update()
    {
        UpdateLobby();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerEvent();
        }
    }

    private async void UpdateLobby()
    {
        if (string.IsNullOrEmpty(lobbyId) || currentLobby == null) return;

        lobbyUpdateTimer += Time.deltaTime;
        if (lobbyUpdateTimer >= LOBBY_UPDATE_INTERVAL)
        {
            lobbyUpdateTimer = 0;
            try
            {
                currentLobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
            }
            catch (System.Exception) { }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            myIndex = 0;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            myIndex = (int)clientId;
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            myIndex = -1;
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerIndex", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, myIndex.ToString()) }
            }
        };
    }

    private void TriggerEvent()
    {
        if (!NetworkManager.Singleton.IsConnectedClient || myIndex < 0) return;
        TriggerEventServerRpc(myIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TriggerEventServerRpc(int index)
    {
        TriggerEventClientRpc(index);
    }

    [ClientRpc]
    private void TriggerEventClientRpc(int index) { }

    private async void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        if (!string.IsNullOrEmpty(lobbyId) && currentLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch (System.Exception) { }
        }
    }
}