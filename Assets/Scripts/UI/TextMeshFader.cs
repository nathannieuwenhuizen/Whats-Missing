using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextMeshFader : MonoBehaviour
{
    [SerializeField]
    private TMP_Text mesh;

    private List<TMP_Text> meshes = new List<TMP_Text>();

    [SerializeField]
    private AnimationCurve animCurve = AnimationCurve.EaseInOut(0,0, 1, 1);

    [SerializeField]
    protected float duration = 2f;

    [SerializeField]
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
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].gameObject.SetActive(value);
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
            meshes.Add(newMesh);
        }
        mesh.gameObject.SetActive(false);
    }

    ///<summary>
    /// Resets all the latters back to their spawn position.
    ///</summary>
    public void ResetPosition() {
        for (int i = 0; i < meshes.Count; i++)
        {
            meshes[i].transform.position = GetPositionOfCharacter(mesh, i);
        }
    }


    ///<summary>
    /// Fades the whole text in
    ///</summary>
    public virtual void FadeIn() {
        if (visible) return;
        visible = true;
        Active = true;
        StopAllCoroutines();
        ResetPosition();
        for (int i = 0; i < meshes.Count; i++)
        {
            StartCoroutine(FadeLetter(meshes[i], delayBeforeAppearAnimation + ((float)i / (float)meshes.Count) * totalDelay, true));
        }
    }
    ///<summary>
    /// Fades the whole text out.
    ///</summary>
    public void FadeOut() {
        if (!visible) return;
        visible = false;
        StopAllCoroutines();
        for (int i = 0; i < meshes.Count; i++)
        {
            StartCoroutine(FadeLetter(meshes[i],  ((float)i / (float)meshes.Count) * totalDelay, false));
        }
    }

    ///<summary>
    /// Fades one letter in.
    ///</summary>
    private IEnumerator FadeLetter(TMP_Text mesh, float delay, bool fadeIn = false) {
        float index = 0;
        float start = fadeIn ? 0 : 1;
        float end = fadeIn ? 1 : 0;


        Vector3 randomPos = mesh.transform.position + new Vector3(
            Random.Range(-offSetDistance, offSetDistance),
            Random.Range(-offSetDistance, offSetDistance),
            Random.Range(-offSetDistance, offSetDistance)
        );
        Vector3 startPos = fadeIn ? randomPos: mesh.transform.position;
        Vector3 endPos = fadeIn ? mesh.transform.position : randomPos;

        mesh.gameObject.transform.position = startPos;

        SetAlpha(mesh, start);
        yield return new WaitForSeconds(delay);
        while (index < duration) {

            index += Time.unscaledDeltaTime;
            SetAlpha(mesh, Mathf.Lerp(start, end, animCurve.Evaluate(index / duration)));
            mesh.gameObject.transform.position = Vector3.Lerp(startPos, endPos, animCurve.Evaluate(index / duration));
            yield return new WaitForEndOfFrame();
        }

        mesh.gameObject.transform.position = fadeIn ?  endPos : startPos;
        SetAlpha(mesh, end);
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

    public void SetAlpha(TMP_Text mesh, float val)
    {
        Color col = mesh.color;
        col.a = val;
        mesh.color = col;
    }
}
