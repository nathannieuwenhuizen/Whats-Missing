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
    protected string letters;

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
            InitializeLetter(letters[i].ToString(), GetLetterPosBasedOnIndex(letterObjects.Count));
        }
        for(int i = 0; i < preAnswer.Length; i++) {
            Letter answerLetter = InitializeLetter(preAnswer[i].ToString(), GetLetterPosBasedOnIndex(letterObjects.Count));
            //LetterClicked(answerLetter);
        }
        Word = preAnswer;
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
        if (!huzzleWords) {
            x = index;
        }
        int y = Mathf.FloorToInt((float)index / (float)containerColloms);
        return GetLetterPosition(x,y);
    }

    //fires when the player wants to apply the question or sentence
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
        
        letterObjects.Remove(letter);
        base.LetterClicked(letter);
    }

    protected override void RemoveSelectedLetter(int index)
    {
        base.RemoveSelectedLetter(index);
        if (index < 0) return;
        selectedLetterObjects[index].Color = Color.white;
        selectedLetterObjects[index].transform.parent = letterContainer;
        selectedLetterObjects[index].Deselected();
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
        selectedLetterObjects = new List<Letter>();
         
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
}
