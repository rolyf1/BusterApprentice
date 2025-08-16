using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody RigidBody;
    public float moveSpeed;
    private Vector3 moveDir;

    public InputActionReference move;

    private void update()
    {
        moveDir = move.action.ReadValue<Vector3>();
    }

    private void fixUpdate()
    {
        RigidBody.linearVelocity = new Vector3((moveDir.x * moveSpeed), (moveDir.y * moveSpeed), 0);
    }
}