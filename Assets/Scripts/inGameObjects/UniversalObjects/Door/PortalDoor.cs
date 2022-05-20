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

    public override Transform GetKnob(bool start)
    {
        float delta = Vector3.Distance(Camera.main.transform.position, transform.position);
        float deltaConnectedDoor = Vector3.Distance(Camera.main.transform.position, connectedDoor.transform.position);
        if (delta < deltaConnectedDoor) return base.GetKnob(start);
        else return connectedDoor.GetKnob(start);
    }

    public override bool Locked { 
        get => base.Locked; 
        set {
            if (connectedDoor) {
                connectedDoor.locked = value;
                connectedDoor.DoorAnimator.enabled = value;
            }
 
            base.Locked = value; 
            SetPortalState(!value);
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
        if ((WentThroughPortal(player) || precentage == 2) && connectedDoor != null) connectedDoor.UpdatePlayerWalkingPosition(Mathf.Min(1, precentage), player);
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

    public override void OnWalkingEnd(Player player)
    {
        if (!WentThroughPortal(player)) {
            portal.Teleport(player);
        }
        base.OnWalkingEnd(player);
    }

    private void LateUpdate() {
        if (!inSpace || Door.IN_WALKING_ANIMATION) return;

        if (connectedDoor == null || connectedDoor.room == null) return;
        if (room.Player == null) return;

        if (portal.IncameraRange() && (!locked && Vector3.Distance(room.Player.transform.position, transform.position) < 150f)) {
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
        SetPortalState(!locked);
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
