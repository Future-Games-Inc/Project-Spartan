using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTimer : MonoBehaviour
{
    public TextMeshProUGUI[] textElements;
    private int currentElement;

    private void Start()
    {
        currentElement = 0;
        StartCoroutine(ActivateDeactivate());
    }

    IEnumerator ActivateDeactivate()
    {
        while (true)
        {
            textElements[currentElement].gameObject.SetActive(false);
            currentElement = (currentElement + 1) % textElements.Length;
            textElements[currentElement].gameObject.SetActive(true);
            yield return new WaitForSeconds(10f);
        }
    }
}
