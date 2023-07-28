using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{    private void LateUpdate()
    {
        Vector3 dirToCamra = (Camera.main.transform.position - transform.position).normalized;
        transform.LookAt(Camera.main.transform.position + dirToCamra * -1);
    }
}
