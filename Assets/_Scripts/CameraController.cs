using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera followCamera;

    private void Awake()
    {

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Debug.Log("Here");
            //   followCamera.Priority = 20;
            // followCamera.Follow = GetComponent;
            // gameObject.
            // followCamera.gameObject.SetActive(true);

        }
        if (Input.GetKeyUp(KeyCode.Space))
        {// followCamera.gameObject.SetActive(false);
         // followCamera.Priority = 0;
        }
    }

}