using System.Collections;
using UnityEngine;

public class ReactorCover : MonoBehaviour
{
    public int Health;
    public MatchEffects matchEffects;
    public bool hit;
    public GameObject explosionEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (!hit)
            StartCoroutine(Hit());
        if (Health <= 0)
        {
            matchEffects.spawnReactor = true;
        }
    }

    IEnumerator Hit()
    {
        hit = true;
        explosionEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        explosionEffect.SetActive(false);
        hit = false;
    }
}
