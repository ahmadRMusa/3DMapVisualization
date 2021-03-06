﻿using UnityEngine;

public class ViewController : MonoBehaviour {
    /* MoveSpeed, RotateSpeed and ZoomSpeed can be floats in range (0..infinity>, 
     * ZoomFactor can be float in range (0..1)
     */
    public float MoveSpeed, RotateSpeed, ZoomFactor, ZoomCap, SlowdownFactor;
    public GameObject pivotPoint;

    private Rigidbody rb;
    private Vector3 gotoPosition, gotoZoom;

    void Start() {
        rb = pivotPoint.GetComponent<Rigidbody>();
        gotoPosition = pivotPoint.transform.position;
        gotoZoom = transform.localPosition;
	}

    
    void FixedUpdate() {
        // On every physics calculation, take input and readjust camera and its pivot

        Vector3 mouseVector = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); // get screen mouse vector

        Vector3 worldMouseVector = pivotPoint.transform.TransformDirection(mouseVector); // convert into world coordinates
        worldMouseVector = new Vector3(-worldMouseVector.x, worldMouseVector.y, 0f);

        float mouseWheel = Input.mouseScrollDelta.y; // get mouse scroll input


        float rot = transform.rotation.eulerAngles.z; // stabilize z axis rotation
        transform.Rotate(Vector3.forward, -rot);

        Vector3 pos = transform.localPosition;

        if (pos.magnitude > 0.0001f) { 
            // interpolate camera position, if needed
            transform.localPosition = Vector3.Lerp(pos, gotoZoom, Time.deltaTime * SlowdownFactor);
        } else {
            transform.localPosition = gotoZoom;
        }

        Vector3 pppos = pivotPoint.transform.position;
        
        if ((pppos - gotoPosition).magnitude > 0.0001f) {
            // interpolate camera position, if needed
            pivotPoint.transform.position = Vector3.Lerp(pppos, gotoPosition, Time.deltaTime * SlowdownFactor);
        } else {
            pivotPoint.transform.position = gotoPosition;
        }

        if (Input.GetMouseButton(0)) { 
            // if left mouse button, set new pivot position
            float distanceFactor = transform.position.magnitude / 100f;
            gotoPosition += worldMouseVector * MoveSpeed * distanceFactor * -1; // inverted axees
        } else if (Input.GetMouseButton(1)) { 
            // if right mouse button, adjust target rotation of pivot
            Vector3 rotateForce = mouseVector * RotateSpeed * -1;
            rb.AddTorque(rotateForce.y, rotateForce.x, 0f);
        } else if (Mathf.Abs(mouseWheel) > 0.0001f) {
            // if mouse wheel is scrolling, adjust camera-pivot zoom
            if (pos.magnitude >= ZoomCap || mouseWheel < 0f) { // dont reverse the objects, cap max zoom
                float distanceFactor = (1f + (ZoomFactor * mouseWheel * -1));
                gotoZoom *= distanceFactor;
            }
        }
    }
}
