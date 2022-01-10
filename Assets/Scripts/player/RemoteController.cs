using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteController : MonoBehaviour
{

    [SerializeField]
    private float remoteDistance = 5f;

    private Mirror focusedMirror;

    private Mirror oldMirror;

    private bool isEnabled = true;

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) return;

        focusedMirror = FocussedMirror();
        Debug.Log("focussed mirror: " + focusedMirror);
        if (focusedMirror == null) {
            if (oldMirror) oldMirror.MirrorCanvas.IsInteractable = false;
        } else {
            if (oldMirror != focusedMirror)  {
                if (oldMirror) oldMirror.MirrorCanvas.IsInteractable = false;
                Debug.Log("mirror focused!");
                focusedMirror.MirrorCanvas.IsInteractable = true;
            }
        }
        oldMirror = focusedMirror;

    }
    private void OnEnable() {
        PauseScreen.OnPause += DisableClick;
        PauseScreen.OnResume += EnableClick;
    }

    private void OnDisable() {
        PauseScreen.OnPause -= DisableClick;
        PauseScreen.OnResume -= EnableClick;
    }

    private void EnableClick() {
        isEnabled = true;
    }
    private void DisableClick() {
        isEnabled = false;
        oldMirror = null;
        if (focusedMirror != null) {
            focusedMirror.MirrorCanvas.IsInteractable = false;
            focusedMirror = null;
        }
    }


    private Mirror FocussedMirror() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, remoteDistance)) {
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.GetComponentInParent<Mirror>() != null) return hit.collider.GetComponentInParent<Mirror>();
        }
        return null;
    }

}
