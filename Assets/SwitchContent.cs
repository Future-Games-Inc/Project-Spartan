using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwitchContent : MonoBehaviour
{
    // Start is called before the first frame update
    public void SeePass()
    {
        GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.Standard;
        GetComponent<TMP_InputField>().ForceLabelUpdate();

    }

    public void HidePass()
    {
        GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.Password;
        GetComponent<TMP_InputField>().ForceLabelUpdate();
    }
}
