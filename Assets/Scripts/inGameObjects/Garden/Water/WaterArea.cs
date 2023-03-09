using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterArea : MonoBehaviour, ITriggerArea, ICollissionArea
{
    public static bool ON_WATER_SURFACE = false;
    public static bool IN_WATER = false;
    public static Transform WATER_TRANSFORM;
    public static Collider WATER_COLLIDER;

    public bool InsideArea { get; set;} = false;

    [SerializeField]
    private bool zAxis = true;

    [SerializeField]
    private Collider waterSwimCollider;
    ///<summary>
    /// Collider that makes the palyer do a swim animation.
    ///</summary>
    public Collider WaterSwimCollider {
        get { return waterSwimCollider;}
    }

    public void OnAreaEnter(Player player)
    {
        ON_WATER_SURFACE = true;
        WATER_TRANSFORM = transform;
        WATER_COLLIDER = waterSwimCollider;
        AdjustSwimColliderHeight(player);
    }
    private void OnDisable() {
        WATER_COLLIDER = null;
        WATER_TRANSFORM = null;
        ON_WATER_SURFACE = false;
        IN_WATER = false;
    }

    public void OnAreaExit(Player player)
    {
        ON_WATER_SURFACE = false;
        WATER_TRANSFORM = null;
        WATER_COLLIDER = null;
    }

    public void OnColliderEnter(Player player)
    {
        IN_WATER = true;
        player.CharacterAnimationPlayer.SetInWater(true);
    }

    public void OnColliderExit(Player player)
    {
        IN_WATER = false;
        player.CharacterAnimationPlayer.SetInWater(false);
    }


    private void AdjustSwimColliderHeight(Player player) {
        float v = player.CurrentScale != 1 ? 5.5f : 4.3f;
        if (zAxis)
            waterSwimCollider.transform.localPosition =  new Vector3( 
                waterSwimCollider.transform.localPosition.x,
                waterSwimCollider.transform.localPosition.y,
                v * player.CurrentScale / -(float)transform.localScale.z
            );
        else
            waterSwimCollider.transform.localPosition =  new Vector3( 
                waterSwimCollider.transform.localPosition.x,
                v * player.CurrentScale / -(float)transform.localScale.y,
                waterSwimCollider.transform.localPosition.z
            );
        waterSwimCollider.gameObject.SetActive(true);

    }

    public void OnColliderStay(Player player)
    {
        // IN_WATER = true;
        // player.CharacterAnimationPlayer.SetInWater(true);
    }
}
