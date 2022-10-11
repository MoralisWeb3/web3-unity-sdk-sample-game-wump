using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public Animator characterAnimator;
    public HandInfo leftHand, rightHand;
    [System.Serializable]
    public class HandInfo
    {
        public GameObject hand;
        public string animationName;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CastProjectile(HandInfo handUsed)
    {
        characterAnimator.Play(handUsed.animationName, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
