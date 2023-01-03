using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Sprite2D : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Transform LookAt;

    // Update is called once per frame
    void Update()
    {
        if (LookAt)
        {
            transform.rotation = Quaternion.Euler(LookAt.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }
}
