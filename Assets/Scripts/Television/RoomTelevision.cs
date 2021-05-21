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

    [Header("color indicator")]
    [SerializeField]
    private MeshRenderer mr;
    [SerializeField]
    private Color offColor = Color.red;
    [SerializeField]
    private Color onColor = Color.green;
    

    [Header("Sounds")]
    [SerializeField]
    private AudioClip succesSound;
    [SerializeField]
    private AudioClip failSound;

    public bool IsOn {
        get { return isOn; }
        set { 
            isOn = value; 
            if (value) {
                ConfirmationSucceeded();
            } else {
                ConfirmationFailed();
            }
            updateIndicatorLight();
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
        updateIndicatorLight();
    }

    private void updateIndicatorLight() {
        Color colour = isOn ? onColor : offColor;
        colour *= 3.0f;
        mr.material.SetColor("_EmissionColor", colour);
    }

    private void InitializeLetters()
    {
        for(int i = 0; i < letters.Length; i++) {
            InitializeLetter(letters[i], GetLetterPosBasedOnIndex(letterObjects.Count));
        }
        for(int i = 0; i < preAnswer.Length; i++) {
            Letter answerLetter = InitializeLetter(preAnswer[i].ToString(), GetLetterPosBasedOnIndex(letterObjects.Count));
            LetterClicked(answerLetter);
        }
    }

    private Vector3 GetLetterPosBasedOnIndex(int index) {
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
        if (room.Animated) sfx.Play(failSound);
        Reset();
    }

    protected override void LetterClicked(Letter letter)
    {
        if ( room != null && room.Animated) sfx.Play(letterClickSound);
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
        if (room.Animated) sfx.Play(succesSound);
    }
}
