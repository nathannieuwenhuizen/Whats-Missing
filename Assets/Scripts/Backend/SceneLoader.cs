using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
        [SerializeField]
        private Animator animator;
        public void LoadNewScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        public void LoadNextScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        public void LoadNewSceneWithTransition(string scene)
        {
            StartCoroutine(Transitioning(scene));
        }

        public void LoadNextSceneWithTransition()
        {
            StartCoroutine(Transitioning(SceneManager.GetActiveScene().buildIndex + 1));
        }

        private IEnumerator Transitioning(string scene)
        {
            animator.SetBool("LoadOut", true);
            yield return new WaitForSeconds(.5f);
            SceneManager.LoadScene(scene);
        }
        private IEnumerator Transitioning(int scene)
        {
            animator.SetBool("LoadOut", true);
            yield return new WaitForSeconds(.5f);
            SceneManager.LoadScene(scene);
        }

}
