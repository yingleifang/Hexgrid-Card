using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAction : MonoBehaviour
{
    protected HexUnit unit;
    protected virtual void Awake()
    {
        unit = GetComponent<HexUnit>();
    }
}
