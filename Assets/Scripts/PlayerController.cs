using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpForce = 50.0f;

    public float breakDistance = 5.0f;

    public LayerMask chunkMeshMask;

    [Range(0, .3f)]
    public float movementSmoothing = .05f;

    private new Rigidbody rigidbody;
    private Vector3 velocity = new Vector3();

    void Start()
    {
        this.rigidbody = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        var forward = Camera.main.transform.forward;
        forward.y = 0.0f;

        this.velocity = forward * Input.GetAxisRaw("Vertical");
        this.velocity += Camera.main.transform.right * Input.GetAxisRaw("Horizontal");
        this.velocity *= this.speed * Time.deltaTime * 10.0f;

        if (Input.GetButtonDown("Jump"))
        {
            this.rigidbody.AddForce(new Vector3(0, this.jumpForce * 10.0f, 0));
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hit, this.breakDistance, this.chunkMeshMask))
            {
                var chunk = hit.collider.GetComponent<ChunkMesh>();

                var coordinates = hit.point + -hit.normal * 0.5f;

                var x = Mathf.RoundToInt(coordinates.x) % 16;
                var y = Mathf.RoundToInt(coordinates.y) % 16;
                var z = Mathf.RoundToInt(coordinates.z) % 16;

                chunk.BreakBlock(new Vector3Int(x, y, z));
            }
        }
    }

    void FixedUpdate()
    {
        var targetVelocity = velocity;
        targetVelocity.y = this.rigidbody.velocity.y;

        this.rigidbody.velocity = Vector3.SmoothDamp(this.rigidbody.velocity, targetVelocity, ref targetVelocity, this.movementSmoothing);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * this.breakDistance);
    }
}
