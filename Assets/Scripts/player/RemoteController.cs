using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteController : MonoBehaviour
{

    [SerializeField]
    private float remoteDistance = 5f;

    private Television focusedTelevision;

    private Television oldTelevision;

    private bool isEnabled = true;

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) return;

        focusedTelevision = FocussedTelevision();
        if (focusedTelevision == null) {
            if (oldTelevision) oldTelevision.IsInteractable = false;
        } else {
            if (oldTelevision != focusedTelevision)  {
                if (oldTelevision) oldTelevision.IsInteractable = false;
                focusedTelevision.IsInteractable = true;
            }
        }
        oldTelevision = focusedTelevision;

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
        oldTelevision = null;
        if (focusedTelevision != null) {
            focusedTelevision.IsInteractable = false;
            focusedTelevision = null;
        }
    }


    private Television FocussedTelevision() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, remoteDistance)) {
            if (hit.rigidbody) if (hit.rigidbody.gameObject.GetComponent<Television>() != null) return hit.rigidbody.gameObject.GetComponent<Television>();
        }
        return null;
    }

}
