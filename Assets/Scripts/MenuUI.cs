using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text scoreText;

    private void Start()
    {
        // ugh. sometimes I just don't have the brainpower to do it all and some code ends up kinda yuck.
        // I'd really rather not just straight up grab it, I'd rather a more elegant way. But I'm workin hard man!
        // and since it's just me - fuck it. That's right. fuck it.
        scoreText.text = "Your family will receive $" + Scoreboard.totalScore + ".00";
    }
}
