using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public Image fadeImage;
    public int nextSceneIndex;

    public void TransitionScene()
    {
        isTransitioning = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTransitioning = true;
        }
    }
    public float transitionSpeed;
    public bool isTransitioning = false;
    private void Update()
    {
        if (isTransitioning)
        {
            if (fadeImage.color.a < 1)
            {
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, Mathf.Clamp01(fadeImage.color.a + transitionSpeed * Time.deltaTime));
            }
            else
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }

}