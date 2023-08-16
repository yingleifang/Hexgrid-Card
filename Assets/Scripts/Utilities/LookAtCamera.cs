using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Camera lookAtCamera;
    private void Awake()
    {
        lookAtCamera = Camera.main;
    }
    private void LateUpdate()
    {
        Vector3 dirToCamra = transform.position - Camera.main.transform.position;
        transform.LookAt(lookAtCamera.transform.position - dirToCamra);
    }
}
