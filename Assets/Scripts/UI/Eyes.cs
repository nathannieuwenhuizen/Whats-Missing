using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyes : MonoBehaviour
{
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void CloseEyes() {
        animator.SetTrigger("closing");
        Debug.Log("should be closing eyes!");
    }

    private void OnEnable() {
        TeddyBear.OnTeddyBearPickUp += CloseEyes;
    }
    private void OnDisable() {
        TeddyBear.OnTeddyBearPickUp -= CloseEyes;
    }
}
