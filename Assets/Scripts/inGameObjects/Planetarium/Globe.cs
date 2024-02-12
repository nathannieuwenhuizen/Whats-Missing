using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globe : InteractabelObject
{
    [SerializeField]
    private Transform sphere;
    private float spinSpeed = 0;
    private float drag = 2.5f;
    private float maxSpinSpeed = 10f;
    private Coroutine spinCoroutine;
        private SFXInstance spinSound;

    public override void Interact()
    {
        base.Interact();
        if (spinSpeed <= 0) {
            if (spinSound == null) {
                spinSound =  AudioHandler.Instance.Play3DSound(SFXFiles.globe, transform, 1f, 1, true, true, 40f, false);
            }
            spinSound.Play();
        }
        spinSpeed = Mathf.Min(maxSpinSpeed, spinSpeed + Random.Range(3, 5f));

            if (spinCoroutine != null) StopCoroutine(spinCoroutine);
            spinCoroutine = StartCoroutine(Spinning());
            
        
    }

    private IEnumerator Spinning() {
        while (spinSpeed > 0) {
            spinSpeed -= drag * Time.deltaTime;
            sphere.Rotate(new Vector3(0,0,spinSpeed * Time.deltaTime * 100));
            spinSound.Volume =Mathf.Sin((spinSpeed / maxSpinSpeed) * (Mathf.PI / 2f));
            yield return new WaitForEndOfFrame();
        }
        spinSound.Pause();
    }

    public void OnPause() {
        if (spinSpeed > 0) spinSound.Pause();

    }
    public void OnResume() {
        if (spinSpeed > 0) spinSound.Play();
    }

    private void OnEnable() {
        PauseScreen.OnPause += OnPause;
        PauseScreen.OnResume += OnResume;
    }
    private void OnDisable() {
        PauseScreen.OnPause -= OnPause;
        PauseScreen.OnResume -= OnResume;
        if (spinSound != null) spinSound.Stop(true);
    }

    private void Reset() {
        Word = "globe";
        AlternativeWords = new string[] { "world" };
    }


}
