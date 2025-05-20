using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameStart : NetworkBehaviour
{
    void Start()
    {

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
