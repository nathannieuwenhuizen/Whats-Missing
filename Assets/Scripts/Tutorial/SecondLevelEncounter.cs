using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondLevelEncounter : MonoBehaviour
{
    [Header("Second level")]
    [SerializeField]
    private RoomTelevision tv;

    public void OnRoomEnter() {
        StartCoroutine(WaitingForEncounter());
    }
    private IEnumerator WaitingForEncounter() {
        yield return new WaitForSeconds(.1f);
        tv.LetterContainer.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        tv.Talk(IntroDialogue.secondRoom, tv.QuestionText, () => {
            tv.LetterContainer.gameObject.SetActive(true);
        });
    }
}
