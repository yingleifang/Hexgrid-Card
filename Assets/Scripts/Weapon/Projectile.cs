using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float deltaRange = 0.01f;
    float speed = 2;
    public IEnumerator moveToTarget(Vector3 position)
    {
        while (Mathf.Abs(transform.position.z - position.z) > deltaRange)
        {
            transform.LookAt(position);
            transform.Translate(speed * Time.deltaTime * Vector3.forward);
            yield return null;
        }
    }
}
