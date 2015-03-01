using UnityEngine;
using System.Collections;

public class DemoCameraController : MonoBehaviour {

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.Rotate(45f);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            this.Rotate(-45f);
        }
        
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.Zoom(1f);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            this.Zoom(-1f);
        }
    }

    private void Rotate(float amount)
    {
        Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, amount * Time.deltaTime);
    }
    
    private void Zoom(float amount)
    {
        Camera.main.transform.Translate(Vector3.forward * amount, Space.Self);
    }
}
