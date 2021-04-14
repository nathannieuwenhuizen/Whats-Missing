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

    [SerializeField]
    protected Text questionText;
    [SerializeField]
    protected RectTransform answerText;

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
    protected int containerRows = 3;


    [SerializeField]
    protected GameObject letterPrefab;

    public string Word {
        get { 
            string temp = "";
            foreach(Letter letter in selectedLetterObjects) {
                temp += letter.LetterValue;
            }
            return temp; 
        }
    }
    protected List<Letter> letterObjects = new List<Letter>();
    protected List<Letter> selectedLetterObjects = new List<Letter>();




    protected virtual void Awake() {
        IsInteractable = false;
    }

    protected void UpdateAnswerTextPosition() {
        float totalWidth = 0;
        foreach(Letter letter in selectedLetterObjects) {
            Debug.Log("width: " + letter.size.width);
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
        result.y += -Mathf.Sin(xIndex / (float)containerColloms * Mathf.PI) * cellSize * .3f;
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
    protected virtual void LetterClicked(Letter letter)
    {
        selectedLetterObjects.Add(letter);
        letter.Selected();
        letter.transform.parent = answerText.transform;
        UpdateAnswerTextPosition();
        Debug.Log(selectedLetterObjects.Count);
    }



}
