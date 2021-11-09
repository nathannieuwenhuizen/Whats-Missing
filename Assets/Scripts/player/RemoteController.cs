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
        if (focusedMirror == null) {
            if (oldMirror) oldMirror.MirrorCnvas.IsInteractable = false;
        } else {
            if (oldMirror != focusedMirror)  {
                if (oldMirror) oldMirror.MirrorCnvas.IsInteractable = false;
                focusedMirror.MirrorCnvas.IsInteractable = true;
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
            focusedMirror.MirrorCnvas.IsInteractable = false;
            focusedMirror = null;
        }
    }


    private Mirror FocussedMirror() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, remoteDistance)) {
            if (hit.rigidbody) if (hit.rigidbody.gameObject.GetComponent<Mirror>() != null) return hit.rigidbody.gameObject.GetComponent<Mirror>();
        }
        return null;
    }

}
