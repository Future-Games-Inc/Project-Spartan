using UnityEngine;
using TMPro;


public class PlayerNameTag : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerTag;


    // Start is called before the first frame update
    private void Start()
    {
        SetName();
    }

    // Update is called once per frame
    private void SetName()
    {
        playerTag.text = PlayerPrefs.GetString("Nickname");
    }
}
