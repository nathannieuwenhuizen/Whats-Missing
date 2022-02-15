using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKPass : MonoBehaviour
{
    [SerializeField]
    private FPMovement movement;
    protected Animator animator;
    
    private bool ikActive = true;
    private Vector3 lookPosition; 


    private IKHand rightHand;
    private IKHand leftHand;

    private float headDuration = .5f;

    void Awake () 
    {
        Debug.Log("awake");
        animator = GetComponent<Animator>();
        rightHand = gameObject.AddComponent<IKHand>();
        leftHand = gameObject.AddComponent<IKHand>();
    }

    private void Start() {
        rightHand.Setup(transform,animator, movement.RB, true);
        leftHand.Setup(transform,animator, movement.RB, false);
    }

    public bool IKActive {
        get { return ikActive;}
        set { 
            ikActive = value; 
            leftHand.IsActive = value;
            rightHand.IsActive = value;
            StartCoroutine(TogglingHeadIK(value ? 1 : 0));
        }
    }

    public void SetHeadDirection(Transform cam) {
        lookPosition = cam.transform.position + cam.transform.forward * 2f;
    }
    private float lookWeight = 1f;
    public float LookWeight {
        get { return lookWeight;}
        set { 
            lookWeight = value;
            animator.SetLookAtWeight(value); 
        }
    }

    private IEnumerator TogglingHeadIK(float end) {
        float index = 0;
        float start = LookWeight;
        AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
        while (index < headDuration) {
            index += Time.deltaTime;
            headDuration = Mathf.Lerp(start,end, curve.Evaluate(index / headDuration));
            yield return new WaitForEndOfFrame();
        }
        LookWeight = end;
    }



    public Vector3 getShoulderPos(bool rightShoulder = true) {
        Vector3 temp = IKHand.SHOULDER_OFFSET;
        if (rightShoulder == false) temp.x *= -1;
        return transform.position + transform.TransformDirection(temp) + transform.forward;
    }

    
    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if(animator) {
            //if the IK is active, set the position and rotation directly to the goal. 
            if(ikActive) {

                animator.SetLookAtWeight(1);
                animator.SetLookAtPosition(lookPosition);
                
                // Set the right hand target position and rotation, if one has been assigned
                rightHand.RaycastWall();
                // leftHand.RaycastWall();
                rightHand.updateIK();
                // leftHand.updateIK();
                
            }
        }
    }    

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(lookPosition, .1f);
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(getShoulderPos(true), getShoulderPos(true) + transform.forward * IKHand.HAND_RANGE);
        Gizmos.DrawLine(getShoulderPos(false), getShoulderPos(false) + transform.forward * IKHand.HAND_RANGE);
    }
}
