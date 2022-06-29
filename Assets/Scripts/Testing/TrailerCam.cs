using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerCam : MonoBehaviour, IRoomObject
{

    private Vector3 startPos;
    private Quaternion startRot;

    public Vector3 positionDelta;
    public Vector3 rotationDelta;
    public Room room;
    private Camera camera;

    public float acceleration = 0;
    private float speed = 1f;

    private bool isMoving = false;

    public bool InSpace { get;set; } = false;

    private void Awake() {
        camera = GetComponent<Camera>();
        ResetCam();
    }

    public void ResetCam() {
        startPos = transform.position;
        startRot = transform.rotation;
        isMoving = false;
    }
    public void ResetCamToPlayer() {
        // transform.position = player.Camera.transform.position;
        // transform.rotation = player.Camera.transform.rotation;
        ResetCam();
    }

    private void Update() {
        if (!InSpace) return;
        if (Input.GetKeyDown(KeyCode.C)) {
            if (isMoving) {
                Debug.Log("[CAM STOP]");
                if (camera != null) {
                    camera.enabled = false;
                    room.Player.Camera.enabled = true;
                }

                isMoving = false;
            } else {
                Debug.Log("[CAM START]");

                isMoving = true;
                speed  = 1f;
                if (camera != null) {
                    room.Player.Camera.enabled = false;
                    camera.enabled = true;
                    camera.gameObject.tag = "MainCamera";
                }
                transform.position = startPos;
                transform.rotation = startRot;
            }
        }
        if (Input.GetKeyDown(KeyCode.V)) {
            Debug.Log("[CAM RESET]");
            // ResetCam();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            Debug.Log("[CAM RESET ANIMATION]");
            ResetAnimation();
            // ResetCam();
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log("[CAM TO PLAYER]");
            // ResetCamToPlayer();
        }

        if (isMoving) {
            transform.localPosition += positionDelta * Time.deltaTime * speed;
            transform.Rotate(rotationDelta * Time.deltaTime * speed);
            speed += acceleration * Time.deltaTime;
        }
    }

    public void ResetAnimation() {
        isMoving = false;
        // StartCoroutine(TransformExtensions.AnimatingPos(transform, startPos, AnimationCurve.EaseInOut(0,0,1,1), 1f));
    }

    public void OnRoomEnter()
    {
        ResetCam();
        InSpace = true;
    }

    public void OnRoomLeave()
    {
        InSpace = false;
    }
}
