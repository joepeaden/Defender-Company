using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFadeInOut : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text subTitle;

    public void ShowAnnouncementText(string newTitle, string newSubTitle)
    {
        StartCoroutine(AnnouncementTextFade(newTitle, newSubTitle));
    }

    //StartCoroutine(AnnouncementTextFade(title, subtitle));
    private IEnumerator AnnouncementTextFade(string newTitle, string newSubTitle)
    {
        title.text = newTitle;
        subTitle.text = newSubTitle;

        float timePassed = 0f;
        title.alpha = 0f;
        title.gameObject.SetActive(true);
        subTitle.alpha = 0f;
        subTitle.gameObject.SetActive(true);

        while (timePassed < 1f)
        {
            float percent = timePassed / 1f;
            title.alpha = percent;
            subTitle.alpha = percent;

            timePassed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        while (timePassed > 0f)
        {
            float percent = timePassed / 1f;
            title.alpha = percent;
            subTitle.alpha = percent;

            timePassed -= Time.deltaTime;
            yield return null;
        }

        title.gameObject.SetActive(false);
        subTitle.gameObject.SetActive(false);
    }
}
