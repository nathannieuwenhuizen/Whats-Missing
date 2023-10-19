using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class SlideListButton : MonoBehaviour
{
    [SerializeField] private SlideMainMenu slideMainMenu;
    [SerializeField] private SlideList slideList;
    private Button button;
    private void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => slideMainMenu.GoToSlideList(slideList)); 
    }
    
}
