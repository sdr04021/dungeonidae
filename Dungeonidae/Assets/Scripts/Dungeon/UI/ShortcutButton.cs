using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShortcutButton : MonoBehaviour
{
    public Image icon;
    public Image curtain;
    public TMP_Text coolDownText;
    public GameObject pressed;
    public KeyCode key;

    private void Update()
    {
        if(Input.GetKeyDown(key))
            pressed.SetActive(true);
        if (Input.GetKeyUp(key))
            pressed.SetActive(false);
    }
}
