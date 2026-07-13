using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public float distance = 5f;
    public float height = 2f;

    public float rotationSpeed = 120f;

    private float currentYaw = 0f;
    private bool isDragging = false;

    // Update is called once per frame
    void Update()
    {
        //Detectar click
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isDragging = true;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        if (isDragging)
        {
            float mouseX = Mouse.current.delta.ReadValue().x;

            currentYaw += mouseX * rotationSpeed * Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(0f, currentYaw, 0f);

        Vector3 offset = new Vector3(0f, height, -distance);

        //posicion de la camara
        transform.position = target.position + rotation * offset;

        //Rotacion hacia el player
        transform.rotation = Quaternion.LookRotation(target.position - transform.position);
    }
}