using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCanvas : MonoBehaviour
{
    private CanvasGroup cg;

    private void Awake() {
        cg = GetComponent<CanvasGroup>();
    }

    private void Update() {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftControl))  
            cg.alpha = cg.alpha == 1 ? 0 : 1;
#endif
        
    }
}
