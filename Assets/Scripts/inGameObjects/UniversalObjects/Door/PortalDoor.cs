using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Door that also has a portal attached to it.
///</summary>
public class PortalDoor : Door
{
    private Coroutine delayDeactivationCoroutine;

    [SerializeField]
    private PortalDoor connectedDoor;
    public PortalDoor ConnectedDoor {
        get { return connectedDoor;}
        set { 
            connectedDoor = value; 
            Portal.connectedPortal = connectedDoor.Portal;
        }
    }

    [SerializeField]
    private Portal portal;
    public Portal Portal {
        get { return portal;}
    }

    private void SetPortalState(bool val) {
        portal.IsActive = val;
    }

    public override bool Locked { 
        get => base.Locked; 
        set {
            if (connectedDoor) connectedDoor.locked = value;
            base.Locked = value; 
        }
    }
    private void Awake() {
        SetPortalState(false);
    }

    public override void SetBezierPoints(Player player)
    {
        base.SetBezierPoints(player);
        if (connectedDoor != null) {
            connectedDoor.point0 = connectedDoor.transform.TransformPoint(transform.InverseTransformPoint(point0));
            connectedDoor.point1 = point1 == StartPos() ? connectedDoor.StartPos() : connectedDoor.EndPos();
            connectedDoor.point2 = point2 == StartPos() ? connectedDoor.StartPos() : connectedDoor.EndPos();
        }
    }
    public override void UpdatePlayerWalkingPosition(float precentage, Player player)
    {
        if (WentThroughPortal(player) && connectedDoor != null) connectedDoor.UpdatePlayerWalkingPosition(precentage, player);
        else base.UpdatePlayerWalkingPosition(precentage, player);
    }
    public override void SetDoorLocalRotation(Quaternion val)
    {
        if (connectedDoor != null) connectedDoor.DoorPivot.localRotation = val;
        base.SetDoorLocalRotation(val);
    }

    private bool WentThroughPortal(Player player) {
        return Vector3.Distance(transform.position, player.transform.position) > 15f;
    }

    public override void OnRoomEnter()
    {
        SetPortalState(true);
        base.OnRoomEnter();
    }

    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        if (delayDeactivationCoroutine != null) StopCoroutine(delayDeactivationCoroutine);
        delayDeactivationCoroutine = StartCoroutine(delaydeactivation());
    }
    private IEnumerator delaydeactivation() {
        yield return new WaitForSeconds(3f);
        SetPortalState(false);
    }
}
