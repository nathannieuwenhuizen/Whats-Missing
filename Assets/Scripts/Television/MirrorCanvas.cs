using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

///<summary>
/// Handles all the events that happens inside the canvas element of the mirror. 
/// Such as holding the letters and firing canvas events.
///</summary>
public class MirrorCanvas : MonoBehaviour
{
    public List<Letter> letterObjects = new List<Letter>();
    public List<Letter> selectedLetterObjects = new List<Letter>();

    [SerializeField]
    protected RectTransform letterContainer;

    [SerializeField]
    protected TMP_Text headerText;
    public TMP_Text HeaderText { get => headerText; }

    [SerializeField]
    private RectTransform answerText;
    [SerializeField]
    protected GameObject letterPrefab;

    [SerializeField]
    private bool isInteractable = false;
    public bool IsInteractable {
        get { return isInteractable; }
        set { 
            isInteractable = value; 
            GetComponentInChildren<GraphicRaycaster>().enabled = value;
            GetComponentInChildren<CanvasGroup>().alpha = value ? 1 : .8f;
        }
    }
    
    //ui elements
    protected int containerColloms = 9;
    protected int containerRows = 2;



    public string Word {
        get { 
            string temp = "";
            foreach(Letter letter in selectedLetterObjects) {
                temp += letter.LetterValue;
            }
            return temp; 
        } set {
            for (int i = 0; i < value.Length; i++)
            {
                Letter letter = letterObjects.Find(x => x.LetterValue[0] == value[i]);
                if (letter != null)
                    LetterClicked(letter);
                else {
                    Debug.LogError("TV doesn't contain the letter: " + value[i] + " for the word: " + value);
                }
            }
        }
    }


    protected virtual void Awake() {
        IsInteractable = false;
    }

    public void UpdateAnswerTextPosition() {
        float padding = 15f;
        if (selectedLetterObjects.Count == 0) return;
        float totalWidth = -padding;
        foreach(Letter letter in selectedLetterObjects) {
            totalWidth += letter.size.width + padding;
        }
        float cPos = -(totalWidth + padding + selectedLetterObjects[0].size.width) * .5f;
        for(int i = 0; i < selectedLetterObjects.Count; i++) {
            if (i != 0) {
                cPos += selectedLetterObjects[i].size.width *.5f + padding;
            }
            selectedLetterObjects[i].MoveTo(answerText.localPosition + new Vector3(cPos, 0, 0));
            cPos += selectedLetterObjects[i].size.width *.5f + padding;
        }
    }


    private void Update() {
        if (isInteractable)
        {
            CheckKeyboardInput();
        }
    }

    private void CheckKeyboardInput() {
#if UNITY_EDITOR
        foreach( Letter letter in letterObjects) {
            if (Input.GetKeyDown(GetKeyCode(letter.LetterValue[0]))) {
                LetterClicked(letter);
                return;
            }
        }
#endif
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            AudioHandler.Instance?.PlaySound(SFXFiles.letter_click);
            RemoveSelectedLetter(selectedLetterObjects.Count - 1);
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
            Confirm();
        }
    }
    
    private readonly Dictionary<char, KeyCode> _keycodeCache = new Dictionary<char, KeyCode>();
    private KeyCode GetKeyCode(char character)
    {
        // Get from cache if it was taken before to prevent unnecessary enum parse
        KeyCode code;
        if (_keycodeCache.TryGetValue(character, out code)) return code;
        // Cast to it's integer value
        int alphaValue = character;
        code = (KeyCode)Enum.Parse(typeof(KeyCode), alphaValue.ToString());
        _keycodeCache.Add(character, code);
        return code;
    }

    protected Vector3 GetLetterPosition(int xIndex, int yIndex) {

        Vector3 result = new Vector3(0,0,0);

        float width = letterContainer.rect.width;
        float height = letterContainer.rect.height;
        float cellSize = (height / (float)containerRows);
        result.x = -width /2f + ((float)xIndex  / (float)containerColloms) * width + (width / (float)containerColloms) * .5f;
        result.y = height /2f - ((float)yIndex  / (float)containerRows) * height  -  cellSize * .5f;
        result.y += -Mathf.Sin(xIndex / (float)containerColloms * Mathf.PI) * cellSize * .1f;
        return result;
    }
    protected Letter InitializeLetter(string val, Vector3 pos) {
        Letter newLetter = GameObject.Instantiate(letterPrefab).GetComponent<Letter>();
            
            newLetter.onLetterClick += LetterClicked;
            newLetter.GetComponent<RectTransform>().SetParent(letterContainer);
            newLetter.GetComponent<RectTransform>().localPosition = pos;
            newLetter.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
            newLetter.GetComponent<RectTransform>().localScale = Vector3.one;
            newLetter.LetterValue = val;
            letterObjects.Add(newLetter);
            return newLetter;
    }
    protected virtual void RemoveSelectedLetter(int index) {

    }
    protected virtual void LetterClicked(Letter letter)
    {
        if (!letter.Selected) {
            AddLetterToAnswer(letter);
        }
        else {
            RemoveLetterFromAnswer(letter);

        }
        UpdateAnswerTextPosition();
    }

    protected virtual void AddLetterToAnswer(Letter letter) {
        selectedLetterObjects.Add(letter);
        letter.Select();
        letter.transform.SetParent(answerText.transform);
    }
    protected virtual void  RemoveLetterFromAnswer(Letter letter) {
        letter.Deselect();
    }

    public virtual void Confirm() {

    }
}
