using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -10f;
    public float jumpHeight = 2f;   
    public bool canJump = false;
    public bool creativeMode = false;
    public float sprintMultiplier = 2f;
    private GameObject playerHand;
    public GameObject playerHandPrefab;
    public Vector3 handOffset = new Vector3(0f, -10f, 0f);
    private CharacterController controller;
    private Vector3 velocity;

    public GameObject UI;
    public bool isRecording = false;
    private GameObject uiInstance;
    private float timer = 0.8f;
    private bool isCountingDown = false;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // WASD
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Di chuyển theo hướng Player đang nhìn (camera-relative)
        Vector3 move = transform.right * x + transform.forward * z;

        // Shift to sprint (applies to both creative and normal mode movement)
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = speed * sprintMultiplier;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (creativeMode)
        {
            // Disable gravity in creative mode
            velocity.y = 0f;

            // Space to fly up
            if (Input.GetButton("Jump"))
            {
                velocity.y = currentSpeed;
            }

            // Ctrl to fly down
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                velocity.y = -currentSpeed;
            }

            controller.Move(velocity * Time.deltaTime);
        } else
        {
            // Gravity
            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // Jump (before gravity so it takes effect this frame)
            if (Input.GetButtonDown("Jump") && controller.isGrounded && canJump)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            if (!isRecording && !isCountingDown)
            {
                // Bắt đầu đếm ngược
                isCountingDown = true;
                timer = 0.8f;
                if (playerHand != null) Destroy(playerHand);
                playerHand = Instantiate(playerHandPrefab);
                GameObject cam = GameObject.Find("Camera");
                if (cam != null)
                {
                    playerHand.transform.SetParent(cam.transform, false);
                    playerHand.transform.localPosition = handOffset;
                }
            }
            else if (isRecording)
            {
                // Dừng recording
                if (uiInstance != null) Destroy(uiInstance);
                isRecording = false;
            }
        }

        // Đếm ngược liên tục mỗi frame
        if (isCountingDown && timer > 0)
        {
            timer -= Time.deltaTime;
            Debug.Log("Timer: " + timer);
            if (timer <= 0)
            {
                isCountingDown = false;
                isRecording = true;
                uiInstance = Instantiate(UI);
                if (playerHand != null) Destroy(playerHand);
            }
        }

    }
}
