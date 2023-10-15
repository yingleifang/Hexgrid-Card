using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if (UNITY_EDITOR) 
namespace UserInterface
{
    public sealed class UICameraPositioner : MonoBehaviour
    {
        private Canvas _canvas;

        private Canvas Canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = transform.FindFirstComponentInAncestor<Canvas>();
                }

                return _canvas;
            }
        }

        private void Awake()
        {
            //SetCameraPosition();
        }
        public void SetCameraPosition()
        {
            var canvasHalfHeight = 0.5f * ((RectTransform)Canvas.transform).rect.height;
            var cameraHalfFieldOfView = 0.5f * Canvas.worldCamera.fieldOfView;
            var distance = -canvasHalfHeight / Mathf.Tan(cameraHalfFieldOfView * Mathf.Deg2Rad);
            transform.localPosition = new Vector3(0, 0, distance);
            transform.rotation = new Quaternion();
        }
    }
}
#endif