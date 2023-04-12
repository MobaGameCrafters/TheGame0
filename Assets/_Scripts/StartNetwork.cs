using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class StartNetwork : MonoBehaviour
{
    // Start is called before the first frame update

    public void StartServer() {
        NetworkManager.Singleton.StartServer();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
}
