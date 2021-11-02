using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeOpacity : MonoBehaviour
{
    
    void Start()
    {
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        if (Input.anyKeyDown) StartCoroutine(FadeOut());
    }

    public IEnumerator FadeIn(int fadeSpeed = 1)
    {
        Color color = gameObject.GetComponent<Image>().color;
        float fadeAmount;

        while (gameObject.GetComponent<Image>().color.a > 0)
        {
            fadeAmount = color.a - (fadeSpeed * Time.deltaTime);
            color = new Color(color.r, color.g, color.b, fadeAmount);
            gameObject.GetComponent<Image>().color = color;
            yield return null;
        }
        
    }

    public IEnumerator FadeOut(int fadeSpeed = 1)
    {
        Color color = gameObject.GetComponent<Image>().color;
        float fadeAmount;

        while (gameObject.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = color.a + (fadeSpeed * Time.deltaTime);
            color = new Color(color.r, color.g, color.b, fadeAmount);
            gameObject.GetComponent<Image>().color = color;
            yield return null;
        }
        SceneManager.LoadScene(1);
    }
}
