using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Referensi ke Player
    public Vector3 offset; // Offset pada sumbu X dan Z
    public float verticalThresholdTop = 1f; // Batas atas vertikal antara kamera dan karakter
    public float verticalThresholdBottom = 0.5f; // Batas bawah vertikal antara kamera dan karakter

    private void LateUpdate()
    {
        if (player != null)
        {
            // Posisi kamera di sumbu horizontal selalu mengikuti Player
            float targetX = player.position.x + offset.x;

            // Posisi kamera di sumbu vertikal
            float currentY = transform.position.y;
            float playerY = player.position.y;

            // Hanya ubah posisi kamera di Y jika Player berada di luar threshold vertikal
            if (playerY > currentY + verticalThresholdTop)
            {
                currentY = playerY - verticalThresholdTop; // Geser kamera ke bawah
            }
            else if (playerY < currentY - verticalThresholdBottom)
            {
                currentY = playerY + verticalThresholdBottom; // Geser kamera ke atas
            }

            // Set posisi kamera dengan offset Z
            transform.position = new Vector3(targetX, currentY, offset.z);
        }
    }
}