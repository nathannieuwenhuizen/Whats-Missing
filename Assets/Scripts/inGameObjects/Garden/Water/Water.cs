using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Water : RoomObject
{

    public delegate void WaterEvent();
    public static WaterEvent OnWaterBig;
    public static WaterEvent OnWaterBigEnd;
    public static WaterEvent OnWaterNormal;

    private Material watermaterial;
    [SerializeField]
    private GameObject mainLand_water;
    private Vector3 mainLand_water_large_altitude;
    private Vector3 mainLand_water_shrink_altitude;
    private MeshRenderer meshRenderer;

    private void Awake() {
        base.Awake();
        meshRenderer = GetComponent<MeshRenderer>();
        watermaterial = meshRenderer.material;
    }

    private void OnEnable() {
        TimeProperty.onTimeMissing += UpdateTimeScale;
        TimeProperty.onTimeAppearing += UpdateTimeScale;
    }

    private void OnDisable() {
        TimeProperty.onTimeMissing -= UpdateTimeScale;
        TimeProperty.onTimeAppearing -= UpdateTimeScale;
    }


    public void UpdateTimeScale() {
        if (InSpace == false) return;
        watermaterial.SetFloat("_RoomTime", Room.TimeScale);
    }

    public override void OnEnlarge()
    {
        if (mainLand_water_large_altitude.y == 0) {
            mainLand_water_large_altitude = mainLand_water.transform.localPosition;
            mainLand_water_shrink_altitude = mainLand_water.transform.localPosition;
            mainLand_water_shrink_altitude.y = transform.localPosition.y;
        }
        mainLand_water.transform.localPosition = mainLand_water_shrink_altitude;
        base.OnEnlarge();
        OnWaterBig?.Invoke();
        ToggleMainLandWater(true);
}

    public override IEnumerator AnimateEnlarging()
    {
        AudioHandler.Instance?.PlaySound(SFXFiles.water_rise);
        StartCoroutine(mainLand_water.transform.AnimatingLocalPos(mainLand_water_large_altitude, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
        return base.AnimateEnlarging();
    }

    public override void OnEnlargingFinish()
    {
        base.OnEnlargingFinish();
        OnWaterBigEnd?.Invoke();
        mainLand_water.transform.localPosition = mainLand_water_large_altitude;

    }

    public override void OnEnlargeRevert()
    {
        base.OnEnlargeRevert();
        OnWaterNormal?.Invoke();

    }

    public override IEnumerator AnimateEnlargeRevert()
    {
        AudioHandler.Instance?.PlaySound(SFXFiles.water_lower);
        StartCoroutine(mainLand_water.transform.AnimatingLocalPos(mainLand_water_shrink_altitude, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
        return base.AnimateEnlargeRevert();
    }

    public override void OnEnlargeRevertFinish()
    {
        base.OnEnlargeRevertFinish();
        ToggleMainLandWater(false);
        mainLand_water.transform.localPosition = mainLand_water_shrink_altitude;

    }

    private void ToggleMainLandWater(bool val) {
        meshRenderer.enabled = !val;
        mainLand_water.SetActive(val);
    }

    private void Reset() {
        Word = "water";
        AlternativeWords = new string[] { "waters", "lake", "pond", "pool", "liquid" };
    }

}
