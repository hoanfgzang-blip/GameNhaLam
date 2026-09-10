using UnityEngine;

/// <summary>
/// Script xoay camera theo input chuột (mouse).
/// Không sử dụng Rigidbody.
/// 
/// Cách dùng:
/// - Gắn script này vào Camera (hoặc object cha chứa Camera).
/// - Kéo Transform của Player vào trường "playerBody" trong Inspector.
/// - Camera sẽ xoay lên/xuống, Player sẽ xoay trái/phải.
/// </summary>
public class MouseCameraController : MonoBehaviour
{
    [Header("Cài đặt độ nhạy")]
    [Tooltip("Độ nhạy chuột")]
    public float mouseSensitivity = 200f;

    [Header("Giới hạn góc nhìn")]
    [Tooltip("Góc nhìn lên tối đa (độ)")]
    public float topClamp = 80f;

    [Tooltip("Góc nhìn xuống tối đa (độ)")]
    public float bottomClamp = -80f;

    [Header("Tham chiếu")]
    [Tooltip("Transform của Player - camera sẽ xoay ngang theo Player")]
    public Transform playerBody;

    [Header("Tùy chọn")]
    [Tooltip("Ẩn và khóa con trỏ chuột khi chơi")]
    public bool lockCursor = true;

    [Tooltip("Đảo ngược trục Y (nhìn lên khi kéo chuột xuống)")]
    public bool invertY = false;

    // Góc xoay hiện tại
    private float xRotation = 0f; // lên/xuống
    private float yRotation = 0f; // trái/phải

    void Start()
    {
        // Tự động lấy parent (Capsule) làm playerBody nếu chưa gán
        if (playerBody == null && transform.parent != null)
        {
            playerBody = transform.parent;
        }

        // Khóa con trỏ chuột vào giữa màn hình
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Lấy góc xoay ban đầu để không bị nhảy về 0
        Vector3 camEuler = transform.localEulerAngles;
        xRotation = camEuler.x;
        if (xRotation > 180f) xRotation -= 360f;

        if (playerBody != null)
        {
            yRotation = playerBody.eulerAngles.y;
        }
    }

    void Update()
    {
        // Lấy input chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Đảo ngược trục Y nếu cần
        if (invertY)
        {
            mouseY = -mouseY;
        }

        // Cộng dồn góc xoay
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, bottomClamp, topClamp);

        // Áp dụng rotation
        if (playerBody != null)
        {
            // Camera xoay dọc, Player xoay ngang
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        else
        {
            // Tự xoay cả 2 trục
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }

    /// <summary>
    /// Bật/tắt khóa con trỏ chuột khi nhấn Escape.
    /// </summary>
    void OnApplicationFocus(bool hasFocus)
    {
        if (lockCursor && hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
