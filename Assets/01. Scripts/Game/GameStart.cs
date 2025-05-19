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
    }
}
