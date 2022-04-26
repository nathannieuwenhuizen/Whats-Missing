using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SoxAtkJiggleBone))]
public class BossWiggleBone : MonoBehaviour
{
    [SerializeField]
    private bool childHaveSameValuesAsMainBone = true;

    private SoxAtkJiggleBone mainBone;
    [SerializeField]
    private SoxAtkJiggleBone[] bones;
    private SoxAtkJiggleBone[] Bones {
        get {
            if (bones == null) bones = GetBones();
            return bones;
        }
    }
    private SoxAtkJiggleBone[] GetBones() {
        mainBone = GetComponent<SoxAtkJiggleBone>();
        mainBone.enabled = false;
        List<SoxAtkJiggleBone> result = new List<SoxAtkJiggleBone>(GetComponentsInChildren<SoxAtkJiggleBone>());
        result.Remove(mainBone);
        return result.ToArray();
    }

    private void CreateSubBones() {
        Transform t = transform;
        int i = 0;
        while (t.childCount > 0 && i < 10) {
            if (t.GetComponent<SoxAtkJiggleBone>() == null){
                t.gameObject.AddComponent(typeof(SoxAtkJiggleBone));
                i++;
            } 
            t = t.GetChild(0);
        }
        if (t.GetComponent<SoxAtkJiggleBone>() == null)
            t.gameObject.AddComponent(typeof(SoxAtkJiggleBone));
    }

    private void Reset() {
        CreateSubBones();
        UpdateBoneValues();
    }

    public void UpdateBoneValues() {
        CreateSubBones();
        bones = GetBones();
        mainBone = GetComponent<SoxAtkJiggleBone>();
        if (childHaveSameValuesAsMainBone) {

            for (int i = 0; i < Bones.Length; i++) {
                Bones[i].m_animated = mainBone.m_animated;
                Bones[i].m_simType = mainBone.m_simType;
                Bones[i].m_targetDistance = mainBone.m_targetDistance;
                Bones[i].m_tension = mainBone.m_tension;
                Bones[i].m_inercia = mainBone.m_inercia;
                Bones[i].m_lookAxis = mainBone.m_lookAxis;
                Bones[i].m_lookAxisFlip = mainBone.m_lookAxisFlip;
                Bones[i].m_sourceUpAxis = mainBone.m_sourceUpAxis;
                Bones[i].m_sourceUpAxisFlip = mainBone.m_sourceUpAxisFlip;
                Bones[i].m_upWorld = mainBone.m_upWorld;
                Bones[i].m_upNodeAxis = mainBone.m_upNodeAxis;
                Bones[i].m_upnodeControl = mainBone.m_upnodeControl;
                Bones[i].m_gravity = mainBone.m_gravity;
                Bones[i].m_colliders = mainBone.m_colliders;
                Bones[i].m_optShowGizmosAtPlaying = mainBone.m_optShowGizmosAtPlaying;
                Bones[i].m_optShowGizmosAtEditor = mainBone.m_optShowGizmosAtEditor;
                Bones[i].m_optGizmoSize = mainBone.m_optGizmoSize;
                Bones[i].m_optShowHiddenNodes = mainBone.m_optShowHiddenNodes;
                Bones[i].m_simulationSpeed = mainBone.m_simulationSpeed;

                Bones[i].m_upNode = Bones[i].transform.parent;
            }
        }
    }
    private void Update() {
#if UNITY_EDITOR
#endif
        UpdateBoneValues();
    }

    private void OnDrawGizmosSelected() {
        UpdateBoneValues();
    }


}
