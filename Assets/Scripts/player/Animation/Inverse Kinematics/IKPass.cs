using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
/// Responsible for the IK update calls for the player character
///</summary>
public class IKPass : MonoBehaviour
{

    public delegate void IkPassEvent();
    public static IkPassEvent OnIKUpdate;

    [SerializeField]
    private FPMovement movement;

    [SerializeField]
    private Transform leftFeet;
    [SerializeField]
    private Transform rightFeet;
    
    protected Animator animator;
    
    private bool ikActive = true;
    private Vector3 lookPosition; 


    private IKHand rightHand;
    private IKHand leftHand;

    private IKLeg rightLeg;
    private IKLeg leftLeg;

    private float headDuration = .5f;

    void Awake () 
    {
        animator = GetComponent<Animator>();
        rightHand = gameObject.AddComponent<IKHand>();
        leftHand = gameObject.AddComponent<IKHand>();
        rightLeg = gameObject.AddComponent<IKLeg>(); 
        leftLeg = gameObject.AddComponent<IKLeg>();
    }

    private void Start() {
        rightHand.Setup(transform,animator, movement.RB, true);
        leftHand.Setup(transform,animator, movement.RB, false);
        rightLeg.Setup(transform,animator, movement.RB, rightFeet, true);
        leftLeg.Setup(transform,animator, movement.RB, leftFeet, false);
    }

    public bool IKActive {
        get { return ikActive;}
        set { 
            ikActive = value; 
            // leftHand.IsActive = value;
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


    //a callback for calculating IK
    void OnAnimatorIK()
    {
        // OnIKUpdate?.Invoke();
        if(animator) {
            //if the IK is active, set the position and rotation directly to the goal. 
            if(ikActive) {

                animator.SetLookAtWeight(1);
                animator.SetLookAtPosition(lookPosition);
                
                // Set the right hand target position and rotation, if one has been assigned
                rightHand.UpdateIK();
                leftHand.UpdateIK();

                //update the legs inverse kinematic
                rightLeg.UpdateIK();
                leftLeg.UpdateIK();
                
            }
        }
    }    

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(lookPosition, .1f);
        Gizmos.color = Color.yellow;
    }
}
