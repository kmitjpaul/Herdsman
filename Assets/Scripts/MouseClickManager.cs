using UnityEngine;

public class MouseClickManager : MonoBehaviour
{
    private Camera _cam;

    private Vector2 _posToMoveTo = Vector2.zero;
    public float distanceToColliderSurfaceRatio = 0.3f;
    public float movementSpeed = 10;
    public Transform player;
    public LayerMask rayCastTarget;

    private void Start()
    {
        _cam = Camera.main;
        _posToMoveTo = player.position;
    }

    private void Update()
    {
        ProcessInput();
    }

    private void FixedUpdate()
    {
        player.position = Vector2.MoveTowards(player.position, _posToMoveTo, movementSpeed * Time.deltaTime);
    }

    private void ProcessInput()
    {
        if (!Input.GetButtonDown("Fire1"))
            return;

        var position = player.position;
        var point = _cam.ScreenToWorldPoint(Input.mousePosition);

        var diff = point - position;

        var h = Physics2D.Raycast(position, diff, diff.magnitude, rayCastTarget);

        _posToMoveTo = h ? h.point + h.normal * distanceToColliderSurfaceRatio : (Vector2) point;
    }
}