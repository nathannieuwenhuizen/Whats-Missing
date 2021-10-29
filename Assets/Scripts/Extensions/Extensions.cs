using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{


    public static Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }

    public static string Shuffle(string _list)  
    {  
        Random.InitState(_list.Length);
        string result = "";
        int rngIndex;
        int test = 0;
        List<char> chars = new List<char>(_list.ToCharArray());

        while (chars.Count > 0 && test < 100) {  
            test++;
            rngIndex = Mathf.FloorToInt(Random.Range(0, chars.Count));
            result += chars[rngIndex];
            chars.RemoveAt(rngIndex);
        } 
        return result;
    }


    public static Vector3 RandomVector(float maxValue) {
        return new Vector3(
            Random.Range(-maxValue, maxValue),
            Random.Range(-maxValue, maxValue),
            Random.Range(-maxValue, maxValue));
    }

    public static Vector3 CalculateLinearBezierPoint(float t, Vector3 p0, Vector3 p1) {
        return p0 + t * (p1 - p0);
    }


    public static IEnumerator FadeCanvasGroup( this CanvasGroup group, float end, float duration, float delay = 0) {
        yield return new WaitForSeconds(1f);
        float index = 0;
        float begin = group.alpha;
         while (index < duration) {
             yield return new WaitForEndOfFrame();
             index += Time.unscaledDeltaTime;
             group.alpha = Mathf.Lerp(begin, end, index / duration);
         }
        group.alpha = end;
    }

    public static  IEnumerator AnimatingScale(this Transform transform, Vector3 endScale,  AnimationCurve curve, float duration = .5f) {
        float timePassed = 0f;
        Vector3 beginScale = transform.localScale;
        while (timePassed < duration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.unscaledDeltaTime;
            transform.localScale = Vector3.LerpUnclamped(beginScale, endScale , curve.Evaluate(timePassed / duration));
        }
        transform.localScale = endScale;

    }

    public static  IEnumerator AnimatingDissolveMaterial(this Material mat, float beginVal, float endVal,  AnimationCurve curve, float duration = .5f, float edgeWidth = .1f) {
        mat.SetFloat("EdgeWidth", edgeWidth);
        mat.SetFloat("Dissolve", beginVal);
        float timePassed = 0f;

        while (timePassed < duration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.unscaledDeltaTime;
            mat.SetFloat("Dissolve", Mathf.LerpUnclamped(beginVal, endVal , curve.Evaluate(timePassed / duration)));
        }
        mat.SetFloat("Dissolve", endVal);
        mat.SetFloat("EdgeWidth", 0);
    }
    public static  IEnumerator AnimatingSnowMaterial(this Material mat, float beginVal, float endVal,  AnimationCurve curve, float duration = .5f) {
        mat.SetFloat("Opacity", beginVal);
        float timePassed = 0f;

        while (timePassed < duration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.unscaledDeltaTime;
            mat.SetFloat("Opacity", Mathf.LerpUnclamped(beginVal, endVal , curve.Evaluate(timePassed / duration)));
        }
        mat.SetFloat("Opacity", endVal);
    }

    public static void SetNearClipPlane(this Camera reflectionCamera, Transform transform, Camera mainCamera) {
        Transform clipPlane = transform; 
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, clipPlane.position - reflectionCamera.transform.position));

        Vector3 cameraSpacePos = reflectionCamera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        int revert = clipPlane.position.y < mainCamera.transform.position.y ? 1 : -1;
        Vector3 cameraSpaceNormal = reflectionCamera.worldToCameraMatrix.MultiplyVector(clipPlane.up * revert) * dot;
        float camSpaceDst = -Vector3.Dot(cameraSpacePos, cameraSpaceNormal);
        Vector4 clipPlaneCameraSpace = new Vector4(cameraSpaceNormal.x, cameraSpaceNormal.y, cameraSpaceNormal.z, camSpaceDst);


        reflectionCamera.projectionMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
    }

    public static bool IncameraRange(this Renderer renderer, Camera mainCamera) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if(GeometryUtility.TestPlanesAABB(planes, renderer.bounds)){
            return true;
        } else {
            return false;   
        }
    }

}
