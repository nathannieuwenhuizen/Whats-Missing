using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickableRoomObject : RoomObject, IPickable
{

    private Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public bool focused { get; set;} = false;
    public Rigidbody rigidBody { 
        get {
            return rb;
        } 
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
