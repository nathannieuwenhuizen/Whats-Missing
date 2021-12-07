using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class HintButton : MonoBehaviour, IPointerEnterHandler, IRoomObject
{

    private Animator animator;

    public bool InSpace {get; set;} = false;

    private void Awake() {
        animator = GetComponent<Animator>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        MakeButtonIdle();
    }

    private void MakeButtonIdle() {
        if (InSpace)
            animator.SetTrigger("Hint_Idle");
    }

    private void OnEnable() {
        Room.OnRoomComplete += MakeButtonIdle;
    }
    private void OnDisable() {
        Room.OnRoomComplete -= MakeButtonIdle;
    }


    public void OnRoomEnter()
    {
        InSpace = true;
    }

    public void OnRoomLeave()
    {
        InSpace = false;
    }
}
