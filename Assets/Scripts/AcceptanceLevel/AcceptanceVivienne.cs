using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceptanceVivienne : MonoBehaviour
{
    [SerializeField]
    private float minDistnaceToPlayer = 5f;
    [SerializeField]
    private float maxDistnaceToPlayer = 20f;
    [SerializeField]
    private Player player;
    [SerializeField]
    private float distancePercentage;


    public float currentDesiredDistance {
        get { return minDistnaceToPlayer + (maxDistnaceToPlayer - minDistnaceToPlayer) * distancePercentage;}
    }
    public bool playerMovesTowardsVivienne {
        get { return player.Movement.RB.velocity.x < -0.5f; }
    }
    private void Start() {
        distancePercentage = 1f;
        transform.position = new Vector3(player.transform.position.x,transform.position.y, transform.position.z) + 
        new Vector3(-1,0,0) * currentDesiredDistance;
    }
    private bool hasSaidSomething = false;
    private void Update() {
        if (!playerMovesTowardsVivienne) {
            distancePercentage = Mathf.Min(1, distancePercentage + Time.deltaTime * .05f);
        } else {
            distancePercentage = Mathf.Max(0, distancePercentage - Time.deltaTime *.1f);
        }
        transform.position = Vector3.Lerp(transform.position, 
        new Vector3(player.transform.position.x,transform.position.y, transform.position.z) + 
        new Vector3(-1,0,0) * currentDesiredDistance, Time.deltaTime * 2f);
        if (playerMovesTowardsVivienne) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, -90, 0), Time.deltaTime);
        
        player.IKPASS.RightHand.ReachHandForward = Vector3.Distance(transform.position, player.transform.position) < 25f;
        if (!hasSaidSomething && player.IKPASS.RightHand.ReachHandForward) {
            hasSaidSomething = true;
            DialoguePlayer.Instance.PlayLine( new Line() {
                text = "VIVIENNE!!!",
                duration = 1f,
                lineEffect = LineEffect.shake
            });
        }
        
    }
}
