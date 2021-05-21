using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : RoomObject
{

    [SerializeField]
    private Camera playerCamera;

    public Camera Camera { get => playerCamera;}


    private FPMovement movement;
    public FPMovement Movement { get=> movement; }
    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<FPMovement>();
    }
}
