using UnityEngine;
using Unity.Netcode;

public class TestCharacter : MonoBehaviour
{
    private NetworkObject networkObject;

    private void Start()
    {
        networkObject = GetComponent<NetworkObject>();
    }

    private void Update()
    {
        if (networkObject.IsOwner)
        {
            UpdateMove();
        }
    }

    private void UpdateMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(h, v).normalized;

        transform.position += (Vector3)dir * Time.deltaTime;

    }
}
