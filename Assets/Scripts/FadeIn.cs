using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    private Image image;

    void OnEnable()
    {
        image = GetComponent<Image>();
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        float fadeInDuration = 3f;

        float timePassed = 0;

        while (timePassed < fadeInDuration)
        {
            float percent = timePassed / fadeInDuration;
            Color newColor = image.color;
            newColor.a = percent;
            image.color = newColor;

            timePassed += Time.deltaTime;
            yield return null;
        }
    }
}
