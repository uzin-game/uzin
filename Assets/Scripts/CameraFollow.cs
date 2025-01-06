using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float followspeed = 2f;
    public float yOffset = 1f;
    public Transform target;

    Camera cam;
    public float zoom = 5f;

    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //camera movement
        Vector3 newPos = new Vector3(target.position.x,target.position.y + yOffset,-10f);
        transform.position = Vector3.Slerp(transform.position,newPos,followspeed*Time.deltaTime);

        //camera panning
        if (cam.orthographic)
        {
            cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * zoom;
        }
        else
        {
            cam.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * zoom;
        }
    }
}
