using UnityEngine;

// Attach to a door's hinge pivot (the door leaf must be a child of this transform, offset from the
// hinge so rotating the pivot swings it). Needs a Player-tagged object with a Collider (and at least
// one side of the interaction — this object or the player — needs a Rigidbody for triggers to fire).
public class DoorInteractable : MonoBehaviour
{
    public float openAngle = -100f;
    public float openSpeed = 3f;

    bool isOpen;
    bool playerInRange;
    Quaternion closedRotation;
    Quaternion targetRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        targetRotation = closedRotation;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            targetRotation = isOpen ? closedRotation * Quaternion.Euler(0f, openAngle, 0f) : closedRotation;
        }

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * openSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
