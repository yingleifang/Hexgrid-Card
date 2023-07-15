using System;
using System.Collections.Generic;
using UnityEngine;

public class FeatureSelectedVisuals : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        Feature myFeature = transform.parent.GetComponentInChildren<Feature>();
        myFeature.FeatureSelected += FeatureSelectedVisuals_EnableMesh;
        myFeature.FeatureDeSelected += FeatureSelectedVisuals_DisableMesh;
        if (myFeature is HexUnit temp)
        {
            temp.GetMoveAction().StartMoving += FeatureSelectedVisuals_EnableMesh;
            temp.GetMoveAction().StartMoving += FeatureSelectedVisuals_DisableMesh;
        }
    }

    void FeatureSelectedVisuals_EnableMesh(object sender, EventArgs empty)
    {
        meshRenderer.enabled = true;
    }


    void FeatureSelectedVisuals_DisableMesh(object sender, EventArgs empty)
    {
        meshRenderer.enabled = false;
    }
}
