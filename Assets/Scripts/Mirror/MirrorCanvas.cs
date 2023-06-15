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
    public Mirror Mirror {
        get { return mirror;}
    }

    [SerializeField]
    private CanvasGroup hintToggle;
    [SerializeField]
    private CanvasGroup hintToggle2;

    [SerializeField]
    private TMP_FontAsset sentenceFont;
    [SerializeField]
    private TMP_FontAsset questionFont;

    [Header("buttons")]
    [SerializeField]
    private Button comfirmationButton;
    [SerializeField]
    private Button resetButton;

    public delegate void MirrorcanvasEvent(string hintText, float duration);
    public static MirrorcanvasEvent OnShowHint;

    private float letterPadding = 5f;

    private Canvas canvas;
    public Canvas Canvas {
        get { 
            return canvas;
        }
    }

    private string hintText = "";

    public List<Letter> Letters = new List<Letter>();
    public List<Letter> SelectedLetters = new List<Letter>();

    [SerializeField]
    public RectTransform letterContainer;

    [SerializeField]
    private HeaderText headerText;
    public HeaderText HeaderText { get => headerText; }

    private Vector3 headerPos;

    [SerializeField]
    private RectTransform answerText;
    [SerializeField]
    private GameObject letterPrefab;

    [SerializeField]
    private bool isInteractable = true;
    public bool IsInteractable {
        get { return isInteractable; }
        set { 
            isInteractable = value; 
            UpdateCanvas();
        }
    }

    private bool isFocused = false;
    public bool IsFocused {
        get { return isFocused; }
        set { 
            isFocused = value; 
            UpdateCanvas();
        }
    }

    private void Awake() {
        canvas = GetComponent<Canvas>();
        IsFocused = false;
        headerPos = headerText.transform.localPosition;
    }


    private void UpdateCanvas() {
        bool val = IsInteractable && isFocused;
        GetComponent<GraphicRaycaster>().enabled = true;// val;
        GetComponent<CanvasGroup>().alpha = val ? 1 : .8f;
        foreach(Letter letter in Letters) {
            letter.Interactable = val;
        }
        foreach(Letter letter in SelectedLetters) {
            letter.Interactable = val;
        }

        comfirmationButton.gameObject.SetActive(IsInteractable);
        resetButton.gameObject.SetActive(IsInteractable);
    }

    public TMP_FontAsset Font {
        set { 
            headerText.Text.font = value;
            foreach(Letter letter in SelectedLetters) 
                letter.Text.font = value;
            foreach(Letter letter in Letters) 
                letter.Text.font = value;
        }
    }

    //ui elements
    private int containerColloms = 13;
    private int containerRows = 2;

    public string Word {
        get { 
            string temp = "";
            foreach(Letter letter in SelectedLetters) {
                temp += letter.LetterValue;
            }
            return temp; 
        } set {
            for (int i = 0; i < value.Length; i++)
            {
                Letter letter = Letters.Find(x => x.LetterValue[0] == value[i]);
                if (letter != null)
                    LetterClicked(letter);
                else {
                    Debug.LogError("MirrorCanvas doesn't contain the letter: " + value[i] + " for the word: " + value);
                }
            }
        }
    }

    ///<summary>
    /// Sets up the type of text in the header and sets the font type depending if the mirror is a question mirror or not.
    ///</summary>
    public void SetupText(ChangeType changeType) {
        string header = Change.GetChangeTypeText(changeType);
        if (mirror.isQuestion) {
            HeaderText.Text.text = "What's <b>" + header + "</b>" + "?";
            Font = questionFont;
        } else {
            Font = sentenceFont;
            
            HeaderText.Text.text = "What SHOULD be <b>" + header + "</b>" + "?";

            // HeaderText.Text.text = "<b> is " + header +  "!</b>";
            // Vector3 answerPos = answerText.transform.parent.position;
            // answerPos.y = Mathf.Lerp(answerPos.y, headerText.transform.position.y, .5f);
            // answerText.transform.parent.position = answerPos;
        }

    }


    private void Update() {
        if (isInteractable && isFocused)
        {
            CheckKeyboardInput();
            HighLightClosestLetter();
        }
    }

    ///<summary>
    /// Checks and updates all the keyboard input the mirror canvas needs.
    ///</summary>
    private void CheckKeyboardInput() {
        if (InputManager.KEYBOARD_ENABLED_MIRROR){
            foreach( Letter letter in Letters) {
                if (Input.GetKeyDown(GetKeyCode(letter.LetterValue[0]))) {
                    if (letter.Interactable) {
                        letter.pressedTime = 0;
                        LetterClicked(letter);
                    }
                    return;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            AudioHandler.Instance?.PlaySound(SFXFiles.letter_click);
            RemoveSelectedLetter(SelectedLetters.Count - 1);
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
            mirror.Confirm();
        }
    }

    ///<summary>
    /// Fix for overlapping padding issue on the letters
    ///</summary>
    private void HighLightClosestLetter() {
        HighLightClosestLetterFromList(SelectedLetters);
        HighLightClosestLetterFromList(Letters);
    }
    ///<summary>
    /// Fix for overlapping padding issue on the letters
    ///</summary>
    private void HighLightClosestLetterFromList(List<Letter> list) {
        float length = Mathf.Infinity;
        Letter focusedLetter = null;
        foreach(Letter letter in list) {
            float delta= Vector3.Distance(letter.transform.position,
             canvas.MouseToWorldPosition());
             if (delta < length) {
                length = delta;
                focusedLetter = letter;
             }
        }
        if (focusedLetter != null) {
            focusedLetter.transform.SetSiblingIndex(list.Count);
        }
    }

    public void DeselectFrontLetter() {
        AudioHandler.Instance?.PlaySound(SFXFiles.letter_click, .2f, .6f);
        RemoveSelectedLetter(SelectedLetters.Count - 1);
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

    ///<summary>
    /// Calculates in which row and collomn indax the letter should be spawned in
    ///</summary>
    public Vector3 GetLetterSpawnPosBasedOnIndex(int index, int length) {
        //last row
        int y = Mathf.FloorToInt((float)index / (float)containerColloms);

        int x;
        if ((length - (y * containerColloms)) < containerColloms) {
            x = ((containerColloms - (length % containerColloms))/ 2) + (index % containerColloms);
        } else {
            //toprows without the need to center
            x = Mathf.FloorToInt((float)index % (float)containerColloms);
        }
        return GetLetterSpawnPosition(x,y);
    }

    ///<summary>
    /// Gets the position of the indexes.
    ///</summary>
    private Vector3 GetLetterSpawnPosition(int xIndex, int yIndex) {

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
            InitializeLetter(letters[i].ToString(), GetLetterSpawnPosBasedOnIndex(i, letters.Length));
        }
        for(int i = 0; i < preAnswer.Length; i++) {
            Letter answerLetter = InitializeLetter(preAnswer[i].ToString(), GetLetterSpawnPosBasedOnIndex(i, preAnswer.Length));
        }
        Word = preAnswer;
    }

    ///<summary>
    /// Initializes a single letter.
    ///</summary>
    private Letter InitializeLetter(string val, Vector3 pos) {
        Letter newLetter = GameObject.Instantiate(letterPrefab).GetComponent<Letter>();
            newLetter.gameObject.name ="letter: " + val;
            if (IsInteractable) newLetter.onLetterClick += LetterClicked;
            newLetter.MirrorCanvas = this;
            newLetter.GetComponent<RectTransform>().SetParent(letterContainer);
            newLetter.GetComponent<RectTransform>().localPosition = pos;
            newLetter.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
            newLetter.GetComponent<RectTransform>().localScale = Vector3.one;
            newLetter.LetterValue = val;
            Letters.Add(newLetter);
            return newLetter;
    }

    ///<summary>
    /// Removes a selected letter back to its spawn position.
    ///</summary>
    public void RemoveSelectedLetter(int index)
    {
        if (index < 0) return;
        SelectedLetters[index].transform.SetParent(letterContainer);
        SelectedLetters[index].Deselect();
        Letters.Add(SelectedLetters[index]);
        SelectedLetters.Remove(SelectedLetters[index]);
    }

    ///<summary>
    ///  Fires when a letter has been clicked. (so the cursor has been lifted.)
    ///</summary>
    public virtual void LetterClicked(Letter letter, bool canBeDragged = false)
    {
        if (letter.Dragged() && canBeDragged) {
            if (passedLetterIndex == -1) {
                if (mirror.canPlayAudio) AudioHandler.Instance?.PlaySound(SFXFiles.letter_click, .2f, .6f);

                letter.Deselect(true);
                letter.PreClickSelected = false;
            } else {
                Debug.Log("bug happening?");
                AddLetterToAnswer(letter, passedLetterIndex);
            }

        } else {
            if (!letter.PreClickSelected) {
                AddLetterToAnswer(letter);
            }
            else {
                letter.Deselect();
                letter.PreClickSelected = false;
            }
        }
        
        UpdateAnswerTextPosition(null);
    }


    ///<summary>
    /// sets all the letters to their original place.
    ///</summary>
    public void DeselectLetters() {
        for(int i = SelectedLetters.Count - 1; i >= 0; i--) {
            RemoveSelectedLetter(i);
        }
    }
    private int passedLetterIndex = -1;
    public void UpdateLetterDrag(Letter draggedLetter) {

        passedLetterIndex = -1;
        if (draggedLetter.Position.y > 80f) {
            passedLetterIndex = 0;
        }

        bool passed = UpdateAnswerTextPosition(draggedLetter);

        if (passed == false && passedLetterIndex == 0) {
            passedLetterIndex = SelectedLetters.Count;
        }
    }

    ///<summary>
    /// Updates the letters of the answer position to make it more centered.
    ///</summary>
    public bool UpdateAnswerTextPosition(Letter draggedLetter) {
        bool passed = false;
        if (SelectedLetters.Count == 0) return passed;

        float totalWidth = -letterPadding;
        foreach(Letter letter in SelectedLetters) {
            totalWidth += letter.size.width + letterPadding;
        }
        if (draggedLetter) {
            totalWidth += draggedLetter.size.width + letterPadding;
        }
        // if (!mirror.isQuestion) {
        //     headerText.Text.ForceMeshUpdate();
        //     totalWidth += headerText.Text.GetRenderedValues(true).x;
        // }
        float cPos = -(totalWidth + letterPadding + SelectedLetters[0].size.width) * .5f;
        for(int i = 0; i < SelectedLetters.Count; i++) {
            if (i != 0) {
                cPos += SelectedLetters[i].size.width *.5f + letterPadding;
            }
            if (draggedLetter != null) {
                if (passed == false && passedLetterIndex == 0 && SelectedLetters[i].Position.x > draggedLetter.Position.x) {
                    passed = true;
                    passedLetterIndex = i;
                    cPos += draggedLetter.size.width *.5f + letterPadding;
                }
            }
            SelectedLetters[i].MoveTo(answerText.localPosition + new Vector3(cPos, 0, 0));
            cPos += SelectedLetters[i].size.width *.5f + letterPadding;
        }
        // if (!mirror.isQuestion){
        //     headerText.Text.rectTransform.SetParent(answerText.transform);
        //     headerText.MoveTo(new Vector3(cPos + headerText.Text.GetRenderedValues(true).x * .5f + 20f, 0, 0));
        //     // headerText.Text.rectTransform.localPosition = new Vector3(cPos + headerText.Text.GetRenderedValues(true).x * .5f + 20f, 0, 0);
        // }

        return passed;
    }

    private float durationBeforeSecondHint = 0;
    public void ShowHintButton(string _hintText, float _durationBeforeSecondHint) {
        durationBeforeSecondHint = _durationBeforeSecondHint;
        hintText = _hintText;
        hintToggle.interactable = true;
        hintToggle.blocksRaycasts = true;
        StartCoroutine(hintToggle.FadeCanvasGroup(1f, 1f, 0f));
    }

    private string secondHintAnswer = "";
    public void ShowSecondHintButton(string _answer) {
        secondHintAnswer = _answer;
        hintToggle2.interactable = true;
        hintToggle2.blocksRaycasts = true;
        hintToggle2.GetComponent<Button>().onClick.AddListener(HighlightAnswer);
        StartCoroutine(hintToggle2.FadeCanvasGroup(1f, 1f, 0f));
    }
    public void HighlightAnswer() {
        DeselectLetters();
        List<Letter> answerLetters = new List<Letter>();
        List<Letter> letterobjectsTemp = Letters;
        for(int i = 0; i < secondHintAnswer.Length; i++) {
            Letter foundLetter = letterobjectsTemp.Find(l => l.LetterValue == (secondHintAnswer[i] + "") );
            letterobjectsTemp.Remove(foundLetter);
            answerLetters.Add(foundLetter);
        }
        foreach(Letter letter in Letters) {
            letter.DefaultColor = new Color(1,1,1,1f);
        }

        foreach(Letter letter in letterobjectsTemp) {
                letter.DefaultColor = new Color(1,1,1,.2f);
        }
    }

    public void HintToggleClick() {
        OnShowHint?.Invoke(hintText, durationBeforeSecondHint);
    }

    public void AddLetterToAnswer(Letter letter, int index = -1) {
        if (mirror.Room != null && mirror.Room.Animated && isInteractable) {
            AudioHandler.Instance?.PlaySound(SFXFiles.letter_click, .5f, 
            .8f + (.4f * ((float)SelectedLetters.Count / (float)(Letters.Count + SelectedLetters.Count)))
            );
        }

        Letters.Remove(letter);
        if (index == -1) SelectedLetters.Add(letter); 
        else SelectedLetters.Insert(index, letter);
        letter.Select();
        letter.transform.SetParent(answerText.transform);
    }


    public void AnimateCorrectLetters() {
        for (int i = 0 ; i < SelectedLetters.Count; i++ ) {
            SelectedLetters[i].AnimateGlowColor(.5f, Letter.CorrectColor);
            StartCoroutine(SelectedLetters[i].ZDistanceBounce(.5f, -10, (((float)i / (float)SelectedLetters.Count)) * .5f));
        }
    }
    public void AnimateInCorrectLetters() {
        for (int i = 0 ; i < SelectedLetters.Count; i++ ) {
            SelectedLetters[i].GlowColor = Letter.IncorrectColor;
            SelectedLetters[i].AnimateGlowColor(1f, SelectedLetters[i].DefaultGlowColor);
        }
    }
}
