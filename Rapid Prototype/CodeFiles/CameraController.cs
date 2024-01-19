using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{


    // for move
    [SerializeField] private float panSpeed = 10f;
    

    // for zoom
    private float zoom;
    [SerializeField] private float scrollSpeed = 0f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;

    [SerializeField] private Camera cam; // to get orthographic size (size of window) for zoom


    void Start()
    {
        zoom = cam.orthographicSize; // sets current zoom lvl to default/start window size
    }

    // Update is called once per frame
    void Update()
    {

        // reacts to camera movement input
        MoveCam();

        // reacts to camera zoom (scroll) input
        ZoomCam();


    }

    void MoveCam() {

        

        float deltaX = 0f;
        float deltaY = 0f;

        if (Input.GetKey(KeyCode.A)) {

            // move left
            deltaX = 1f;
        }
        else if(Input.GetKey(KeyCode.D)) {

            // move right
            deltaX = -1f;
        }

        if (Input.GetKey(KeyCode.S)) {

            // move down
            deltaY = 1f;

        }
        
        else if(Input.GetKey(KeyCode.W)) {

            // move up
            deltaY = -1f;
        }

        Vector3 moveDir = new Vector3(deltaX, deltaY, 0f);
        transform.Translate(moveDir * panSpeed * Time.deltaTime, Space.World);

    }

    void ZoomCam() {

        float scroll = Input.GetAxis("Mouse ScrollWheel"); // mouseScrollDelta is a Vec2, scroll value is stored in y, other value (x) is ignored


        zoom += scroll * scrollSpeed * Time.deltaTime * 1000; // subtraction 
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom); // range check/clipping

        cam.orthographicSize = zoom;

    }


}
