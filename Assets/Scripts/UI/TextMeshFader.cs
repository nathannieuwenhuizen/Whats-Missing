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
    private float offSetDistance;

    private void Start() {
        InitializeLetters();
        StartCoroutine(Test());
    }

    public IEnumerator Test() {
        while (true) {

            FadeIn();
            yield return new WaitForSeconds(5f);
            FadeOut();
            yield return new WaitForSeconds(5f);
        }

    }
    private void InitializeLetters() {
        for (int i = 0; i < mesh.text.Length; i++)
        {
            GameObject go = Instantiate(mesh.gameObject, mesh.transform.position, mesh.transform.rotation);
            go.name = mesh.text[i].ToString();
            TMP_Text newMesh = go.GetComponent<TMP_Text>();
            newMesh.text = go.name;
            go.transform.SetParent(transform);
            go.transform.position = GetPositionOfCharacter(mesh, i);

            Debug.Log(GetPositionOfCharacter(mesh, i));
            meshes.Add(newMesh);
        }
        mesh.gameObject.SetActive(false);
    }

    public void FadeIn() {
        for (int i = 0; i < meshes.Count; i++)
        {
            StartCoroutine(FadeLetter(meshes[i],  ((float)i / (float)meshes.Count) * totalDelay, true));
        }
    }
    public void FadeOut() {
        for (int i = 0; i < meshes.Count; i++)
        {
            StartCoroutine(FadeLetter(meshes[i],  ((float)i / (float)meshes.Count) * totalDelay, false));
        }
    }

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

    public Vector3 GetPositionOfCharacter(TMP_Text tmp_text, int index)
    {
        tmp_text.ForceMeshUpdate();
        Vector3[] vertices = tmp_text.mesh.vertices;
        TMP_CharacterInfo charInfo = tmp_text.textInfo.characterInfo[index];
        int vertexIndex = charInfo.vertexIndex;
        Vector2 charMidTopLine = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, (charInfo.bottomLeft.y + charInfo.topLeft.y) / 2);
        Vector3 worldPos = tmp_text.transform.TransformPoint(charMidTopLine);
        return worldPos;
    }

    public static Vector2 GetWidth(TMP_Text text)
    {
        text.ForceMeshUpdate();
        Size size = new Size() {width = text.GetRenderedValues(false).x, height = text.GetRenderedValues(false).y};
        return new Vector2(size.width, size.height);
    }
    public void SetAlpha(TMP_Text mesh, float val)
    {
        Color col = mesh.color;
        col.a = val;
        mesh.color = col;
    }
}
