using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ITriggerArea {
    bool InsideArea {get; set; }
    void OnAreaEnter(Player player);
    void OnAreaExit(Player player);
}
public interface ICollissionArea {
    bool InsideArea {get; set; }
    void OnColliderEnter(Player player);
    void OnColliderExit(Player player);
    void OnColliderStay(Player player);
}

[RequireComponent(typeof(Collider))]
public class AreaTrigger : MonoBehaviour, ITriggerArea
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
