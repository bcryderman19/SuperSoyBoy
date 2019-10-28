using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLerpToTransform : MonoBehaviour
{
    //1 specify which target to track , tracking spped and camera bounds
    public Transform camTarget;

    public float trackingSpeed;
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;

    //2 best method when dealing with ridgidbody, result it camera will track player which will have rigidbody2D
    private void FixedUpdate()
    {
        //3 null check ensures that a valid transform component was assigned in the camTarget field on the script in editor
        if (camTarget != null)
        {
            //4 performs linear interpolation between two vectors by the third parameters value
            var newPos = Vector2.Lerp(transform.position, camTarget.position, Time.deltaTime * trackingSpeed);
            var camPosition = new Vector3(newPos.x, newPos.y, -10f);
            var v3 = camPosition;
            var clampX = Mathf.Clamp(v3.x, minX, maxX);
            var clampY = Mathf.Clamp(v3.y, minY, maxY);
            transform.position = new Vector3(clampX, clampY, -10f);
        }
    }


}
