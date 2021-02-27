using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    
    private Button button;
    private Text text;
    private bool canBeClicked = true;

    private string letterValue;

    // sets the value of the letter
    public string LetterValue {
        get {return letterValue;}
        set {
            letterValue = value;
            text.text = value;
        }
    }

    public delegate void LetterClickedEvent(Letter letter);
    public event LetterClickedEvent onLetterClick;

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => LetterIsClicked());
        text = GetComponent<Text>();
    }
    public void Hide()
    {
        canBeClicked = false;
        transform.localScale = new Vector3(0, 0, 0);
    }

    public void Show()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        canBeClicked = true;
    }

    void LetterIsClicked()
    {
        if (!canBeClicked) return;
        onLetterClick?.Invoke(this);
    }

}
