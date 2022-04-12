using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingIsland : RoomObject, ITriggerArea
{
    public bool InsideArea { get; set; }
    // private Transform oldParent;

    [SerializeField]
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Reset() {
        Word = "ground";
        AlternativeWords = new string[] { "island" };
    }

    public void OnAreaEnter(Player player)
    {
        // if (oldParent == transform) return;
        
        // oldParent = player.transform.parent;
        animator.speed = 0;
        // player.transform.SetParent(transform);
        AudioHandler.Instance?.FadeMusic(MusicFiles.planetarium_hidden_room, 1f);

    }

    public void OnAreaExit(Player player)
    {
        animator.speed = 1;
        // player.transform.SetParent(oldParent);
        AudioHandler.Instance?.FadeMusic(MusicFiles.garden, .5f);
    }
}
