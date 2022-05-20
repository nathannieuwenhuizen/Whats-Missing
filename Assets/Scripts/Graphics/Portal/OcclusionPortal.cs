using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionPortal : MonoBehaviour, IPortal, IRoomObject
{
    private Portal connectedPortal;
    private Player player;

    public Room room;
    public IPortal ConnectedPortal { get => connectedPortal; }

    public bool InsidePortal { get; set; }
    public bool InSpace { get; set; }

    [SerializeField]
    private GameObject reflectionPlane;

    public void OnPortalEnter(Player _player)
    {
        InsidePortal = true;
        player = _player;
    }

    public void OnPortalLeave()
    {

        InsidePortal = false;
    }

    public void OnRoomEnter()
    {
        InSpace = true;
    }

    public void OnRoomLeave()
    {
        InSpace = false;
    }

    public void Teleport(Player _player)
    {
        player = _player;
    }

    private void Update() {
        if (IncameraRange()) {

        }
    }

    private bool inView = false;
    public bool InView {
        get { return inView;}
        set { 
            if (inView == value) return;

            inView = value; 
            if (inView) SetOcclusionMask();
            else RemoveOcclusionMask();
        }
    }

    public void SetOcclusionMask() {
        
    }
    public void RemoveOcclusionMask() {

    }

    private Camera mainCamera;
    public bool IncameraRange() {

        if (mainCamera == null) {
            mainCamera = Camera.main;
        }

        Renderer renderer = reflectionPlane.GetComponent<MeshRenderer>();
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if(GeometryUtility.TestPlanesAABB(planes, renderer.bounds)){
            return true;
        } else {
            return false;   
        }
    }

}
