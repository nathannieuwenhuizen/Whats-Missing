using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneHandeler : MonoBehaviour
{

    [SerializeField]
    private FPMovement movement;
    [SerializeField]
    private KeyboardTelevision introTelevision;
    [SerializeField]
    private Door door;

    [SerializeField]
    private TutorialIndicator tutorialIndicator;


    [SerializeField]
    private SceneLoader sceneLoader;

    void Start()
    {
        movement.EnableWalk = false;
        door.Locked = true;
        introTelevision.Centertext.text = "";
        introTelevision.ToggleKeyboardButtons(false);
        introTelevision.ToggleYesNoButtons(false);
        StartCoroutine(tutorialIndicator.WaitForMouseClick());
    }

    private void OnEnable() {
        Door.OnPassingThrough += GoToNextScene;
    }

    private void OnDisable() {
        Door.OnPassingThrough -= GoToNextScene;
    }

    private void GoToNextScene(Door door) {
        sceneLoader.LoadNextScene();
    }

    public void PickUpRemote() {
        StartCoroutine(PickingUpRemote());
    }
    public IEnumerator PickingUpRemote() {
        yield return StartCoroutine(introTelevision.Talking(IntroDialogue.firstLines, introTelevision.Centertext, null));
        introTelevision.ToggleYesNoButtons(true);
    }
    public void ResponseToPlayGame(bool answer) {
        StartCoroutine(AfterPlayingGame(answer));
    }
    public IEnumerator AfterPlayingGame(bool answer) {
        introTelevision.ToggleYesNoButtons(false);
        yield return StartCoroutine(introTelevision.Talking(answer ? IntroDialogue.responseToGame : IntroDialogue.responseToNoGame, introTelevision.Centertext, null));
        yield return new WaitForSeconds(2f);
        introTelevision.Centertext.text = "";
        yield return StartCoroutine(introTelevision.Talking(IntroDialogue.askForName, introTelevision.QuestionText, null));
        introTelevision.ToggleKeyboardButtons(true);
        introTelevision.InitializeKeyboard();
    }
    public void ResponseToName() {
        StartCoroutine(ReplyingToName());
    }
    public IEnumerator ReplyingToName() {
        string answer = introTelevision.Word;
        PlayerData.PLAYER_NAME = answer;
        introTelevision.QuestionText.text = "";
        introTelevision.ToggleKeyboardButtons(false);
        introTelevision.DestroyKeyboard();
        if (answer.Length < 3) {
            yield return StartCoroutine(introTelevision.Talking( IntroDialogue.responceofSmallName, introTelevision.Centertext, null));
        } else if  ( answer.Length > 10) {
            yield return StartCoroutine(introTelevision.Talking( IntroDialogue.responceofTooLongName, introTelevision.Centertext, null));
        } else {
            yield return StartCoroutine(introTelevision.Talking( IntroDialogue.responceofNormalName, introTelevision.Centertext, null));
        }
        yield return new WaitForSeconds(2f);
        door.Locked = false;
        yield return StartCoroutine(introTelevision.Talking( IntroDialogue.goToNextRoom, introTelevision.Centertext, null));
        StartCoroutine(tutorialIndicator.WaitForMove());
        movement.EnableWalk = true;

    }

}
