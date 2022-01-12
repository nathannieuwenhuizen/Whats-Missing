using UnityEngine;

public class Floor : RoomObject
{
    [SerializeField]
    private GameObject invisibleFloorCollider;

    private void Awake() {
        invisibleFloorCollider.SetActive(false);
    }

    public override void OnMissing()
    {
        base.OnMissing();
        invisibleFloorCollider.SetActive(true);
    }

    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
        invisibleFloorCollider.SetActive(false);
    }
    private void Reset() {
        Word = "floor";
        AlternativeWords = new string[] { "ground" };
    }

    
}
