using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentController : MonoBehaviour
{
    private AfterPeopleManager afterPeopleManager;

    private void Start()
    {
        afterPeopleManager = GameObject.Find("Player").GetComponent<AfterPeopleManager>();
    }
    private void Update()
    {
        if(this.gameObject.transform.childCount <=0)
            afterPeopleManager.DestroyParent();
    }
}
