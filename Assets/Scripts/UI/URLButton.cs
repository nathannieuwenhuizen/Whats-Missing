using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.UI.Menu
{
    ///<summary>
    /// Button that opens an URL in the browser when clicked
    ///</summary>
    public class URLButton : MonoBehaviour
    {
        [SerializeField]
        private string url;
        protected Button button;

        private void OnEnable() {
            button = GetComponent<Button>();
            button.onClick.AddListener(OpenURL);
        }

        private void OnDisable() {
            button.onClick.RemoveListener(OpenURL);
        }

        public void OpenURL()
        {
            Application.OpenURL(url);
        }
    }
}