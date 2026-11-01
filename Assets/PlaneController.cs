using UnityEngine;
using UnityEngine.InputSystem; // 引用新版輸入系統命名空間

public class PlaneController : MonoBehaviour
{
    [Header("飛行參數")]
    public float forwardSpeed = 15.0f;
    public float turnSpeed = 50.0f;

    [Header("音效設定")]
    public AudioSource engineAudioSource;
    public AudioSource sfxAudioSource;
    public AudioClip crashSound;
    public AudioClip passSound;

    private bool isCrashed = false;
    private Vector2 inputVector = Vector2.zero;

    void Update()
    {
        if (isCrashed) return;

        // 讀取輸入訊號
        ReadInput();

        // 1. 持續向前飛行
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // 2. 轉向控制（X 軸控制左右偏航，Y 軸控制上下俯仰）
        float horizontal = inputVector.x;
        float vertical = inputVector.y;

        transform.Rotate(Vector3.up, horizontal * turnSpeed * Time.deltaTime);
        transform.Rotate(Vector3.right, -vertical * turnSpeed * Time.deltaTime);
    }

    private void ReadInput()
    {
        inputVector = Vector2.zero;

        // 讀取行動端虛擬搖桿（On-Screen Stick 模擬的 Gamepad 訊號）
        if (Gamepad.current != null)
        {
            inputVector = Gamepad.current.leftStick.ReadValue();
        }

        // 若無搖桿輸入，則讀取電腦鍵盤輸入（方便在 Unity 編輯器內直接按 WASD 測試）
        if (inputVector == Vector2.zero && Keyboard.current != null)
        {
            float kHorizontal = 0f;
            float kVertical = 0f;

            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) kHorizontal -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) kHorizontal += 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) kVertical += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) kVertical -= 1f;

            inputVector = new Vector2(kHorizontal, kVertical);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && !isCrashed)
        {
            isCrashed = true;
            if (engineAudioSource != null) engineAudioSource.Stop();
            if (sfxAudioSource != null && crashSound != null)
            {
                sfxAudioSource.PlayOneShot(crashSound);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PassZone") && !isCrashed)
        {
            if (sfxAudioSource != null && passSound != null)
            {
                sfxAudioSource.PlayOneShot(passSound);
            }
            other.enabled = false;
        }
    }
}