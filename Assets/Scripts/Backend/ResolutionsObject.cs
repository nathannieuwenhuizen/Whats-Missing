using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Resolutions", order = 2)]
public class ResolutionsObject : ScriptableObject
{
    public Vector2[] SupportedResolutions;
}
