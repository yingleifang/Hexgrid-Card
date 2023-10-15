using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR) 
namespace UserInterface.Editor
{
    [CustomEditor(typeof(UICameraPositioner))]
    public class UiCameraPositionerEditor : UnityEditor.Editor
    {
        private UICameraPositioner Target => (UICameraPositioner)target;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Set Position"))
            {
                Target.SetCameraPosition();
            }
        }
    }
}
#endif