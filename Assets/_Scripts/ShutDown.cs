using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShutDown : MonoBehaviour
{
    // Start is called before the first frame update
    public void Shutdown()
    {
        // Stop any running coroutines
        StopAllCoroutines();

        // Disconnect from any network connections

        // Destroy any instantiated game objects
        foreach (NetworkObject obj in NetworkObject.FindObjectsOfType<NetworkObject>())
        {
            obj.Despawn();
        }
        NetworkManager.Singleton.Shutdown();

        // Quit the application
        Application.Quit();
    }
}
