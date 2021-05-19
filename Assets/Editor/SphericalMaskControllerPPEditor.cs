using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor (typeof (SphericalMaskPPController))]
public class SphericalMaskPPControllerEditor : Editor {
    private void OnSceneGUI () {
        SphericalMaskPPController controller = target as SphericalMaskPPController;
        Vector3 spherePosition = controller.spherePosition;
        EditorGUI.BeginChangeCheck ();
        spherePosition = Handles.DoPositionHandle (spherePosition, Quaternion.identity);
        if (EditorGUI.EndChangeCheck ()) {
            Undo.RecordObject (controller, "Move sphere pos");
            EditorUtility.SetDirty (controller);
            controller.spherePosition = spherePosition;
        }
 
        Handles.DrawWireDisc (controller.spherePosition, Vector3.up, controller.radius);
        Handles.DrawWireDisc (controller.spherePosition, Vector3.forward, controller.radius);
        Handles.DrawWireDisc (controller.spherePosition, Vector3.right, controller.radius);
    }
}
#endif