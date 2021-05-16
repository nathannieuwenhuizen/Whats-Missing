using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardTelevision : Television
{

    [SerializeField]
    private Text centerText;

    [SerializeField]
    private AudioClip beepSound;

    [SerializeField]
    private TelevisionButton yesButton;
    [SerializeField]
    private TelevisionButton noButton;

    [SerializeField]
    private TelevisionButton comfirmButton;
    [SerializeField]
    private TelevisionButton resetButton;

    
    public Text Centertext { get => centerText; }

    private string[] upRowLetters = new string[] {"q", "w" , "e", "r", "t", "y", "u", "i", "o", "p"};
    private string[] middleRowLetters = new string[] {"a", "s" , "d", "f", "g", "h", "j", "k", "l"};
    private string[] bottomRowLetters = new string[] {"z", "x" , "c", "v", "b", "n", "m"};
    protected override void Awake()
    {
        base.Awake();
        containerColloms = 10;
    }

    public void InitializeKeyboard()
    {
        for(int i = 0; i < upRowLetters.Length; i++) {
            InitializeLetter(upRowLetters[i], GetLetterPosition(i, 0));
        }
        for(int i = 0; i < middleRowLetters.Length; i++) {
            InitializeLetter(middleRowLetters[i], GetLetterPosition(i, 1));
        }
        for(int i = 0; i < bottomRowLetters.Length; i++) {
            InitializeLetter(bottomRowLetters[i], GetLetterPosition(1 + i, 2));
        }
    }

    public void ToggleYesNoButtons(bool visible) {
        yesButton.gameObject.SetActive(visible);
        noButton.gameObject.SetActive(visible);
    }
    public void ToggleKeyboardButtons(bool visible) {
        comfirmButton.gameObject.SetActive(visible);
        resetButton.gameObject.SetActive(visible);
    }

    public void DestroyKeyboard() {
        Reset();
        foreach(Letter letter in letterObjects) {
            Destroy(letter.gameObject);
        }
        letterObjects = new List<Letter>();
    }

    //deletes all the letters selected
    public void Reset() {
        for(int i = 0; i < selectedLetterObjects.Count; i++) {
            Destroy(selectedLetterObjects[i].gameObject);
            letterObjects.Remove(selectedLetterObjects[i]);
        }
        selectedLetterObjects = new List<Letter>();
    }

    protected override void LetterClicked(Letter letter)
    {
        sfx.Play(letterClickSound);
        Letter newLetter = InitializeLetter(letter.LetterValue, letter.transform.localPosition);
        base.LetterClicked(newLetter);
    }

    public void Talk(string[] lines, Text speakText, Action callback) {
        if (speakText == null) speakText = questionText;
        StartCoroutine(Talking(lines, speakText, callback));
    }

    public IEnumerator Talking(string[] lines, Text speakText, Action callback) {
        yield return null;
        int lineIndex = 0;
        while (lineIndex < lines.Length) {
            questionText.text = "";
            centerText.text = "";
            int letterIndex = 0;
            lines[lineIndex] = lines[lineIndex].Replace("[NAME]", PlayerData.PLAYER_NAME);
            Debug.Log(lines[lineIndex]);
            while(letterIndex < lines[lineIndex].Length) {
                speakText.text += lines[lineIndex][letterIndex];
                letterIndex++;
                sfx.Play(beepSound, .01f, false);
                yield return new WaitForSeconds(.04f);
            }
            lineIndex++;
            yield return null;
            yield return new WaitForSeconds(2f);
        }
        callback?.Invoke();
    }
}
