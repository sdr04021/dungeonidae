using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocaleManager : MonoBehaviour
{
    IEnumerator Start()
    {
        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;
    }

    static void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    public void ButtonEngClick()
    {
        LocaleSelected(0);
    }
    public void ButtonKorClick()
    {
        LocaleSelected(1);
    }
}

//https://docs.unity3d.com/Packages/com.unity.localization@1.0/manual/Scripting.html

//https://forum.unity.com/threads/loading-string-tables-with-addressables.1345715/