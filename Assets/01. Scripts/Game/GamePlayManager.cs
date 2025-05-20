using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    public ScreenSplit screenSplit;

    public GameObject splitCamera;

    public GameObject lobbyCanvas;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
