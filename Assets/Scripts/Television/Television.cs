using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
     /// Television class that holds the letter container and can be filled in
/// </summary>
public class Television : MonoBehaviour
{
    [Header("Base TV settings")]
    [SerializeField]
    protected RectTransform letterContainer;

    public RectTransform LetterContainer {
        get => letterContainer;
    }

    [SerializeField]
    protected Text questionText;
    public Text QuestionText { get => questionText; }

    [SerializeField]
    protected RectTransform answerText;

    [SerializeField]
    private AudioClip beepSound;

    [SerializeField]
    private Text centerText;
    public Text Centertext { get => centerText; }


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


    [SerializeField]
    protected GameObject letterPrefab;

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
    protected List<Letter> letterObjects = new List<Letter>();
    protected List<Letter> selectedLetterObjects = new List<Letter>();




    protected virtual void Awake() {
        IsInteractable = false;
    }

    public void UpdateAnswerTextPosition() {
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


    private void Update() {
        if (isInteractable)
        {
            CheckKeyboardInput();
        }
    }
    private void CheckKeyboardInput() {
        foreach( Letter letter in letterObjects) {
            if (Input.GetKeyDown(GetKeyCode(letter.LetterValue[0]))) {
                LetterClicked(letter);
                return;
            }
        }
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

        selectedLetterObjects.Add(letter);
        letter.Selected();
        letter.transform.SetParent(answerText.transform);
        UpdateAnswerTextPosition();
    }

    public virtual void Confirm() {

    }

    public void Talk(string[] lines, Text speakText, Action callback) {
        if (speakText == null) speakText = questionText;
        StartCoroutine(Talking(lines, speakText, callback));
    }

    public IEnumerator Talking(string[] lines, Text speakText, Action callback) {
        yield return null;
        int lineIndex = 0;
        while (lineIndex < lines.Length) {
            questionText.text = "";
            centerText.text = "";
            int letterIndex = 0;
            lines[lineIndex] = lines[lineIndex].Replace("[NAME]", PlayerData.PLAYER_NAME);
            Debug.Log(lines[lineIndex]);
            while(letterIndex < lines[lineIndex].Length) {
                speakText.text += lines[lineIndex][letterIndex];
                letterIndex++;
                // AudioHandler.Instance?.PlaySound(SFXFiles.tv_beep);
                yield return new WaitForSeconds(.04f);
            }
            lineIndex++;
            yield return null;
            yield return new WaitForSeconds(lineIndex == lines.Length ? 0f : 2f);
        }
        callback?.Invoke();
    }


}
