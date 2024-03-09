using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class HighScore : MonoBehaviour
{
    static public int score = 1000; // a
    void Update()
    { // b
        Text gt = this.GetComponent<Text>();
        gt.text = "High Score: " + score;
    }
}
