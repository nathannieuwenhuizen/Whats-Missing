using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    public Slider Slider {
        get { return slider;}
        set { slider = value; }
    }
}

