using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisuals : MonoBehaviour
{
    [SerializeField] private Feature feature;
    [SerializeField] Animator unitAnimator;

    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        unitAnimator = FindObjectOfType<Animator>();
        unitAnimator.fireEvents = false;
    }

    private void Start()
    {
        HexUnit myUnit = transform.parent.GetComponentInChildren<HexUnit>();
        GameManager.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        myUnit.GetMoveAction().StartMoving += UnitActionSystem_StartMoving;
        myUnit.GetMoveAction().StopMoving += UnitActionSystem_StopMoving;
        UpdateVisual();
    }

    void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty)
    {
        UpdateVisual();
    }

    void UnitActionSystem_StartMoving(object sender, EventArgs empty)
    {
        HexGrid.Instance.unitIsBusy = true;
        meshRenderer.enabled = false;
        unitAnimator.SetBool("isRunning", true);
    }


    void UnitActionSystem_StopMoving(object sender, EventArgs empty)
    {
        meshRenderer.enabled = true;
        unitAnimator.SetBool("isRunning", false);
        HexGrid.Instance.unitIsBusy = false;
    }

    void UpdateVisual()
    {
        if (GameManager.Instance.selectedFeature == feature)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }
}
