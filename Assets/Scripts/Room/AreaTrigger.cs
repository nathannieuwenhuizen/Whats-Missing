using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaTrigger : MonoBehaviour
{
    private bool inArea;
    public bool InsideArea {
        get { return inArea; }
    }

    public virtual void OnAreaEnter() {
        inArea = true;
    }

    public virtual void OnAreaExit() {
        inArea = false;
    }
}
