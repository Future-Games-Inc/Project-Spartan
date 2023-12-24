using UnityEngine;
using UnityEngine.VFX;

public class SpawnShieldRipples : MonoBehaviour
{
    public GameObject shieldRipples;

    private VisualEffect shieldRipplesVFX;

    public GameObject player;

    void OnEnable()
    {
        transform.position = player.transform.position;
    }

    private void Update()
    {
        transform.position = player.transform.position;
    }
    private void OnCollisionEnter(Collision co)
    {
        var ripples = Instantiate(shieldRipples, transform) as GameObject;
        shieldRipplesVFX = ripples.GetComponent<VisualEffect>();
        shieldRipplesVFX.SetVector3("SphereCenter", co.contacts[0].point);

        Destroy(ripples, 2);
        if (co.gameObject.CompareTag("EnemyBullet"))
            Destroy(co.gameObject);
    }
}
