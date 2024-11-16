using System.Collections;
using UnityEngine;

public class ParentMove : MonoBehaviour
{
    private float call, firstZPos;
    private Rigidbody my_Rigidbody;
    private Vector3 velocty;
    const float jumpVec = 10f;   
    
    private stamina stamina_Script;

    void Start()
    {
        call = (gameObject.transform.parent.childCount - 1) / 4;
        my_Rigidbody = GetComponent<Rigidbody>();     
        velocty = new Vector3(0, 10f, 0);
        firstZPos = transform.localPosition.z;

        stamina_Script = GameObject.Find("Player").GetComponent<stamina>();
    }
    
    void Update()
    {
        float Input_Jump = Input.GetAxis("Jump");

        if (Input_Jump == 1&& transform.position.y <= 2 && stamina_Script.stamina_rest > 0)
        {
            Invoke(nameof(Jump), call);
        }

        Vector3 pos = transform.localPosition;
        pos.x = 0;
        pos.z = firstZPos;
        transform.localPosition = pos;
    }

    void Jump()
    {
        int childCount = transform.childCount;
        for(int i = 0; i < childCount; i++)
        {
            GameObject AfterPeople = transform.GetChild(i).gameObject;
            if(AfterPeople != null)
            {
                AfterPeopleAnimationController PeopleAnimation = 
                    AfterPeople.transform.GetChild(0).GetComponent<AfterPeopleAnimationController>();
                PeopleAnimation.Jump();
            }
        }

        velocty = my_Rigidbody.velocity;
        velocty.y = jumpVec;
        my_Rigidbody.velocity = velocty;
    }

    IEnumerator Sride(Vector3 Force)
    {
        yield return call*10;
        my_Rigidbody.AddForce(Force, ForceMode.Acceleration);
    }
}
