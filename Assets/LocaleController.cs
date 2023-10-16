using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleController : MonoBehaviour
{
    bool isChanging = false;

    public void ChangeLocale(int index)
    {
        if (isChanging)
            return;

        StartCoroutine(ChangeLocaleCoroutine(index));
    }
    
    IEnumerator ChangeLocaleCoroutine(int index)
    {
        isChanging = true;
        
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
       
        //LocalizationSettings.StringDatabase.GetLocalizedString("Table", "Key", "current locale");
        
        isChanging = false;
    }
}
