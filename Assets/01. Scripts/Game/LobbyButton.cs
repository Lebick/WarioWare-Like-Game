using UnityEngine;

public class LobbyButton : MonoBehaviour
{
    public void ActiveObject(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void InActiveObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
