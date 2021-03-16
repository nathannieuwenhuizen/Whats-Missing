using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update

    [HideInInspector]
    public  Room room;

    public delegate void PassingDoorAction(Door door);
    public static event PassingDoorAction OnPassingThrough;
    private bool locked = true;

    private float openAngle = -20f;
    private float wideAngle = -135f;
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

    public bool Focused { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

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
        //TODO: Go to next room with player
        OnPassingThrough(this);
        transform.rotation = Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y + wideAngle, startRotation.eulerAngles.z);
    }

    public void Interact()
    {
        Debug.Log("try to open");
        if (locked) return;
        Opening();
    }
}
