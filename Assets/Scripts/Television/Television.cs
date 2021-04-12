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
    private int containerColloms = 9;
    private int containerRows = 3;

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
            newLetter.onLetterClick += LetterClicked;
            newLetter.GetComponent<RectTransform>().SetParent(letterContainer);
            newLetter.GetComponent<RectTransform>().localPosition = GetLetterPosBasedOnIndex(letterObjects.Count);
            newLetter.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
            newLetter.GetComponent<RectTransform>().localScale = Vector3.one;
            newLetter.LetterValue = val;
            letterObjects.Add(newLetter);
            return newLetter;
    }
    Vector3 LetterPos(int xIndex, int yIndex) {

        Vector3 result = new Vector3(0,0,0);

        float width = letterContainer.rect.width;
        float height = letterContainer.rect.height;
        float cellSize = (height / (float)containerRows);
        result.x = -width /2f + ((float)xIndex  / (float)containerColloms) * width + (width / (float)containerColloms) * .5f;
        result.y = height /2f - ((float)yIndex  / (float)containerRows) * height  -  cellSize * .5f;
        result.y += -Mathf.Sin(xIndex / (float)containerColloms * Mathf.PI) * cellSize * .3f;
        return result;
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
        // Debug.Log(x + " |  " + y);
        return LetterPos(x,y);
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
            selectedLetterObjects[i].transform.parent = letterContainer;
            selectedLetterObjects[i].Deselected();
        }
        selectedLetterObjects = new List<Letter>();
        room.RemoveTVChange(this);
    }
    public void ConfirmationSucceeded() {
    }
}
