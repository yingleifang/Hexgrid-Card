using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseAction : NetworkBehaviour
{
    protected HexUnit unit;
    protected virtual void Awake()
    {
        unit = GetComponent<HexUnit>();
    }

    public void StartBlockingCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(BlockingCoroutine(coroutine));
    }
    IEnumerator BlockingCoroutine(IEnumerator coroutine)
    {
        GameUIHandler.Instance.disableCanvasGroupRayCast();
        yield return StartCoroutine(coroutine);
        GameUIHandler.Instance.enableCanvasGroupRayCast();
    }

}
