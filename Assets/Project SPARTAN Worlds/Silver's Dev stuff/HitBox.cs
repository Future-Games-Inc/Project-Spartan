using UnityEngine;

public class HitBox : MonoBehaviour
{
    void Start()
    {
        ApplyTagRecursively(gameObject.transform);
    }
    void ApplyTagRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.tag = this.gameObject.tag;
            ApplyTagRecursively(child);
        }
    }
}
