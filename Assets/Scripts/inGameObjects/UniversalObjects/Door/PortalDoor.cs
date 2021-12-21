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
        if (delayDeactivationCoroutine != null) StopCoroutine(delayDeactivationCoroutine);
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

    private void LateUpdate() {
        if (!inSpace || Door.IN_WALKING_ANIMATION) return;

        if (connectedDoor == null || connectedDoor.room == null) return;
        if (room.Player == null) return;

        if (portal.IncameraRange() && !locked && Vector3.Distance(room.Player.transform.position, transform.position) < 40f) {
            if (!connectedDoor.room.gameObject.activeSelf) 
            {
                connectedDoor.room.gameObject.SetActive(true);
            }
        } else {
            if (connectedDoor.room.gameObject.activeSelf) {
                connectedDoor.room.gameObject.SetActive(false);
            }
        }
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
        delayDeactivationCoroutine = StartCoroutine(Delaydeactivation());
    }
    private IEnumerator Delaydeactivation() {
        while (IN_WALKING_ANIMATION) {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(3f);
        SetPortalState(false);
    }
}
