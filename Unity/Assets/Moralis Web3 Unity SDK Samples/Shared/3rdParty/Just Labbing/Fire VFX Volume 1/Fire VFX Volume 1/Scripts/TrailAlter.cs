using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailAlter : MonoBehaviour
{
    public TrailRenderer myTrail;
    public Material myMat;
    [ColorUsage(true, true)]
    public Color trailColor;
    // Start is called before the first frame update
    void Start()
    {
        myTrail = GetComponent<TrailRenderer>();
        myMat = myTrail.materials[0];
        myMat.SetColor("_BaseColor", trailColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
