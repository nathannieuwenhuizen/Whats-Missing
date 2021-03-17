using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Television : MonoBehaviour
{

    public ChangeType changeType = ChangeType.missing;
    public bool isQuestion = true;
    [SerializeField]
    private bool isOn = false;



    [SerializeField]
    private string answer;

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

    private bool isInteractable = false;
    public bool IsInteractable {
        get { return isInteractable; }
        set { 
            isInteractable = value; 
            GetComponentInChildren<GraphicRaycaster>().enabled = value;
        }
    }
    private Room room;
    public Room Room {
        get { return room; }
        set { 
            room = value; 
        }
    }
    
    //ui elements
    [SerializeField]
    private Text questionText;
    [SerializeField]
    private Text answerText;
    [SerializeField]
    private RectTransform letterContainer;

    [SerializeField]
    private GameObject letterPrefab;

    [SerializeField]
    private string[] letters;

    public string Word {
        get { return answerText.text; }
    }
    private List<Letter> letterObjects = new List<Letter>();
    private List<Letter> selectedLetterObjects = new List<Letter>();
    
    void Awake()
    {
        answerText.text = "";
        IsInteractable = false;
        InitializeLetters();
    }

    void InitializeLetters()
    {
        for(int i = 0; i < letters.Length; i++) InitializeLetter(letters[i]);
        for(int i = 0; i < answer.Length; i++) {
            Letter answerLetter = InitializeLetter(answer[i].ToString());
            LetterClicked(answerLetter);
        }
        
    }
    Letter InitializeLetter(string val) {
        Letter newLetter = GameObject.Instantiate(letterPrefab).GetComponent<Letter>();
            letterObjects.Add(newLetter);
            newLetter.onLetterClick += LetterClicked;
            newLetter.GetComponent<RectTransform>().SetParent(letterContainer);
            newLetter.GetComponent<RectTransform>().localPosition = new Vector3(letterObjects.Count * 50, 0, 0);
            newLetter.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
            newLetter.GetComponent<RectTransform>().localScale = new Vector3(.5f,.5f,.5f);
            newLetter.LetterValue = val;
            return newLetter;
    }

    void LetterClicked(Letter letter)
    {
        letter.Hide();
        answerText.text += letter.LetterValue;
        selectedLetterObjects.Add(letter);
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
    public void Reset() {
        answerText.text = "";
        foreach(Letter letter in selectedLetterObjects) {
            letter.Show();
        }
        selectedLetterObjects = new List<Letter>();
        room.RemoveTVChange(this);
    }
    public void ConfirmationSucceeded() {
    }
}
