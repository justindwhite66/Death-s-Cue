using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    private void FaceMouse(){
      Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector2 direction = mousePosition - transform.position;
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    transform.rotation = Quaternion.Euler(0, 0, angle);

    // Flip vertically if aiming backwards
    if (angle > 90 || angle < -90)
        transform.localScale = new Vector3(1, -1, 1);
    else
        transform.localScale = new Vector3(1, 1, 1);

        

    }

    private void Update()
    {
     FaceMouse();   
    }
}
