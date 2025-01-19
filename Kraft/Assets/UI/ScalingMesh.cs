using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingMesh : MonoBehaviour
{
    [SerializeField] private float scaleAmount = 0.25f;
    [SerializeField] private float scaleSpeed = 2f;
    [SerializeField] private float defaultScaleX, defaultScaleY, defaultScaleZ;
    private float time = 0f;

    private void Start()
    {
        defaultScaleX = transform.localScale.x;
        defaultScaleY = transform.localScale.y;
        defaultScaleZ = transform.localScale.z;
    }

    void Update()
    {
        time += Time.deltaTime * scaleSpeed;

        float _scaleMod = Mathf.Sin(time) * scaleAmount;

        this.transform.localScale = new Vector3( defaultScaleX + _scaleMod * defaultScaleX,
                                                 defaultScaleY + _scaleMod * defaultScaleY, 
                                                 defaultScaleZ + _scaleMod * defaultScaleZ);
    }
}
