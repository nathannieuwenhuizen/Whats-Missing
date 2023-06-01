using UnityEngine;

public class Floor : RoomObject
{
    [SerializeField]
    private GameObject invisibleFloorCollider;

    public delegate void OnFloorMissingEvent();
    public static OnFloorMissingEvent OnFloorMissing;

    private void Awake() {
        invisibleFloorCollider.SetActive(false);
    }

    public override void OnMissing()
    {
        base.OnMissing();
        OnFloorMissing?.Invoke();
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
