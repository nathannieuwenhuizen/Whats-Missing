using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IslandType {
    main,
    windmill,
    grave
}
public class FloatingIsland : RoomObject, ITriggerArea, IRoomObject
{
    public delegate void OnRoomEnterEvent(FloatingIsland _floatingIsland);

    public static OnRoomEnterEvent OnRoomEntering;

    [Space]
    [Header("island info")]
    [SerializeField]
    private Room room;
    public Room Room {
        get { return room;}
    }
    public IslandType IslandType;

    [SerializeField]
    private Renderer[] renderers;
    public Renderer[] Renderers {
        get { return renderers;}
    }

    public bool InsideArea { get; set; }
    // private Transform oldParent;

    private Animator animator;

    protected override void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Reset() {
        Word = "ground";
        AlternativeWords = new string[] { "island" };
    }
    public override void OnRoomEnter() {
        base.OnRoomEnter();
        animator.Rebind();
        animator.Update(0f);
        OnRoomEntering?.Invoke(this);
    }

    

    public void OnAreaEnter(Player player)
    {
        // if (oldParent == transform) return;
        
        // oldParent = player.transform.parent;
        animator.speed = 0;
        // player.transform.SetParent(transform);
        AudioHandler.Instance?.FadeMusic(MusicFiles.hidden_room, 1f);

    }

    public void OnAreaExit(Player player)
    {
        animator.speed = 1;
        // player.transform.SetParent(oldParent);
        AudioHandler.Instance?.FadeMusic(MusicFiles.garden, .5f);
    }
}
