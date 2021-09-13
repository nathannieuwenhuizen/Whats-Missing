using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardTelevision : Television
{

    [SerializeField]
    private TelevisionButton yesButton;
    [SerializeField]
    private TelevisionButton noButton;

    [SerializeField]
    private TelevisionButton comfirmButton;
    [SerializeField]
    private TelevisionButton resetButton;

    
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
    protected override void RemoveSelectedLetter(int index)
    {
        base.RemoveSelectedLetter(index);
        if (index < 0) return;
        Destroy(selectedLetterObjects[index].gameObject);
        letterObjects.Remove(selectedLetterObjects[index]);
        selectedLetterObjects.Remove(selectedLetterObjects[index]);

    }

    //deletes all the letters selected
    public void Reset() {
        for(int i = selectedLetterObjects.Count - 1; i >= 0; i--) {
            RemoveSelectedLetter(i);
        }
        selectedLetterObjects = new List<Letter>();
    }

    protected override void LetterClicked(Letter letter)
    {
        AudioHandler.Instance?.PlaySound(SFXFiles.letter_click);

        Letter newLetter = InitializeLetter(letter.LetterValue, letter.transform.localPosition);
        base.LetterClicked(newLetter);
    }
}
