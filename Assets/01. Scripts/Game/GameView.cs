using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    public Camera myCamera;

    private RenderTexture renderTexture;

    private void Start()
    {
        transform.parent = GamePlayManager.instance.roomParent;

        GamePlayManager.instance.screenSplit.ClientAmountSynchronization();
        ulong myId = GetComponent<NetworkObject>().OwnerClientId;

        transform.position = new Vector2(myId * 50, 0);

        renderTexture = new RenderTexture(Screen.width, Screen.height, 16);
        renderTexture.Create();

        myCamera.targetTexture = renderTexture;


        print((int)myId);
        GamePlayManager.instance.screenSplit.masks[(int)myId].GetComponentInChildren<RawImage>().texture = renderTexture;
    }

    private void OnDestroy()
    {
        if(renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }
}
