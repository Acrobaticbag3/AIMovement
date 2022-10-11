// This script was written by Kevin

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {
    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;

    private float yAxisRot = 0.0f; // rotation around the up/y axis
    private float xAxisRot = 0.0f; // rotation around the right/x axis

    void Start () {
        Vector3 rot = transform.localRotation.eulerAngles;
        yAxisRot = rot.y;
        xAxisRot = rot.x;
    }

    void Update () {
        float mouseAxisX = Input.GetAxis(axisName: "Mouse X");
        float mouseAxisY = -Input.GetAxis(axisName: "Mouse Y");

        yAxisRot += mouseAxisX * mouseSensitivity * Time.fixedDeltaTime;
        xAxisRot += mouseAxisY * mouseSensitivity * Time.fixedDeltaTime;

        xAxisRot = Mathf.Clamp(value: xAxisRot, min: -clampAngle, max: clampAngle);

        Quaternion localRotation = Quaternion.Euler(x: xAxisRot, y: yAxisRot, z: 0.0f);
        transform.rotation = localRotation;
    }
}
