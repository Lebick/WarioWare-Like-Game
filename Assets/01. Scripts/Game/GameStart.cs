using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameStart : NetworkBehaviour
{
    public GameObject gameView;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        var instance = Instantiate(gameView, Vector3.zero, Quaternion.identity);
        var netObj = instance.GetComponent<NetworkObject>();
        netObj.Spawn();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            NetworkManager.Singleton.SceneManager.LoadScene("GameStartScene", LoadSceneMode.Single);

        if (Input.GetKeyDown(KeyCode.P))
            FadeManager.instance.SetFade(true, 2f, () =>
            {
                FadeManager.instance.SetFade(false, 2f);
            });
    }

    public void RoomJoin()
    {
        FadeManager.instance.SetFade(true, 2f, () =>
        {
            GamePlayManager.instance.lobbyCanvas.SetActive(false);
            GamePlayManager.instance.splitCamera.SetActive(true);
            FadeManager.instance.SetFade(false, 2f);
        });
    }
}
