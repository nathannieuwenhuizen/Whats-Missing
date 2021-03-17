using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : Property
{

    [SerializeField]
    private Room room;

    public override void onMissing()
    {
        base.onMissing();
        foreach(Rigidbody rb in room.GetAllObjectsInRoom<Rigidbody>()) {
            rb.useGravity = false;
        }
    }
    public override void onAppearing()
    {
        base.onAppearing();
        foreach(Rigidbody rb in room.GetAllObjectsInRoom<Rigidbody>()) {
            rb.useGravity = true;
        }

    }
}
