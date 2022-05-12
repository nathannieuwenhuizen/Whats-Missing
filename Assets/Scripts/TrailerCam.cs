using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerCam : MonoBehaviour
{

    private Vector3 startPos;
    private Quaternion startRot;

    private bool isMoving = false;

    private void Awake() {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    private void Update() {
        // if (Input.GetKeyDown(KeyCode.C)) {
        //     if (isMoving) {
        //         isMoving = false;
        //     } else {
        //         isMoving = true;
        //         transform.position = startPos;
        //         transform.rotation = startRot;
        //     }
        // }

        if (isMoving) {
            transform.Translate(transform.right * Time.deltaTime * 1f);
            transform.Rotate(new Vector3(0,0,Time.deltaTime * 0.5f));
        }
    }
}
