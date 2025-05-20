using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    public ScreenSplit screenSplit;

    public GameObject splitCamera;
    public RectTransform splitCanvas;

    public GameObject lobbyCanvas;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
