using System.Collections;
using System.Collections.Generic;
using Custom.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class Mirror: MonoBehaviour, IRoomObject
{
    [SerializeField]
    private MirrorCanvas mirrorCanvas;
    public MirrorCanvas MirrorCanvas {
        get { return mirrorCanvas;}
    }

    [SerializeField]
    private PlanarReflection planarReflection;
    [SerializeField]
    private bool hidden = false;

    [Header("Room settings")]
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
    private MeshRenderer indicatorMesh;
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
            if (isOn != value) {
                isOn = value; 
                if (value) {
                    ConfirmationSucceeded();
                } else {
                    ConfirmationFailed();
                }
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
    public bool InSpace { get => inSpace; set => inSpace = value; }

    ///<summary>
    /// Updates the light indicator on whether the mirror is on.
    ///</summary>
    public void UpdateIndicatorLight() {
        Color colour = isOn ? onColor : offColor;
        colour *= 3.0f;
        indicatorMesh.material.SetColor("_EmissionColor", colour);
    }
    protected void Awake() {
        // planarReflection.IsActive = false;
    }

    ///<summary>
    /// Sets up the canvas so it can be used to display all the letters
    ///</summary>
    public void SetupCanvas()
    {
        mirrorCanvas.InitializeLetters(huzzleWords, letters, preAnswer);
        mirrorCanvas.SetupText(changeType, roomIndexoffset);
    }


    ///<summary>
    /// fires when the player wants to apply the question or sentence
    /// Also called from the mirror button
    ///</summary>
    public void Confirm()
    {
        if (isQuestion) room.CheckMirrorQuestion(this);
        else {
            if (!isOn) room.AddMirrorChange(this); else {
                room.RemoveMirrorChange(this);
                room.AddMirrorChange(this);
            }
        }
    }

    public void ConfirmationSucceeded() {
        if (room.Animated)
            AudioHandler.Instance?.PlaySound(SFXFiles.mirror_true, .5f);

        foreach(Letter letter in mirrorCanvas.selectedLetterObjects) {
            letter.Color = new Color(.8f, 1f, .8f);
        }
    }

    public void ConfirmationFailed() {
        if (room.Animated)
            AudioHandler.Instance?.PlaySound(SFXFiles.mirror_false);

        // mirrorCanvas.DeselectLetters();
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
