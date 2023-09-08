using System.Collections;
using UnityEngine;

public class playerDeathToken : MonoBehaviour
{
    public int tokenValue;
    public string faction;
    public PlayerHealth player;

    public bool tokenActivated = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(TokenActivation());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tokenActivated == true && other.GetComponent<PlayerHealth>().alive == true)
        {
            player = other.GetComponent<PlayerHealth>();
            player.UpdateSkills(tokenValue);
            tokenValue = 0;
            Destroy(gameObject);
        }
    }

    private IEnumerator TokenActivation()
    {
        yield return new WaitForSeconds(1f);
        tokenActivated = true;
    }
}
