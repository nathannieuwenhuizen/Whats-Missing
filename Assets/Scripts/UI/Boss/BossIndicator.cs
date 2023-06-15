using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;
using UnityEngine.UI;

public class BossIndicator : MonoBehaviour
{
    [SerializeField] private Transform boss;
    [SerializeField] private Transform player;
    [SerializeField] private RectTransform rt;
    [SerializeField] private RectTransform rt_child;
    [SerializeField] private RectTransform rt_child_overlay;

    [Header("values")]
    [SerializeField] float maxAlpha = .2f;
    [SerializeField] float minAlpha = 0f;
    [SerializeField] float maxDistance = 50f;
    [SerializeField] float minDistance = 5f;

    private bool showAlpha = true;
    private void Update() {
        UpdateBossRotation();
    }


    private void UpdateBossRotation() {
        if (boss == null) return;

        Vector3 _pos = Camera.main.WorldToScreenPoint(boss.position);
        bool camReversed = CamIsBehind(Camera.main.transform.position - boss.position);
        _pos.z = 0;
        if (camReversed) {
            _pos.x *= -1;
            _pos.y *= -1;
        }
        Vector2 mid = new Vector2(Screen.width * .5f, Screen.height * .5f);
        Vector2 pos = new Vector2(_pos.x, _pos.y);

        Vector3 delta = pos - mid;
        float aspect = Mathf.Abs(delta.x) / Mathf.Abs(delta.y);
        float screenAspect = (float)Screen.width / (float)Screen.height;
        bool upside = aspect < screenAspect;

        float divider = Mathf.Abs( upside ? delta.y : delta.x) / ((upside ? Screen.height : Screen.width) * .5f);
        delta.x /= divider;
        delta.y /= divider;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        Rotation = angle - 90f;

        UpdateBossAlpha();
    }
    private float fadeSpeed = 5f;
    public void UpdateBossAlpha() {
        if (showAlpha) {
            float distance = Vector3.Distance(player.position, boss.position);
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            float endAlpha =  Mathf.Lerp(maxAlpha, minAlpha, distance / (maxDistance - minDistance));
            Vector3 direction = (boss.position - player.position).normalized;
            float angle = Vector3.Angle(Camera.main.transform.forward, direction);

            if (angle < 120 && angle > 45) {
                Alpha = Mathf.Lerp(Alpha, endAlpha, Time.deltaTime * fadeSpeed);
                OverlayAlpha = Mathf.Lerp(OverlayAlpha, 0, Time.deltaTime * fadeSpeed);   

            } else if ( angle >= 120){
                Alpha = Mathf.Lerp(Alpha, 0, Time.deltaTime * fadeSpeed);
                OverlayAlpha = Mathf.Lerp(OverlayAlpha, endAlpha * .5f, Time.deltaTime * fadeSpeed);
            } else {
                Alpha = Mathf.Lerp(Alpha, 0, Time.deltaTime * fadeSpeed);
                OverlayAlpha = Mathf.Lerp(OverlayAlpha, 0, Time.deltaTime * fadeSpeed);   

            }
            
        } else {
            Alpha = Mathf.Lerp(Alpha, 0, Time.deltaTime * fadeSpeed);
            OverlayAlpha = Mathf.Lerp(OverlayAlpha, 0, Time.deltaTime * fadeSpeed);
        }

    }

    private bool CamIsBehind(Vector3 _delta) {
        float dot = Vector3.Dot(_delta, Camera.main.transform.forward);
        return dot > 0;
    }

    public float Rotation {
        get { return rt.localEulerAngles.z; }
        set { 
            Quaternion temp = Quaternion.Euler(0,0,value);
            rt.localRotation = temp;
            temp = Quaternion.Euler(0,0, 180 - value);
            rt_child.localRotation = temp;
         }
    }
    public float Alpha {
        get { return rt_child.GetComponent<Image>().color.a;}
        set { 
            Color temp = rt_child.GetComponent<Image>().color;
            temp.a = value; 
            rt_child.GetComponent<Image>().color = temp;
        }
    }
    public float OverlayAlpha {
        get { return rt_child_overlay.GetComponent<Image>().color.a;}
        set { 
            Color temp = rt_child_overlay.GetComponent<Image>().color;
            temp.a = value; 
            rt_child_overlay.GetComponent<Image>().color = temp;
        }
    }

    private void OnEnable() {
        BossHitBox.OnHit += OnHit;
        BossCutsceneState.OnBossCutsceneStart += DisableAlpha;
        BossCutsceneState.OnBossCutsceneEnd += EnableAlpha;
    }
    private void OnDisable() {
        BossHitBox.OnHit -= OnHit;
        BossCutsceneState.OnBossCutsceneStart -= DisableAlpha;
        BossCutsceneState.OnBossCutsceneEnd -= EnableAlpha;
    }

    public void EnableAlpha(Boss.Boss boss, float zoomValue = 50f) {
        showAlpha = true;
    }
    public void DisableAlpha(Boss.Boss boss, float zoomValue = 50f) {
        showAlpha = false;
    }


    public void OnHit(float _val) {

    }
}
