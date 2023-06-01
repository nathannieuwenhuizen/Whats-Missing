using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {

    public interface IIKBossBone {
        public bool IsActive { get; set;}
        public float Weight {get; set; }

        public float WeightAnimationDuration { get; set;}
        
        public Vector3 IKPosition {get; set; }
        public Vector3 IKLookDirection {get; set; }

        public void UpdateIK(Animator _animator);
        public void UpdatePositionIK(Animator _animator);
        public void UpdateRotationIK(Animator _animator);

        public AvatarIKGoal IKGoal {get;}
        public void OnDrawGizmos();
    }


    ///<summary>
    /// A basic boss bone
    ///</summary>
    [System.Serializable]
    public abstract class IKBossBone : IIKBossBone
    {
        private bool isActive = false;
        private Coroutine weightCoroutine;
        private IKBossPass ikPass;

        public bool IsActive { 
            get { return isActive; } 
            set {
                isActive = value;
                if (weightCoroutine != null) 
                    ikPass.StopCoroutine(weightCoroutine);
                ikPass.StartCoroutine(AnimateWeight(value ? 1 : 0));
            }
        }

        [Range(0,1)]
        [SerializeField]
        private float weight = 0f;
        public float Weight { get => weight; set => weight = value; }
        public float WeightAnimationDuration { get; set; } = .5f;
        public Vector3 IKPosition { get; set; } = Vector3.zero;
        public Vector3 IKLookDirection { get; set; } = Vector3.zero;

        [SerializeField]
        protected AvatarIKGoal ikGoal;
        public AvatarIKGoal IKGoal => ikGoal;

        protected Transform transform;
        protected Transform boneTransform;

        ///<summary>
        /// Simply animates the weight values
        ///</summary>
        private IEnumerator AnimateWeight(float _end)
        {
            yield return Extensions.AnimateCallBack(Weight, _end, AnimationCurve.EaseInOut(0,0,1,1), (float val) => {
                Weight = val;
            }, 1f);
        }

        public IKBossBone(Transform _transform, IKBossPass _ikPass) {
            transform = _transform;
            ikPass = _ikPass;
            boneTransform = SetBoneTransform(_ikPass.Animator); 

        }

        ///<summary>
        /// Updates the whole IK on the bone. Will be fried from the animator
        ///</summary>
        public virtual void UpdateIK(Animator _animator)
        {
            if (_animator == null) return;
            if (Weight == 0) return;

            UpdatePositionIK(_animator);
            UpdateRotationIK(_animator);
        }

        public virtual void OnEnable() {
        }

        public virtual void OnDisable() {
        }

        ///<summary>
        /// Gets the transform of the bone this class is asigned to
        ///</summary>
        protected Transform SetBoneTransform(Animator _animator) {
            Transform result = default(Transform);
            switch (ikGoal)
            {
                case AvatarIKGoal.LeftFoot:
                result = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                break;
                case AvatarIKGoal.RightFoot:
                result = _animator.GetBoneTransform(HumanBodyBones.RightFoot);
                break;
                case AvatarIKGoal.LeftHand:
                result = _animator.GetBoneTransform(HumanBodyBones.LeftHand);
                break;
                case AvatarIKGoal.RightHand:
                result = _animator.GetBoneTransform(HumanBodyBones.RightHand);
                break;
                default:
                result = _animator.GetBoneTransform(HumanBodyBones.Head);
                break;
            }
            return result;
        }



        ///<summary>
        /// Updates the position of the bone.
        ///</summary>
        public virtual void UpdatePositionIK(Animator _animator)
        {
            if (IKPosition == Vector3.zero) return;

            _animator.SetIKPositionWeight(IKGoal, Weight);
            _animator.SetIKPosition(IKGoal, IKPosition);
        }

        ///<summary>
        /// Updates the rotation of the bone
        ///</summary>
        public virtual void UpdateRotationIK(Animator _animator)
        {
            if (IKLookDirection == Vector3.zero) return;
            _animator.SetIKRotationWeight(IKGoal, Weight);

            Vector3 slopeCorrected = -Vector3.Cross(IKLookDirection, transform.right);
            Quaternion rotation = Quaternion.LookRotation(slopeCorrected, IKLookDirection);
            _animator.SetIKRotation(IKGoal, rotation);
        }


        public virtual void OnDrawGizmos()
        {

        }
    }
}