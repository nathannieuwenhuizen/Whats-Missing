using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Boss;
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class MirrorShardIndicator : MonoBehaviour
{
    [SerializeField]
    private MirrorShard shard;

    private float uiOffset = 20f;

    private RectTransform rt;
    private CanvasGroup cg;
    [SerializeField]
    private bool active = false;
    private bool inBossCutscene = false;
    public bool Active {
        get { return active;}
        set { 
            active = value; 
        }
    }

    private void Awake() {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
    }
    private void LateUpdate() {
        UpdateUIPosition();
        UpdateUIAlpha();
    }
    public void AssignShard(BossMirror _mirror) {
        int index = _mirror.AmmountOfShardsAttached();
        
        if (index >= 0 && index < _mirror.Shards.Length) {
            Active = true;
            if (shard != null) shard.Focusedshard = false;
            shard = _mirror.Shards[index];
            shard.Focusedshard = true;
        } else Active = false;
    }

    private void UpdateUIPosition() {
        if (!active) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(shard.transform.position);
        bool camReversed = CamIsBehind(Camera.main.transform.position - shard.transform.position);
        screenPos.z = 0;
        if (camReversed) {
            screenPos.x *= -1;
            screenPos.y *= -1;
        }
        rt.position = ClampedScreenPos(screenPos, camReversed);
    }
    private void UpdateUIAlpha () {
        if (inBossCutscene) return;
        if (!active) {
            cg.alpha = 0;
            return;
        }
        if (shard.Attached) {
            cg.alpha = 0;
            return;
        }

        float dist = Vector3.Distance (shard.transform.position, Camera.main.transform.position);
        float result = Extensions.Map(dist, 5,80,0,1);
        // Debug.Log("mapped value = " + result);
        cg.alpha = result;
    }

    private bool CamIsBehind(Vector3 _delta) {
        float dot = Vector3.Dot(_delta, Camera.main.transform.forward);
        return dot > 0;
    }


    private void OnEnable() {
        BossMirror.OnMirrorShardAmmountUpdate += AssignShard;
        BossCutsceneState.OnBossCutsceneStart += HideUI;
        BossCutsceneState.OnBossCutsceneEnd += ShowUI;
    }

    private void OnDisable() {
        BossMirror.OnMirrorShardAmmountUpdate -= AssignShard;
        BossCutsceneState.OnBossCutsceneStart -= HideUI;
        BossCutsceneState.OnBossCutsceneEnd -= ShowUI;
    }

    private void HideUI(Boss.Boss boss, float zoomValue = 50f) {
        inBossCutscene = true;
        StartCoroutine(cg.FadeCanvasGroup(0,1f,0));
    }
    private void ShowUI(Boss.Boss boss, float zoomValue = 50f) {
        inBossCutscene = false;
        StartCoroutine(cg.FadeCanvasGroup(1f,1f,0));
    }

    //log converts a screen position to not go out of bounds
    private Vector3 ClampedScreenPos(Vector3 _pos, bool _focrdClamp) {
        Vector3 result = _pos;
        Vector2 mid = new Vector2(Screen.width * .5f, Screen.height * .5f);
        Vector2 pos = new Vector2(_pos.x, _pos.y);

        Vector3 delta = pos - mid;
        float aspect = Mathf.Abs(delta.x) / Mathf.Abs(delta.y);
        float screenAspect = (float)Screen.width / (float)Screen.height;
        bool upside = aspect < screenAspect;

        if (Mathf.Abs(delta.x) > (Screen.width * .5f) || Mathf.Abs(delta.y) > (Screen.height * .5f) || _focrdClamp) {
            float divider = Mathf.Abs( upside ? delta.y : delta.x) / ((upside ? Screen.height : Screen.width) * .5f);
            delta.x /= divider;
            delta.y /= divider;
            result.x = mid.x + delta.x;
            result.y = mid.y + delta.y;
            result -= delta.normalized * uiOffset;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            rt.rotation = Quaternion.Euler(0,0, angle + 90f);
        } else {
            rt.rotation = Quaternion.Euler(0,0, 0);
        }

        return result;
    }
}
