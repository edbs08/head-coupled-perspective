using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cube : MonoBehaviour
{
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 3.14f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Input.GetAxis("Mouse Y") * moveSpeed, -Input.GetAxis("Mouse X") *moveSpeed, 0f);
        transform.Translate(moveSpeed*Input.GetAxis("Horizontal")*Time.deltaTime, 0f, moveSpeed*Input.GetAxis("Vertical") * Time.deltaTime);
        transform.localScale += new Vector3(moveSpeed * Input.GetAxis("Mouse ScrollWheel"), moveSpeed * Input.GetAxis("Mouse ScrollWheel"), moveSpeed * Input.GetAxis("Mouse ScrollWheel"));
        

    }
}
