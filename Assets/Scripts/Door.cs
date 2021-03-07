using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update

    private bool locked = true;

    private float openAngle = 20f;
    private Quaternion startRotation;
    public bool Locked {
        get { return locked; }
        set {
            locked = value;
            if (locked) {
                Close();
            } else {
                Open();
            }
        }
    }

    void Close() {
        transform.rotation = Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y, startRotation.eulerAngles.z);
    }

    void Open() {
        transform.rotation = Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y + openAngle, startRotation.eulerAngles.z);
    }
    void Start()
    {
        startRotation = transform.rotation;
    }

    void Opening() {
        if (locked) return;
        //TODO: Go to next room with player
    }

}
