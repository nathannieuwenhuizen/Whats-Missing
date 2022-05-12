using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageDisolveParticles : MonoBehaviour
{
    [SerializeField]
    private Image image;

    [SerializeField]
    private ParticleSystem[] particles;

    [SerializeField]
    private Camera particleCamera;

    private void Awake() {
        particleCamera.enabled = false;
    }
    public void Disolve() {
        particleCamera.enabled = true;

        SetparticlesAtImagePosition();
        image.gameObject.SetActive(false);
        foreach(ParticleSystem particle in particles) {
            particle.Play();
        }
    }

    //TODO: make sure it also workds with rect transform scale and oddset anchors, it only works on (.5, .5)
    private void SetparticlesAtImagePosition() {
        Vector3 cameraPos = particleCamera.ScreenToWorldPoint(image.rectTransform.position);
        
        Vector3 minX = particleCamera.ScreenToViewportPoint(image.rectTransform.rect.min);
        Vector3 maxX = particleCamera.ScreenToViewportPoint(image.rectTransform.rect.max);
        float width = (maxX.x - minX.x) * Screen.width;
        float height = (maxX.y - minX.y) * Screen.height;
        Canvas canvas = FindObjectOfType<Canvas>();
        float h = canvas.GetComponent<RectTransform>().rect.height;
        float w = canvas.GetComponent<RectTransform>().rect.width;
        float aspect = w / h;
        float worldWidth = width / w * particleCamera.orthographicSize * 2f;
        float worldHeight = height / h * particleCamera.orthographicSize * 2f;
        worldWidth = worldWidth * aspect;
        Debug.Log("camera pos " + cameraPos);
        
        
        foreach(ParticleSystem particle in particles) {
            Vector3 pos = particle.transform.position;
            pos.x = cameraPos.x;
            pos.y = cameraPos.y;
            particle.transform.position = pos;

            particle.transform.localScale = new Vector3(worldWidth / 5f, worldHeight / 5f, 1f);
        }


        Debug.Log("world width " + worldWidth);
        Debug.Log("world height " + worldHeight);
        // Debug.Log("height " + height);
    }

    public static Rect RectTransformToScreenSpace(RectTransform transform)
     {
         Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
         return new Rect((Vector2)transform.position - (size * 0.5f), size);
     }
}
