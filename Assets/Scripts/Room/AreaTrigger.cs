using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaTrigger : MonoBehaviour
{
    private bool inArea;
    public bool InsideArea {
        get { return inArea; }
        set {inArea = value; }
    }

    public virtual void OnAreaEnter() {
        if (inArea) return;
        inArea = true;
    }

    public virtual void OnAreaExit() {
        if (!inArea) return;
        inArea = false;
    }
}
