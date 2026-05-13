using UnityEngine;
using UnityEngine.InputSystem;

public class GateButton : MonoBehaviour
{
    public GateController gate;

    [Header("UI")]
    public GameObject pressEText;

    [Header("Button Visual")]
    public Transform buttonVisual;
    public float pressDepth = 0.15f;
    public float pressDuration = 0.15f;

    private bool playerNearby = false;
    private bool wasPressed = false;

    private Vector3 buttonStartPosition;

    void Start()
    {
        if (pressEText != null)
        {
            pressEText.SetActive(false);
        }

        if (buttonVisual != null)
        {
            buttonStartPosition = buttonVisual.localPosition;
        }
    }

    void Update()
    {
        if (playerNearby &&
            !wasPressed &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            wasPressed = true;

            if (pressEText != null)
            {
                pressEText.SetActive(false);
            }

            if (gate != null)
            {
                gate.OpenGate();
            }

            if (buttonVisual != null)
            {
                StartCoroutine(PressButtonAnimation());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !wasPressed)
        {
            playerNearby = true;

            if (pressEText != null)
            {
                pressEText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            if (pressEText != null)
            {
                pressEText.SetActive(false);
            }
        }
    }

    private System.Collections.IEnumerator PressButtonAnimation()
    {
        Vector3 pressedPosition = buttonStartPosition - transform.forward * pressDepth;

        buttonVisual.localPosition = pressedPosition;

        yield return new WaitForSeconds(pressDuration);

        buttonVisual.localPosition = buttonStartPosition;
    }
}
