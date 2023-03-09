using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBox : InteractabelObject
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private BlackScreenOverlay blackScreen;
    [SerializeField]
    private CreditsRoller creditsRoller;
    [SerializeField]
    private Transform cameeraTarget;

    [SerializeField]
    private Collider[] boxColliderders;

    public override void Interact()
    {
        base.Interact();
        Interactable = false;
        Focused = false;
        StartCoroutine(EndCutscene());
    }
    private bool moveCamera = false;

    public IEnumerator EndCutscene() {
        foreach(Collider c in boxColliderders) {
            c.enabled = false;
        }
        player.Movement.EnableWalk = false;
        player.Movement.RB.useGravity = false;
        player.Movement.EnableRotation = false;
        player.Movement.enabled = false;
        foreach(Transform prop in player.finalLevelEnd.props) {
            prop.gameObject.SetActive(false);
        }
        player.SetFinalLevelAnimation();
        float duration = 1f;
        StartCoroutine(player.transform.AnimatingPos(target.position, AnimationCurve.EaseInOut(0,0,1,1), duration));
        StartCoroutine(player.transform.AnimatingRotation(target.rotation, AnimationCurve.EaseInOut(0,0,1,1), duration));
        yield return new WaitForSeconds(duration);
    	ToggleMesh();
        yield return new WaitForSeconds(13f);
        player.Camera.transform.SetParent(player.transform.parent);
        moveCamera = true;
        StartCoroutine(MoveCamera());
        yield return new WaitForSeconds(7f);
        moveCamera = false;
        creditsRoller.StartRolling();
    }
    public IEnumerator MoveCamera() {
        float speed = 0;
        Vector3 delta = (cameeraTarget.position - player.Camera.transform.position).normalized;
        while (moveCamera) {
            speed = Mathf.Min(.1f, speed + Time.deltaTime * .01f);
            player.Camera.transform.position += delta * speed;
            yield return new WaitForEndOfFrame();
        }
    }
    private void Start() {
        InteractableDistance = 4f;
    }
    private void Update() {
        if (Interactable) Focused = true;
    }

    public void ToggleMesh() {
        foreach(MeshRenderer m in GetComponentsInChildren<MeshRenderer>()) {
            m.enabled = false;
        }
        foreach(Transform prop in player.finalLevelEnd.props) {
            prop.gameObject.SetActive(true);
        }
    }
}
