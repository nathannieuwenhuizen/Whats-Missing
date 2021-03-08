using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteController : MonoBehaviour
{

    [SerializeField]
    private float remoteDistance = 5f;

    private Television focusedTelevision;

    private Television oldTelevision;
    // Update is called once per frame
    void Update()
    {

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

    private Television FocussedTelevision() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, remoteDistance)) {
            if (hit.rigidbody) if (hit.rigidbody.gameObject.GetComponent<Television>() != null) return hit.rigidbody.gameObject.GetComponent<Television>();
        }
        return null;
    }

}
