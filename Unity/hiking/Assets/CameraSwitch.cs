using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField]
    private GameObject CameraFps;
    [SerializeField]
    private GameObject CameraBack;
    [SerializeField]
    private GameObject CameraSide;

    // Start is called before the first frame update
    void Start()
    {
        CameraFps.SetActive(true);
        CameraBack.SetActive(false);
        CameraSide.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            Debug.Log("Switch to FPS Camera");
            CameraFps.SetActive(true);
            CameraBack.SetActive(false);
            CameraSide.SetActive(false);
        }
        if (Input.GetKeyDown("2"))
        {
            Debug.Log("Switch to Back Camera");
            CameraFps.SetActive(false);
            CameraBack.SetActive(true);
            CameraSide.SetActive(false);
        }
        if (Input.GetKeyDown("3"))
        {
            Debug.Log("Switch to Side Camera");
            CameraFps.SetActive(false);
            CameraBack.SetActive(false);
            CameraSide.SetActive(true);
        }
    }
}
