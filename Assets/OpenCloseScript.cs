using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseScript : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        anim.Play("boosterOpenAnim");
        anim.speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickUp()
    {
        anim.Play("boosterOpenAnim");
        anim.speed = 1;
        Debug.Log(this.gameObject.name + " picked up.");
    }

    public void PutDown()
    {
        anim.Play("boosterCloseAnim");
        Debug.Log(this.gameObject.name + " put down.");
    }
}
