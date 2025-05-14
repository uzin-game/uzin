using UnityEngine;
using UnityEngine.Events;

public class ConveyorBelt : MonoBehaviour
{

    public bool IsLeft;
    public bool IsRight;
    public bool IsTop;
    public bool IsBottom;
    public Vector2 conveyorDirection
    {
        get
        {
            if (IsRight) return Vector2.right;
            if (IsLeft) return Vector2.left;
            if (IsTop) return Vector2.up;
            if (IsBottom) return Vector2.down;
            return Vector2.zero;
        }
    } // Direction of the belt
    public float speed = 2f; // Speed of the conveyor

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name + "is moving to the " + conveyorDirection + " direction");
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.linearVelocity += conveyorDirection.normalized * speed * Time.deltaTime;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        rb.linearVelocity = Vector2.zero;
    }
}
