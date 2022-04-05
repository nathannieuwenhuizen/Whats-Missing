using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
/// This handles the bossy body mesh. 
///It (de) activates and handles he secondary physics, purely the astethics
///</summary>
public class BossBody : MonoBehaviour
{
    //metamorphose values
    private bool hasMetamorphosed = false;
    private float metamorphoseDuration = 2f;
    private float metamorphoseDelay = 0f;
    private string metamorphoseKey = "index";
    [SerializeField]
    private AnimationCurve metamorphoseCurve = AnimationCurve.EaseInOut(0,0,1,1);

    [SerializeField]
    private Renderer[] bodyRenders;
    [SerializeField]
    private Renderer[] metamorphosedRenders;


    ///<summary>
    /// Toggles all the renderers of the boss body making it invisible
    ///</summary>
    public void ToggleBody(bool visible) {
        foreach (Renderer renderer in bodyRenders) renderer.enabled = visible;
        foreach (Renderer renderer in metamorphosedRenders) renderer.enabled = visible;
    }

    ///<summary>
    /// Time to transform!
    ///</summary>
    public void Metamorphose() {
        if (hasMetamorphosed) return;
        hasMetamorphosed = true;
        StartCoroutine(Matemorphosing());
    }

    //does the coroutine showing all the extra tentacles
    private IEnumerator Matemorphosing() {
        yield return new WaitForSeconds(metamorphoseDelay);
        foreach(Renderer renderer in metamorphosedRenders) {
            float randomDelay = Random.Range(0,.5f);
            StartCoroutine(renderer.material.AnimatingNumberPropertyMaterial(metamorphoseKey, 0, 1, metamorphoseCurve, 
            metamorphoseDuration - randomDelay * metamorphoseDuration , 
            randomDelay * metamorphoseDuration ));
        }
        yield return new WaitForSeconds(metamorphoseDuration);
    }
}
