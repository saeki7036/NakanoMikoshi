using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Washoi_script : MonoBehaviour
{
    private float Input_washoi, old_washoi;
    public float Input_washoi_once;
    [SerializeField] private GameObject Washoi_hani;
    [SerializeField] private float Washoi_Duration = 0.2f;
    [SerializeField] float wasyoi_radius = 5.0f;

    [SerializeField] private AudioSource PlayerSEAudio;
    [SerializeField] private AudioClip[] WasshoiSounds;
    [SerializeField] private AudioClip PeopleRecoverySound;
   
    [SerializeField] private GameStateController gamestateController;
    [SerializeField] private MikoshiCollisionDetection MikoshiCollision;
    [SerializeField] private AfterPeopleManager afterPeopleManager;
    [SerializeField] private stamina stamina_script;

    // Start is called before the first frame update
    void Start()
    {      
        Washoi_hani.transform.localScale =  new Vector3(wasyoi_radius,0.01f,wasyoi_radius);
    }

    // Update is called once per frame
    void Update()
    {
        Input_washoi = Input.GetAxis("Fire1");
        Input_washoi_once = old_washoi - Input_washoi;

        old_washoi = Input_washoi;
        string state = gamestateController.GetState().ToString();
      
        if (Input_washoi_once == -1 && stamina_script.stamina_rest > 0&&state == nameof(MainGameState)||
            Input_washoi_once == -1 &&  state == nameof(FeverTimeState))
        {
            for (int i = 0; i < 2; i++)
            {
                PlayerSEAudio.PlayOneShot(WasshoiSounds[i]);
            }

            Destroy(Instantiate(Washoi_hani, this.transform, false), Washoi_Duration);

            stamina_script.slider_clone[stamina_script.stamina_number_now - 1].value = 1 - stamina_script.slide_value;
            stamina_script.stamina_rest--;

            if (stamina_script.stamina_number_now != 1) { stamina_script.stamina_number_now--; }

            Check_People();
        }
    }

    private void FixedUpdate()
    {
        

        
    }

    void Check_People()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, wasyoi_radius,Vector3.one);

        foreach (var hit in hits)
        {
            if (hit.collider.tag == "People")
            {
                PlayerSEAudio.PlayOneShot(PeopleRecoverySound);
                Debug.Log("People Touch");
                afterPeopleManager.AssignMikoshiPeople(hit.transform.position);

                Destroy(hit.collider.gameObject);            
            }
        }
    }
}



