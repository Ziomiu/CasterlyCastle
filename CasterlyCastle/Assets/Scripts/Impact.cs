using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float destroyAfter = 2f;

    void Start()
    {
        Destroy(gameObject, destroyAfter);
    }
}