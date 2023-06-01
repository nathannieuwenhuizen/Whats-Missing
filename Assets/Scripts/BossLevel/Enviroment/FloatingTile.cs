using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTile : MonoBehaviour
{
    [SerializeField]
    private float floatAmplitude = .2f;
    private float floatIndex;
    [SerializeField]
    private float cycleDuration = 5f;
    private Vector3 startLocalPos;
    private void Start() {
        floatIndex = Random.Range(0, cycleDuration);
        startLocalPos = transform.localPosition;
    }
    private void OnEnable() {
        floatIndex = Random.Range(0, cycleDuration);
        startLocalPos = transform.localPosition;
    }

    private void Update() {
        floatIndex = (floatIndex + Time.deltaTime) % cycleDuration;
        Vector3 wave = new Vector3(0, Mathf.Sin((Mathf.PI * 2) * (floatIndex / cycleDuration)) * floatAmplitude,0);
        transform.localPosition = startLocalPos + wave;
    }
}
