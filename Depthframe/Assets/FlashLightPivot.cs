using UnityEngine;

public class Flashlight2DAim : MonoBehaviour
{
    public Transform flashlightPivot;

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - flashlightPivot.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        flashlightPivot.rotation = Quaternion.Euler(0, 0, angle);
    }
}

