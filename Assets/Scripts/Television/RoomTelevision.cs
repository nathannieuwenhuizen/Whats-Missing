using System.Collections;
using System.Collections.Generic;
using Custom.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class RoomTelevision: MonoBehaviour, IRoomObject
{
    [SerializeField]
    private MirrorCanvas mirrorCanvas;
    public MirrorCanvas MirrorCnvas {
        get { return mirrorCanvas;}
    }

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
    public string Word {
        get => mirrorCanvas.Word;
        set => mirrorCanvas.Word = value;
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
            mirrorCanvas.InitializeLetter(letters[i].ToString(), mirrorCanvas.GetLetterPosBasedOnIndex(i, letters.Length));
        }
        for(int i = 0; i < preAnswer.Length; i++) {
            Letter answerLetter = mirrorCanvas.InitializeLetter(preAnswer[i].ToString(), mirrorCanvas.GetLetterPosBasedOnIndex(i, letters.Length));
        }
        mirrorCanvas.Word = preAnswer;
        mirrorCanvas.UpdateHeaderText(changeType, roomIndexoffset);
    }

    ///<summary>
    /// fires when the player wants to apply the question or sentence
    ///</summary>
    public void Confirm()
    {
        if (isQuestion) room.CheckTVQuestion(this);
        else if (isOn == false) room.AddTVChange(this);
    }

    public void ConfirmationFailed() {
        if (room.Animated)         
            AudioHandler.Instance?.PlaySound(SFXFiles.mirror_false);

        DeselectLetters();
    }

    protected void LetterClicked(Letter letter)
    {
        if ( room != null && room.Animated) {
            AudioHandler.Instance?.PlaySound(SFXFiles.letter_click, .5f, 
            .8f + (.4f * ((float)mirrorCanvas.selectedLetterObjects.Count / (float)(mirrorCanvas.letterObjects.Count + mirrorCanvas.selectedLetterObjects.Count)))
            );
        }
        mirrorCanvas.LetterClicked(letter);
    }

    private void RemoveLetterFromAnswer(Letter letter)
    {
        mirrorCanvas.RemoveSelectedLetter(mirrorCanvas.selectedLetterObjects.IndexOf(letter));
        // base.RemoveLetterFromAnswer(letter);
    }


    ///<summary>
    /// sets all the letters to their original place.
    ///</summary>
    public void DeselectLetters() {
        for(int i = mirrorCanvas.selectedLetterObjects.Count - 1; i >= 0; i--) {
            mirrorCanvas.RemoveSelectedLetter(i);
        }
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

        foreach(Letter letter in mirrorCanvas.selectedLetterObjects) {
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
