using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public struct LetterMesh {
    public TMP_Text mesh;
    public Vector3 startPos;
    public Vector3 randomPos;
    public Vector3 startRotation;
    public Vector3 randomRotation;
}


public class TextMeshFader : MonoBehaviour
{
    [SerializeField]
    private TMP_Text mesh;

    private List<LetterMesh> letterMeshes = new List<LetterMesh>();

    [SerializeField]
    private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0,0, 1, 1);

    protected float animationDuration = 1f;

    protected float totalDelay = 1f;
    [SerializeField]
    private float delayBeforeAppearAnimation = 0;

    [SerializeField]
    private float offSetDistance;

    [SerializeField]
    protected bool visible = true;

    protected bool active = true;
    ///<summary>
    /// If the text is active?. idk
    ///</summary>
    public bool Active {
        get { return active;}
        set {
            active = value;
            if (active == false) {
                visible = false;
            }
            for (int i = 0; i < letterMeshes.Count; i++)
            {
                letterMeshes[i].mesh.gameObject.SetActive(value);
            }
        }
    }


    protected virtual void Start() {
        InitializeLetters();
    }

    ///<summary>
    /// Creates all the letters from the mesh object.
    ///</summary>
    private void InitializeLetters() {
        for (int i = 0; i < mesh.text.Length; i++)
        {
            GameObject go = Instantiate(mesh.gameObject, mesh.transform.position, mesh.transform.rotation);
            go.name = mesh.text[i].ToString();
            TMP_Text newMesh = go.GetComponent<TMP_Text>();
            newMesh.text = go.name;
            go.transform.SetParent(transform);
            go.transform.position = GetPositionOfCharacter(mesh, i);
            LetterMesh newLetter = new LetterMesh() { mesh = newMesh, startPos = go.transform.localPosition, startRotation = mesh.transform.rotation.eulerAngles};
            newLetter.randomRotation = TransformExtensions.RandomRotation(5f).eulerAngles;
            newLetter.randomPos = go.transform.localPosition + TransformExtensions.RandomVector(offSetDistance);
            letterMeshes.Add(newLetter);
        }
        mesh.gameObject.SetActive(false);
    }

    ///<summary>
    /// Fades the whole text in
    ///</summary>
    public virtual void FadeIn() {
        if (visible) return;
        visible = true;
        Active = true;
        StopAllCoroutines();
        for (int i = 0; i < letterMeshes.Count; i++)
        {
            float delay = delayBeforeAppearAnimation + ((float)i / (float)letterMeshes.Count) * totalDelay;
            StartCoroutine(letterMeshes[i].mesh.rectTransform.AnimatingLocalRotation(letterMeshes[i].randomRotation, letterMeshes[i].startRotation, animationCurve, animationDuration, delay));
            letterMeshes[i].mesh.alpha = 0;
            StartCoroutine(letterMeshes[i].mesh.AnimateTextAlpha(1f, animationDuration, delay));
            letterMeshes[i].mesh.rectTransform.localPosition = letterMeshes[i].randomPos;
            StartCoroutine(letterMeshes[i].mesh.rectTransform.AnimateLocalPosition(letterMeshes[i].startPos, animationDuration, delay));
            letterMeshes[i].mesh.rectTransform.localScale = Vector3.zero;
            StartCoroutine(letterMeshes[i].mesh.rectTransform.AnimateLocalScale(Vector3.one, animationDuration, delay));

        }
    }
    ///<summary>
    /// Fades the whole text out.
    ///</summary>
    public void FadeOut() {
        if (!visible) return;
        visible = false;
        StopAllCoroutines();
        for (int i = 0; i < letterMeshes.Count; i++)
        {
            float delay = ((float)i / (float)letterMeshes.Count) * totalDelay;
            
            StartCoroutine(letterMeshes[i].mesh.rectTransform.AnimatingLocalRotation(letterMeshes[i].startRotation, letterMeshes[i].randomRotation, animationCurve, animationDuration, delay));
            StartCoroutine(letterMeshes[i].mesh.AnimateTextAlpha(0f, animationDuration, delay));
            StartCoroutine(letterMeshes[i].mesh.rectTransform.AnimateLocalPosition(letterMeshes[i].randomPos, animationDuration, delay));
            StartCoroutine(letterMeshes[i].mesh.rectTransform.AnimateLocalScale(Vector3.zero, animationDuration, delay));

        }
    }

    ///<summary>
    /// Returns the world position of a charachter in the tmp tect mesh.
    ///</summary>
    public Vector3 GetPositionOfCharacter(TMP_Text tmp_text, int index)
    {
        tmp_text.ForceMeshUpdate();
        Vector3[] vertices = tmp_text.mesh.vertices;
        TMP_CharacterInfo charInfo = tmp_text.textInfo.characterInfo[index];
        int vertexIndex = charInfo.vertexIndex;
        float divider = (tmp_text.text[index].ToString() == "." ? .3f : 0);
        Vector2 charMidTopLine = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, (charInfo.bottomLeft.y + charInfo.topLeft.y) / 2);
        charMidTopLine = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, (vertices[vertexIndex + 0].y + vertices[vertexIndex + 2].y) / 2 + divider);
        Vector3 worldPos = tmp_text.transform.TransformPoint(charMidTopLine);
        return worldPos;
    }
}
