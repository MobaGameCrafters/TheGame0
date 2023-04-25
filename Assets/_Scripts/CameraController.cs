using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour {

     [SerializeField] private CinemachineVirtualCamera followCamera;
    private readonly float cameraSpeed = 100f;
    private readonly float borderThickness = 10f;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void Update()
    {
        if (!Application.isFocused) return;

        Vector3 currentPosition = transform.position;


        if (Input.mousePosition.x >= Screen.width - borderThickness)
        {
            currentPosition.x += cameraSpeed * Time.deltaTime;
        }
        else if (Input.mousePosition.x <= borderThickness)
        {
            currentPosition.x -= cameraSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y >= Screen.height - borderThickness)
        {
            currentPosition.z += cameraSpeed * Time.deltaTime;
        }
        else if (Input.mousePosition.y <= borderThickness)
        {
            currentPosition.z -= cameraSpeed * Time.deltaTime;
        }
        transform.position = currentPosition;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

    }
    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}