using UnityEngine;
using UnityEngine.Networking;
using System.Threading;

[RequireComponent(typeof(NetworkManager))]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AutoConnect : MonoBehaviour
{

    NetworkManager manager;
    // Use this for initialization
    bool isEditor = false;
    void Start()
    {
        manager = GetComponent<NetworkManager>();
#if UNITY_EDITOR
        isEditor = true;
#endif
    }

    void Update()
    {
        if (!NetworkClient.active && !NetworkServer.active && isEditor)
        {
            manager.StartHost();
        }
        if (!NetworkClient.active && !NetworkServer.active && !isEditor)
        {
            manager.StartClient();
        }
        if (NetworkClient.active && !ClientScene.ready && manager.client.connection != null)
        {
            ClientScene.Ready(manager.client.connection);

            if (ClientScene.localPlayers.Count == 0)
            {
                ClientScene.AddPlayer(0);
            }
        }
        if (ClientScene.ready)
        {
            enabled = false;
        }
    }
}
