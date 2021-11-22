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
    [SerializeField]
    private Mirror mirror;

    [SerializeField]
    private CanvasGroup hintToggle;

    public delegate void MirrorcanvasEvent(string hintText);
    public static MirrorcanvasEvent OnShowHint;


    private string hintText = "";

    public List<Letter> letterObjects = new List<Letter>();
    public List<Letter> selectedLetterObjects = new List<Letter>();

    [SerializeField]
    public RectTransform letterContainer;

    [SerializeField]
    private TMP_Text headerText;
    public TMP_Text HeaderText { get => headerText; }

    [SerializeField]
    private RectTransform answerText;
    [SerializeField]
    private GameObject letterPrefab;

    [SerializeField]
    private bool isInteractable = false;
    public bool IsInteractable {
        get { return isInteractable; }
        set { 
            isInteractable = value; 
            GetComponent<GraphicRaycaster>().enabled = value;
            GetComponent<CanvasGroup>().alpha = value ? 1 : .8f;
        }
    }

    //ui elements
    private int containerColloms = 9;
    private int containerRows = 2;

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
                    Debug.LogError("MirrorCanvas doesn't contain the letter: " + value[i] + " for the word: " + value);
                }
            }
        }
    }
    public void UpdateHeaderText(ChangeType changeType, int roomIndexOffset = 0) {
        string header = "missing";
        string roomText = "";
        if (roomIndexOffset == -1) {
            roomText = " in the <b>PREVIOUS</b> room";
        }
        switch (changeType) {
            case ChangeType.missing:
                header = "missing";
                break;
            case ChangeType.flipped:
                header = "flipped";
                break;
            case ChangeType.tooBig:
                header = "too big";
                break;
            case ChangeType.tooSmall:
                header = "too small";
                break;
        }
        HeaderText.text = "What's <b>" + header + "<b>" + roomText + "?";
    }

    private void Awake() {
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
            mirror.Confirm();
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

    public Vector3 GetLetterPosBasedOnIndex(int index, int length) {
        //last row
        int y = Mathf.FloorToInt((float)index / (float)containerColloms);

        int x;
        if ((length - (y * containerColloms)) < containerColloms) {
            x = ((containerColloms - (length % containerColloms))/ 2) + (index % containerColloms);
        } else {
            //toprows without the need to center
            x = Mathf.FloorToInt((float)index % (float)containerColloms);
        }
        return GetLetterPosition(x,y);
    }

    private Vector3 GetLetterPosition(int xIndex, int yIndex) {

        Vector3 result = new Vector3(0,0,0);

        float width = letterContainer.rect.width;
        float height = letterContainer.rect.height;
        float cellSize = (height / (float)containerRows);
        result.x = -width /2f + ((float)xIndex  / (float)containerColloms) * width + (width / (float)containerColloms) * .5f;
        result.y = height /2f - ((float)yIndex  / (float)containerRows) * height  -  cellSize * .5f;
        result.y += -Mathf.Sin(xIndex / (float)containerColloms * Mathf.PI) * cellSize * .1f;
        return result;
    }
    ///<summary>
    /// Creates all the letters and sets the word to pre answer
    ///</summary>
    public void InitializeLetters(bool huzzleWords, string letters, string preAnswer)
    {
        letters = letters.Replace(" ", "");
        preAnswer = preAnswer.Replace(" ", "");
        if (huzzleWords) {
            letters = Extensions.Shuffle(letters);
        }
        for(int i = 0; i < letters.Length; i++) {
            InitializeLetter(letters[i].ToString(), GetLetterPosBasedOnIndex(i, letters.Length));
        }
        for(int i = 0; i < preAnswer.Length; i++) {
            Letter answerLetter = InitializeLetter(preAnswer[i].ToString(), GetLetterPosBasedOnIndex(i, preAnswer.Length));
        }
        Word = preAnswer;
    }

    public Letter InitializeLetter(string val, Vector3 pos) {
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

    public void RemoveSelectedLetter(int index)
    {
        if (index < 0) return;
        selectedLetterObjects[index].Color = Color.white;
        selectedLetterObjects[index].transform.parent = letterContainer;
        selectedLetterObjects[index].Deselect();
        letterObjects.Add(selectedLetterObjects[index]);
        selectedLetterObjects.Remove(selectedLetterObjects[index]);
    }
    public virtual void LetterClicked(Letter letter)
    {
        if (!letter.Selected) {
            if (mirror.Room != null && mirror.Room.Animated) {
                AudioHandler.Instance?.PlaySound(SFXFiles.letter_click, .5f, 
                .8f + (.4f * ((float)selectedLetterObjects.Count / (float)(letterObjects.Count + selectedLetterObjects.Count)))
                );
            }
            AddLetterToAnswer(letter);
        }
        else RemoveSelectedLetter(selectedLetterObjects.IndexOf(letter));
        
        UpdateAnswerTextPosition();
    }
    ///<summary>
    /// sets all the letters to their original place.
    ///</summary>
    public void DeselectLetters() {
        for(int i = selectedLetterObjects.Count - 1; i >= 0; i--) {
            RemoveSelectedLetter(i);
        }
    }

    public void ShowHintButton(string _hintText) {
        if (_hintText == "") return;

        hintText = _hintText;
        hintToggle.interactable = true;
        hintToggle.blocksRaycasts = true;
        AudioHandler.Instance.Player3DSound(SFXFiles.hintbutton_show, transform);
        StartCoroutine(hintToggle.FadeCanvasGroup(1f, 1f, 0f));
    }

    public void HintToggleClick() {
        OnShowHint?.Invoke(hintText);
    }

    public void AddLetterToAnswer(Letter letter) {
        letterObjects.Remove(letter);
        selectedLetterObjects.Add(letter);
        letter.Select();
        letter.transform.SetParent(answerText.transform);
    }
}
