using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float gravity = -20f;

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Kiểm tra chạm đất
        CheckGrounded();

        // Reset velocity Y khi chạm đất
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Đọc input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Hướng di chuyển trong không gian 3D
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Di chuyển
        if (moveDirection.magnitude >= 0.1f)
        {
            // Xoay nhân vật theo hướng di chuyển
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Di chuyển nhân vật
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        // Nhảy
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Áp dụng trọng lực
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Cập nhật animation
        UpdateAnimation(moveDirection.magnitude);
    }

    void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            // Fallback: dùng CharacterController.isGrounded
            isGrounded = controller.isGrounded;
        }
    }

    void UpdateAnimation(float speed)
    {
        if (animator == null) return;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", velocity.y);
    }

    // Vẽ groundCheck trong Scene view để dễ debug
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

