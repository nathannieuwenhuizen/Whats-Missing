using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using AmazingAssets.CurvedWorld;
public class World : Property
{

    private float rotationDuration = 2f;
    private AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);

    [SerializeField]
    private Room room;
    [SerializeField]
    private Transform rotationPoint;
    // [SerializeField]
    // private CurvedWorldController curvedWorldController;
    [SerializeField]
    private Material curveMaterial;
    [SerializeField]
    private Material curveTransparentMaterial;
    [SerializeField]
    private Material curveParticleMaterial;
    [SerializeField]
    private Material defaultParticleMaterial;

    private Transform oldParentRotatePoint;
    private Transform oldParentRoom;

    private bool flipped = false;

    private void SetupRotation() {
        oldParentRoom = room.transform.parent;
        oldParentRotatePoint = rotationPoint.parent;

        rotationPoint.SetParent(oldParentRoom);
        room.transform.SetParent(rotationPoint);
    }

    private void ResetSetup() {
        rotationPoint.SetParent(oldParentRotatePoint);
        room.transform.SetParent(oldParentRoom);
    }

    public override void OnFlipped()
    {
        SetupRotation();
        base.OnFlipped();
    }
    public override IEnumerator AnimateFlipping()
    {
        // curvedWorldController.enabled = true;
        ApplyCurveMaterial();
        StartCoroutine(AnimateCurvature(-Mathf.Abs(rotationPoint.position.x - room.Player.transform.position.x)));
        yield return new WaitForSeconds(rotationDuration * .8f);
        StartCoroutine(AnimateCurvature(0));
        yield return StartCoroutine(AnimateRotation(180));
        // curvedWorldController.enabled = false;
        RemoveCurveMaterial();

        yield return base.AnimateFlipping();

    }
    public override void OnFlippingFinish()
    {
        XRotation = 180;
        ResetSetup();
        base.OnFlippingFinish();
    }

    public override void OnFlippingRevert()
    {
        // curvedWorldController.enabled = false;

        SetupRotation();
        base.OnFlippingRevert();
    }

    private List<MaterialHolders> materialHolders;
    private void ApplyCurveMaterial() {
        materialHolders = new List<MaterialHolders>();
        foreach(Renderer renderer in room.GetAllObjectsInRoom<Renderer>()) {
            if (renderer.GetComponent<ParticleSystem>()) continue;

            Texture tex = renderer.material.GetTexture("_MainTex");

            float surfaceType = renderer.material.GetFloat("_Surface");
            Material newMat;
            
            newMat = surfaceType == 0 ? curveMaterial : curveTransparentMaterial;
            if (surfaceType == 1) {
                Debug.Log("gameobject name: " + renderer.gameObject.name);
            }
            // if (renderer.material == defaultParticleMaterial) {
            //     newMat = curveParticleMaterial;
            // }

            newMat.SetTexture("_MainTex", tex);
            if (renderer.material.IsKeywordEnabled("_EMISSION")) {
                newMat.EnableKeyword("_EMISSION");
                newMat.SetColor("_EmissionColor", renderer.material.GetColor("_EmissionColor"));
                newMat.SetTexture("_EmissionMap", renderer.material.GetTexture("_EmissionMap"));
            } else {
                newMat.DisableKeyword("_EMISSION");
            }
            Debug.Log("surface: " + surfaceType);
            materialHolders.Add (new MaterialHolders() {renderer = renderer, materials = renderer.sharedMaterials});
            renderer.materials = new Material[] {newMat};
            renderer.material.mainTexture = tex;
            renderer.UpdateGIMaterials();
        }

    }
    private void RemoveCurveMaterial() {
        foreach(MaterialHolders holder in materialHolders) {
            holder.renderer.materials = holder.materials;
            holder.renderer.UpdateGIMaterials();
        }

    }

    public override IEnumerator AnimateFlippingRevert()
    {
        // curvedWorldController.enabled = true;
        ApplyCurveMaterial();
        StartCoroutine(AnimateCurvature(-Mathf.Abs(rotationPoint.position.x - room.Player.transform.position.x)));
        yield return new WaitForSeconds(rotationDuration * .8f);
        StartCoroutine(AnimateCurvature(0));
        yield return StartCoroutine(AnimateRotation(0));
        // curvedWorldController.enabled = false;
        RemoveCurveMaterial();

        yield return base.AnimateFlippingRevert();
    }
    public override void OnFlippingRevertFinish()
    {
        XRotation = 0;
        ResetSetup();
        base.OnFlippingRevertFinish();
    }

    private IEnumerator AnimateRotation(float endVal) {
        float index = 0;
        float beignVal = XRotation;
        while (index < rotationDuration) {
            XRotation = Mathf.LerpUnclamped(beignVal, endVal, curve.Evaluate(index / rotationDuration));
            yield return new WaitForFixedUpdate();
            index += Time.deltaTime;
        }
        XRotation = endVal;
    }
    private IEnumerator AnimateCurvature(float endVal) {
        float index = 0;
        // float beignVal = CurveOffset;
        while (index < rotationDuration) {
            // CurveOffset = Mathf.LerpUnclamped(beignVal, endVal, curve.Evaluate(index / rotationDuration));
            yield return new WaitForFixedUpdate();
            index += Time.deltaTime;
        }
        // CurveOffset = endVal;
    }

    private float xRotation = 0;
    private float XRotation {
        get => xRotation;
        set {
            xRotation = value;
            rotationPoint.rotation = Quaternion.Euler(value,0,0);
            // room.Player.transform.localRotation = Quaternion.Euler(
            //     -value, 
            //     room.Player.transform.localRotation.y,
            //     room.Player.transform.localRotation.z);
        }
    }

    // private float CurveOffset{
    //     get => curvedWorldController.bendCurvatureOffset;
    //     set {
    //         curvedWorldController.bendCurvatureOffset = value;
    //     }
    // }


    private void Reset() {
        Word = "world";
        AlternativeWords = new string[] {"room", "area", "enviroment"};
    }
}
