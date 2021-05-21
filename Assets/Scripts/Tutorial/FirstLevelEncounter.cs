using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstLevelEncounter : MonoBehaviour
{
    [Header("First level")]
    [SerializeField]
    private RoomTelevision firstLevelTelevision;
    [SerializeField]
    private Door firstLevelDoor;


    public void OnRoomEnter() {
        StartCoroutine(WaitingForEncounter());
    }
    private IEnumerator WaitingForEncounter() {
        firstLevelTelevision.QuestionText.text = "";
        yield return new WaitForSeconds(2f);
        firstLevelTelevision.Talk(IntroDialogue.firstRoom, firstLevelTelevision.Centertext, () => {
            firstLevelDoor.Locked = false;
        });
    }

}
