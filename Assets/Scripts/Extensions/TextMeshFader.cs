using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMeshFader : MonoBehaviour
{
    [SerializeField]
    private TextMesh mesh;
    private List<TextMesh> meshes = new List<TextMesh>();

    [SerializeField]
    private AnimationCurve animCurve = AnimationCurve.EaseInOut(0,0, 1, 1);

    [SerializeField]
    private float duration = 2f;

    [SerializeField]
    private float totalDelay = 1f;

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
        float offset = 0;
        for (int i = 0; i < mesh.text.Length; i++)
        {
            GameObject go = Instantiate(mesh.gameObject, mesh.transform.position, mesh.transform.rotation);
            go.name = mesh.text[i].ToString();
            TextMesh newMesh = go.GetComponent<TextMesh>();
            newMesh.text = go.name;
            go.transform.SetParent(transform);
            go.transform.position += transform.right * offset;
            // Debug.Log(GetWidth(newMesh));
            offset += GetWidth(newMesh);
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

    private IEnumerator FadeLetter(TextMesh mesh, float delay, bool fadeIn = false) {
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

    public static float GetWidth(TextMesh mesh)
    {
        float width = 0;
        
        CharacterInfo info;
        mesh.font.RequestCharactersInTexture(mesh.text, mesh.fontSize,mesh.fontStyle); //add this
        if (mesh.font.GetCharacterInfo(mesh.text[0], out info, mesh.fontSize))
        {
            width += info.width;
        }
        
        return width * mesh.characterSize * 0.1f * mesh.transform.lossyScale.x;
    }
    public void SetAlpha(TextMesh mesh, float val)
    {
        Color col = mesh.color;
        col.a = val;
        mesh.color = col;
    }
}
