using UnityEngine;

public class GateController : MonoBehaviour
{
    public float openHeight = 5f;
    public float speed = 2f;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool opening = false;

    private UnityEngine.AI.NavMeshObstacle obstacle;

    void Start()
    {
        closedPosition = transform.position;

        openPosition = closedPosition + Vector3.up * openHeight;

        obstacle = GetComponent<UnityEngine.AI.NavMeshObstacle>();
    }

    public void OpenGate()
    {
        opening = true;

        if (obstacle != null)
        {
            obstacle.enabled = false;
        }
    }

    void Update()
    {
        if (opening)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                openPosition,
                Time.deltaTime * speed
            );
        }
    }
}
