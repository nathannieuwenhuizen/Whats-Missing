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
    private string preAnswer;

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
    private RectTransform answerText;
    [SerializeField]
    private RectTransform letterContainer;

    [SerializeField]
    private GameObject letterPrefab;

    [SerializeField]
    private string[] letters;

    public string Word {
        get { 
            string temp = "";
            foreach(Letter letter in selectedLetterObjects) {
                temp += letter.LetterValue;
            }
            return temp; 
        }
    }
    private List<Letter> letterObjects = new List<Letter>();
    private List<Letter> selectedLetterObjects = new List<Letter>();
    
    void Awake()
    {
        IsInteractable = false;
        InitializeLetters();
    }

    void InitializeLetters()
    {
        for(int i = 0; i < letters.Length; i++) InitializeLetter(letters[i]);
        for(int i = 0; i < preAnswer.Length; i++) {
            Letter answerLetter = InitializeLetter(preAnswer[i].ToString());
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
            newLetter.GetComponent<RectTransform>().localScale = new Vector3(1.5f,1.5f,1.5f);
            newLetter.LetterValue = val;
            return newLetter;
    }

    void LetterClicked(Letter letter)
    {
        letter.Selected();
        selectedLetterObjects.Add(letter);
        letter.transform.parent = answerText.transform;
        UpdateAnswerTextPosition();
    }

    private void UpdateAnswerTextPosition() {
        float totalWidth = 0;
        foreach(Letter letter in selectedLetterObjects) {
            totalWidth += letter.size.width;
        }
        float cPos = -(totalWidth - selectedLetterObjects[0].size.width) / 2;
        for(int i = 0; i < selectedLetterObjects.Count; i++) {
            if (i != 0) {
                cPos += selectedLetterObjects[i].size.width / 2;
            }
            selectedLetterObjects[i].MoveTo(answerText.localPosition + new Vector3(cPos, 0, 0));
            cPos += selectedLetterObjects[i].size.width / 2;
        }
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

    //sets all the letters to their original place.
    public void Reset() {
        for(int i = 0; i < selectedLetterObjects.Count; i++) {
            selectedLetterObjects[i].transform.parent = letterContainer.parent;
            selectedLetterObjects[i].Deselected();
        }
        selectedLetterObjects = new List<Letter>();
        room.RemoveTVChange(this);
    }
    public void ConfirmationSucceeded() {
    }
}
