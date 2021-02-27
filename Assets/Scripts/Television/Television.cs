using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Television : MonoBehaviour
{
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

    private List<Letter> letterObjects = new List<Letter>();
    private List<Letter> selectedLetterObjects = new List<Letter>();
    
    // Start is called before the first frame update
    void Start()
    {
        answerText.text = "";
        InitializeLetters();
    }

    void InitializeLetters()
    {
        for(int i = 0; i < letters.Length; i++)
        {
            Letter newLetter = GameObject.Instantiate(letterPrefab).GetComponent<Letter>();
            letterObjects.Add(newLetter);
            newLetter.onLetterClick += LetterClicked;
            newLetter.GetComponent<RectTransform>().SetParent(letterContainer);
            newLetter.GetComponent<RectTransform>().localPosition = new Vector3(i * 50, 0, 0);
            newLetter.GetComponent<RectTransform>().localScale = new Vector3(.5f,.5f,.5f);
            newLetter.LetterValue = letters[i];
        }
    }

    void LetterClicked(Letter letter)
    {
        letter.Hide();
        answerText.text += letter.LetterValue;
        selectedLetterObjects.Add(letter);
    }

    public void CheckAnswer()
    {
        AnswerIsFalse();
    }

    void AnswerIsCorrect() {

    }
    void AnswerIsFalse() {
        answerText.text = "";
        foreach(Letter letter in selectedLetterObjects) {
            letter.Show();
        }
        selectedLetterObjects = new List<Letter>();
    }
}
