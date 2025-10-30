using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSpin : MonoBehaviour
{
    public Vector3 spin;
    public float spinRate = 1.0f;

    private void Update()
    {
        this.transform.Rotate(spin * spinRate * Time.deltaTime);
    }
}
