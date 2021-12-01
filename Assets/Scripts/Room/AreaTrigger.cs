using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class AreaTrigger : MonoBehaviour
{

    [SerializeField]
    private UnityEvent onAreaEnter;
    public UnityEvent OnAreaEnterEvent {
        get { return onAreaEnter;}
    }
    [SerializeField]
    private UnityEvent onAreaExit;
    public UnityEvent OnAreaExitEvent {
        get { return onAreaExit;}
    }

    private bool inArea;
    public bool InsideArea {
        get { return inArea; }
        set {inArea = value; }
    }

    public virtual void OnAreaEnter(Player player) {
        if (inArea) return;
        onAreaEnter?.Invoke();
        inArea = true;
    }

    public virtual void OnAreaExit(Player player) {
        if (!inArea) return;
        onAreaExit?.Invoke();
        inArea = false;
    }
}
