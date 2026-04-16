using UnityEngine;

public class GateController : MonoBehaviour
{
    public float openHeight = 5f;
    public float speed = 2f;
    public float delay = 5f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool opening = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;

        Invoke(nameof(OpenGate), delay);
    }

    void OpenGate()
    {
        opening = true;
    }

    void Update()
    {
        if (opening)
        {
            transform.position = Vector3.Lerp(transform.position, openPosition, Time.deltaTime * speed);
        }
    }
}