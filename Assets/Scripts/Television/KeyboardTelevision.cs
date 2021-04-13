using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardTelevision : Television
{

    private string[] upRowLetters = new string[] {"q", "w" , "e", "r", "t", "y", "u", "i", "o", "p"};
    private string[] middleRowLetters = new string[] {"a", "s" , "d", "f", "g", "h", "j", "k", "l"};
    private string[] bottomRowLetters = new string[] {"z", "x" , "c", "v", "b", "n", "m"};
    protected override void Awake()
    {
        base.Awake();
        containerColloms = 10;
        InitializeLetters();
    }

    void InitializeLetters()
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
        Letter newLetter = InitializeLetter(letter.LetterValue, letter.transform.localPosition);
        // newLetter.transform.localPosition = letter.transform.localPosition;
        base.LetterClicked(newLetter);
    }
}
