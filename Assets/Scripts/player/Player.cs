using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : RoomObject
{

    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private Volume volume;

    public Volume Volume { get => volume; }

    public Camera Camera { get => playerCamera;}


    private FPMovement movement;
    public FPMovement Movement { get=> movement; }
    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<FPMovement>();
    }
}
