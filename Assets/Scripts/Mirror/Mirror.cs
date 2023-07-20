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
    public PlanarReflection PlanarReflection {
        get { return planarReflection;}
    }

    [SerializeField]
    private bool hidden = false;

    [Header("Room settings")]
    public bool isQuestion = true;

    [SerializeField]
    private MirrorData mirrorData;
    public MirrorData MirrorData {
        get { return mirrorData;}
        set { mirrorData = value; }
    }

    [SerializeField]
    private string preAnswer;
    public string PreAnswer {
        get { return preAnswer;}
        set { preAnswer = value; }
    }

    public string Letters {
        get { return mirrorData.letters; }
        set { mirrorData.letters = value; }
    }

    [Header("color indicator")]
    [SerializeField]
    private MeshRenderer indicatorMesh;
    [SerializeField]
    private Color offColor = Color.red;
    [SerializeField]
    private Color onColor = Color.green;

    public int id {get; set; }

    public ChangeType ChangeType {
        get { return mirrorData.changeType;}
        set { mirrorData.changeType = value; }
    }

    private string previousWord = "";
    public string PreviousWord {
        get { return previousWord;}
        set { previousWord = value; }
    }

    [HideInInspector]
    public bool canPlayAudio = false;
    

    public bool IsOn {
        get { return mirrorData.isOn; }
        set { 
            if (mirrorData.isOn != value) {
                if (value) {
                    canPlayAudio = true;
                    ConfirmationSucceeded();
                } else {
                    if (isQuestion)
                        ConfirmationFailed();
                }
            }

            mirrorData.isOn = value; 
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
        get => mirrorCanvas.Word.ToLower();
        set => mirrorCanvas.Word = value;
    }


    private bool inSpace = false;
    public bool InSpace { get => inSpace; set => inSpace = value; }

    ///<summary>
    /// Updates the light indicator on whether the mirror is on.
    ///</summary>
    public void UpdateIndicatorLight() {
        Color color = IsOn ? onColor : offColor;
        color *= 3.0f;
        indicatorMesh.material.SetColor("_EmissionColor", color);
    }
    protected virtual void Awake() {
        // planarReflection.IsActive = false;
    }

    ///<summary>
    /// Sets up the canvas so it can be used to display all the letters
    ///</summary>
    public void SetupCanvas()
    {
        mirrorCanvas.IsInteractable = mirrorData.isInteractable;
        mirrorCanvas.InitializeLetters(mirrorData.huzzleWords, Letters, preAnswer);
        mirrorCanvas.SetupText( ChangeType);
    }


    ///<summary>
    /// fires when the player wants to apply the question or sentence
    /// Also called from the mirror button
    ///</summary>
    public void Confirm()
    {
        canPlayAudio = true;
        if (isQuestion) room.CheckMirrorQuestion(this);
        else {
            if (!IsOn) room.AddMirrorChange(this); 
            else {
                room.RemoveMirrorChange(this);
                room.AddMirrorChange(this);
            }
        }
    }

    public void ConfirmationSucceeded() {
        if (room.Animated && room.ChangeLineAnimated && canPlayAudio){
            AudioHandler.Instance?.PlaySound(SFXFiles.mirror_true, .5f);
            canPlayAudio = false;
        }
        mirrorCanvas.AnimateCorrectLetters();
    }

    public void ConfirmationFailed() {
        if (room.Animated && room.ChangeLineAnimated && canPlayAudio)
        {
            AudioHandler.Instance?.PlaySound(SFXFiles.mirror_false);
            canPlayAudio = false;
        }
        mirrorCanvas.AnimateInCorrectLetters();

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
