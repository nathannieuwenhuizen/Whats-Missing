using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteControllerObject : MonoBehaviour, IInteractable
{

    [SerializeField]
    private IntroSceneHandeler handeler;

    public bool Focused { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Interact()
    {
        handeler.PickUpRemote();
        gameObject.SetActive(false);
    }
}
