using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIHandler : MonoBehaviour
{
    public static GameUIHandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TurnSystem!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [SerializeField]
    CanvasGroup canvasGroup;
    public void disableCanvasGroupRayCast()
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void enableCanvasGroupRayCast()
    {
        canvasGroup.blocksRaycasts = true;
    }
}
