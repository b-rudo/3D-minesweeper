/* 6/30/22 */
/* Only use script on GameObject with attached TMP U GUI component! */
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetAppVersNumTMP : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<TextMeshProUGUI>().text = "Vers. " + Application.version + "   |   2022";
    }
}
