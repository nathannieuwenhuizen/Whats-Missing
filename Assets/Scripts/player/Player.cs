using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : RoomObject
{

    [SerializeField]
    private Camera playerCamera;

    public Camera Camera { get => playerCamera;}
}
