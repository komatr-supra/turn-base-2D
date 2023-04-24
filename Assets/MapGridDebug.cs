using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapGridDebug : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    public void SetText(string text)
    {
        tmp.text = text;
    }
}
