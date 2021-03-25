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
    private RectTransform rt;

    private Coroutine movingCoroutine;
    private float movingIndex;

    [SerializeField]
    private AnimationCurve slideAnimation;

    private Vector3 spawnPosition;

    private string letterValue;

    // sets the value of the letter
    public string LetterValue {
        get {return letterValue;}
        set {
            letterValue = value;
            text.text = value;
            //Debug.Log(size.width + " | " + size.height + " | rt width" + rt.rect.width);
        }
    }

    public delegate void LetterClickedEvent(Letter letter);
    public event LetterClickedEvent onLetterClick;

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        rt = GetComponent<RectTransform>();
        button.onClick.AddListener(() => LetterIsClicked());
        text = GetComponent<Text>();
    }
    public void Selected()
    {
        canBeClicked = false;
        spawnPosition = rt.localPosition;
    }

    public void Deselected()
    {
        canBeClicked = true;
        MoveTo(spawnPosition);
    }

    public Size size {
        get {
            TextGenerator textGen = new TextGenerator();
            TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size); 
            float width = textGen.GetPreferredWidth(letterValue, generationSettings);
            float height = textGen.GetPreferredHeight(letterValue, generationSettings);
            return new Size() {width = width, height = height};
        }
    }

    void LetterIsClicked()
    {
        if (!canBeClicked) return;
        onLetterClick?.Invoke(this);
    }

    public void MoveTo( Vector3 pos) {
        if (movingCoroutine != null) StopCoroutine(movingCoroutine);
        movingCoroutine =  StartCoroutine(Moving(pos));
    }
    private IEnumerator Moving(Vector3 pos, float delay = 0) {
        movingIndex = 0;
        yield return new WaitForSeconds(delay);
        Vector3 begin = rt.localPosition;
        float duration = .5f;
        while( movingIndex < duration) {
            movingIndex += Time.deltaTime;
            rt.localPosition = Vector3.LerpUnclamped(begin, pos, slideAnimation.Evaluate(movingIndex/ duration));
            yield return new WaitForFixedUpdate();
        }
        rt.localPosition = pos;
    }

}

[System.Serializable]
public class Size {
    public float width;
    public float height;
}