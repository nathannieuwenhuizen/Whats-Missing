using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomTelevision : Television
{
    [Header("Room TV settings")]

    public ChangeType changeType = ChangeType.missing;
    public bool isQuestion = true;
    [SerializeField]
    private bool isOn = false;

    [SerializeField]
    private string preAnswer;

    [SerializeField]
    protected string[] letters;


    public bool IsOn {
        get { return isOn; }
        set { 
            isOn = value; 
            if (value) {
                ConfirmationSucceeded();
            } else {
                ConfirmationFailed();
            }
            //TODO: change ui image/sound etc...
        }
    }


    private Room room;
    public Room Room {
        get { return room; }
        set { 
            room = value; 
        }
    }
    
    protected override void Awake()
    {
        base.Awake();
        InitializeLetters();
    }

    void InitializeLetters()
    {
        for(int i = 0; i < letters.Length; i++) {
            InitializeLetter(letters[i], GetLetterPosBasedOnIndex(letterObjects.Count));
        }
        for(int i = 0; i < preAnswer.Length; i++) {
            Letter answerLetter = InitializeLetter(preAnswer[i].ToString(), GetLetterPosBasedOnIndex(letterObjects.Count));
            LetterClicked(answerLetter);
        }
        
    }

    Vector3 GetLetterPosBasedOnIndex(int index) {
        int x =  Mathf.RoundToInt((float)containerColloms / 2f);
        int delta = index % containerColloms;
        if (delta != 0) {
            if (delta % 2 == 0) {
                x += Mathf.CeilToInt((float)delta / 2f);
            } else {
                x -= Mathf.CeilToInt((float)delta / 2f);
            }
        }
        int y = Mathf.FloorToInt((float)index / (float)containerColloms);
        return GetLetterPosition(x,y);
    }

    //fires when the player wants to apply the question or sentence
    public void Confirm()
    {
        if (isOn) return;

        if (isQuestion) room.CheckQuestion(this);
        else room.AddTVChange(this);
    }

    public void ConfirmationFailed() {
        Reset();
    }

    protected override void LetterClicked(Letter letter)
    {
        letterObjects.Remove(letter);
        base.LetterClicked(letter);
    }


    //sets all the letters to their original place.
    public void Reset() {
        for(int i = 0; i < selectedLetterObjects.Count; i++) {
            selectedLetterObjects[i].transform.parent = letterContainer;
            selectedLetterObjects[i].Deselected();
            letterObjects.Add(selectedLetterObjects[i]);
        }
        selectedLetterObjects = new List<Letter>();
        room.RemoveTVChange(this);
    }
    public void ConfirmationSucceeded() {
    }
}
