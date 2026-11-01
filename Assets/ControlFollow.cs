using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("目標物體")]
    public Transform target;

    [Header("跟隨參數")]
    public Vector3 offset = new Vector3(0f, 3.5f, -8f);
    public float smoothSpeed = 5.0f;
    public float rotationSpeed = 5.0f;

    void LateUpdate()
    {
        if (target == null) return;

        // 計算相對於飛機朝向的目標世界座標
        Vector3 desiredPosition = target.TransformPoint(offset);

        // 平滑移動攝影機位置
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 平滑旋轉攝影機朝向飛機前方
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}