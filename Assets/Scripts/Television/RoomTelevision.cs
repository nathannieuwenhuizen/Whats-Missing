using System.Collections;
using System.Collections.Generic;
using Custom.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class RoomTelevision : Television, IRoomObject
{

    [SerializeField]
    private PlanarReflection planarReflection;
    [SerializeField]
    private bool hidden = false;

    [Header("Room TV settings")]
    public ChangeType changeType = ChangeType.missing;
    public bool isQuestion = true;
    [SerializeField]
    private bool isOn = false;

    [Range(-1,1)]
    public int roomIndexoffset = 0;

    [SerializeField]
    private string preAnswer;
    public string PreAnswer {
        get { return preAnswer;}
        set { preAnswer = value; }
    }

    [SerializeField]
    protected string letters;
    public string Letters {
        get { return letters; }
        set { letters = value; }
    }

    [SerializeField]
    private bool huzzleWords = true;

    [Header("color indicator")]
    [SerializeField]
    private MeshRenderer mr;
    [SerializeField]
    private Color offColor = Color.red;
    [SerializeField]
    private Color onColor = Color.green;

    public int id {get; set; }

    private string previousWord = "";
    public string PreviousWord {
        get { return previousWord;}
        set { previousWord = value; }
    }

    public bool IsOn {
        get { return isOn; }
        set { 
            isOn = value; 
            if (value) {
                ConfirmationSucceeded();
            } else {
                ConfirmationFailed();
            }
            UpdateIndicatorLight();
        }
    }


    private Room room;
    public Room Room {
        get { return room; }
        set { 
            room = value; 
        }
    }

    private bool inSpace = true;
    public bool InSpace { get => inSpace; }

    ///<summary>
    /// Updates the light indicator on whether the tv is on.
    ///</summary>
    public void UpdateIndicatorLight() {
        Color colour = isOn ? onColor : offColor;
        colour *= 3.0f;
        mr.material.SetColor("_EmissionColor", colour);
    }

    ///<summary>
    /// Creates all the letters and sets the word to pre answer
    ///</summary>
    public void InitializeLetters()
    {
        if (huzzleWords) {
            letters = Extensions.Shuffle(letters);
        }
        for(int i = 0; i < letters.Length; i++) {
            InitializeLetter(letters[i].ToString(), GetLetterPosBasedOnIndex(i));
        }
        for(int i = 0; i < preAnswer.Length; i++) {
            Letter answerLetter = InitializeLetter(preAnswer[i].ToString(), GetLetterPosBasedOnIndex(i));
        }
        Word = preAnswer;
        UpdateHeaderText();
    }
    private void UpdateHeaderText() {
        string header = "missing";
        string roomText = "";
        if (roomIndexoffset == -1) {
            roomText = " in the <b>PREVIOUS</b> room";
        }
        switch (changeType) {
            case ChangeType.missing:
                header = "missing";
                break;
            case ChangeType.reverse:
                header = "flipped";
                break;
            case ChangeType.tooBig:
                header = "too big";
                break;
            case ChangeType.tooSmall:
                header = "too small";
                break;
        }
        headerText.text = "What's <b>" + header + "<b>" + roomText + "?";
    }

    private Vector3 GetLetterPosBasedOnIndex(int index) {
        //last row
        int y = Mathf.FloorToInt((float)index / (float)containerColloms);

        int x;
        if ((letters.Length - (y * containerColloms)) < containerColloms) {
            x = ((containerColloms - (letters.Length % containerColloms))/ 2) + (index % containerColloms);
        } else {
            //toprows without the need to center
            x = Mathf.FloorToInt((float)index % (float)containerColloms);
        }
        return GetLetterPosition(x,y);
    }

    ///<summary>
    /// fires when the player wants to apply the question or sentence
    ///</summary>
    public override void Confirm()
    {
        base.Confirm();

        if (isQuestion) room.CheckTVQuestion(this);
        else if (isOn == false) room.AddTVChange(this);
    }

    public void ConfirmationFailed() {
        if (room.Animated)         
            AudioHandler.Instance?.PlaySound(SFXFiles.mirror_false);

        DeselectLetters();
    }

    protected override void LetterClicked(Letter letter)
    {
        if ( room != null && room.Animated) {
            AudioHandler.Instance?.PlaySound(SFXFiles.letter_click, .5f, 
            .8f + (.4f * ((float)selectedLetterObjects.Count / (float)(letterObjects.Count + selectedLetterObjects.Count)))
            );
        }
        base.LetterClicked(letter);
    }

    protected override void AddLetterToAnswer(Letter letter) {
        letterObjects.Remove(letter);
        base.AddLetterToAnswer(letter);
    }
    protected override void RemoveLetterFromAnswer(Letter letter)
    {
        RemoveSelectedLetter(selectedLetterObjects.IndexOf(letter));
        // base.RemoveLetterFromAnswer(letter);
    }

    protected override void RemoveSelectedLetter(int index)
    {
        base.RemoveSelectedLetter(index);
        if (index < 0) return;
        selectedLetterObjects[index].Color = Color.white;
        selectedLetterObjects[index].transform.parent = letterContainer;
        selectedLetterObjects[index].Deselect();
        letterObjects.Add(selectedLetterObjects[index]);
        selectedLetterObjects.Remove(selectedLetterObjects[index]);
    }

    ///<summary>
    /// sets all the letters to their original place.
    ///</summary>
    public void DeselectLetters() {

        for(int i = selectedLetterObjects.Count - 1; i >= 0; i--) {
            RemoveSelectedLetter(i);
        }
        // Debug.Log("after deselection: " + selectedLetterObjects.Count);         
    }
    ///<summary>
    /// Resets the letters and removes the change or question check.
    ///</summary>
    public void ResetTV() {
        DeselectLetters();
        if (!isQuestion) room.RemoveTVChange(this, true);
        else room.CheckTVQuestion(this);
    }
    public void ConfirmationSucceeded() {
        if (room.Animated)
            AudioHandler.Instance?.PlaySound(SFXFiles.mirror_true);

        foreach(Letter letter in selectedLetterObjects) {
            letter.Color = new Color(.8f, 1f, .8f);
        }
    }

    public void OnRoomEnter()
    {
        inSpace = true;
        if (!hidden) {
            planarReflection.IsActive = true;
        } else {
            planarReflection.IsActive = false;
        }
    }

    public void OnRoomLeave()
    {
        inSpace = false;
        planarReflection.IsActive = false;
    }
}
