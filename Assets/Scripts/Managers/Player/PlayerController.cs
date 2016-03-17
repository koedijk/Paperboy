﻿using UnityEngine;
using System.Collections;

public class PlayerController : CacheMB
{
    private float Speed = 5.0F;

    public bool IsDeath;

    private Animator Anim;

    public GameObject Running;

    private PickupType CurrentPickup = PickupType.None;

    private float GlobalInitSpeed;

    private float ColaTimeout = 0;

    public ParticleSystem ColaBottleParticles;

    public AudioSource audioPoint;

    private bool CantPickUp = false;

    void Start()
    {
        Anim = GetComponent<Animator>();
        //Joey {
        Running.SetActive(true);
        // }
    }
    // New Movement -- Joey Koedijk {
    void Movement()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
        transform.position += move * Speed * Time.deltaTime;
        //Lichte rotatie van Player bij Movement
        transform.rotation = Quaternion.Euler(0, 0, -GetComponent<Rigidbody2D>().velocity.x * 2);
    }//    }

    void Update()
    {
       
        if (Global.Instance.IsPlaying)
        {
            if (Anim != null)
                Anim.enabled = true;       
            
            if (ColaBottleParticles != null && CurrentPickup == PickupType.ColaBottle)
            {
                ColaBottleParticles.GetComponent<Renderer>().material.mainTexture = transform.FindChild("Sprite").GetComponent<Renderer>().material.mainTexture;
                ColaBottleParticles.enableEmission = true;                
            }
            else if (ColaBottleParticles != null)
            {
                ColaBottleParticles.enableEmission = false;
            }

                

            if (CurrentPickup == PickupType.ColaBottle)
            {
                StartColaPickup();
                CantPickUp = true;
                ColaTimeout += Time.deltaTime;
                if (ColaTimeout >= 3)
                {
                    CantPickUp = false;                    
                }
            }

            if (CantPickUp == false)
            {
                GetComponent<BoxCollider2D>().enabled = true;

                Anim.SetBool("Cycling", false);

                CurrentPickup = PickupType.None;
                //Joey
                Running.SetActive(true);
            }

            #region Movement
            

#if UNITY_STANDALONE || UNITY_EDITOR


            GetComponent<Rigidbody2D>().velocity = new Vector2(Input.GetAxis("Horizontal") * Speed, 0);
            //Add -- Joey Koedijk
            Movement();

            //GameSpeed
            // Debug.Log(Global.Instance.Speed);
            if (Input.GetKeyDown(KeyCode.UpArrow))
                Global.Instance.Speed++;
            if (Input.GetKeyDown(KeyCode.DownArrow))
                Global.Instance.Speed--;


#elif UNITY_ANDROID || UNITY_IOS
			
			Speed = 20F;		
			rigidbody2D.velocity = new Vector2(Input.acceleration.x * Speed, 0);
			
#endif

            

            #endregion

            #region Touch/Click Input Check

#if UNITY_STANDALONE || UNITY_EDITOR

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 WorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 MousePos = new Vector2(WorldPosition.x, WorldPosition.y);

                Collider2D Coll = Physics2D.OverlapPoint(MousePos);
                if (Coll != null)
                {
                    if (Coll.tag == "House")
                    {
                        HouseBehaviour House = Coll.GetComponent<HouseBehaviour>();
                        if (House != null)
                        {
                            if (House.NoDelivery)
                            {
                                if (CurrentPickup != PickupType.GoldenPaper)
                                {
                                    GameObject Paper = (GameObject)Instantiate(Resources.Load("Player/Paper"), transform.position, Quaternion.identity);
                                    Paper.GetComponent<PaperController>().StartPaper(Coll.gameObject, false);
                                }
                                else
                                {
                                    GameObject Paper = (GameObject)Instantiate(Resources.Load("Player/GoldenPaper"), transform.position, Quaternion.identity);
                                    Paper.GetComponent<PaperController>().StartPaper(Coll.gameObject, false);

                                    Global.Instance.Dollars += 4;
                                    ResetPickup();
                                }
                                House.NoDelivery = false;
                            }
                            House.Deliver();
                        }
                    }
                    else if (CurrentPickup == PickupType.SteelPaper && Coll.tag == "Obstacle")
                    {
                        GameObject SteelPaper = (GameObject)Instantiate(Resources.Load("Player/SteelPaper"), transform.position, Quaternion.identity);
                        GameObject DestroyParticle = (GameObject)Instantiate(Resources.Load("Obstacles/DestroyParticle"), Coll.transform.position, Quaternion.identity);

                        Destroy(DestroyParticle, 0.5F);

                        SteelPaper.GetComponent<PaperController>().StartPaper(Coll.gameObject, true);

                        ResetPickup();
                    }
                }
            }

#elif UNITY_ANDROID || UNITY_IOS
			
			if(Input.touchCount == 1)
			{
				Vector3 WorldPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
				Vector2 TouchPosition = new Vector2(WorldPosition.x, WorldPosition.y);
				
				Collider2D Coll = Physics2D.OverlapPoint(TouchPosition);
				if(Coll != null)
				{
					if(Coll.tag == "House")
					{
						HouseBehaviour House = Coll.GetComponent<HouseBehaviour>();
						if(House != null)
						{				
							if(House.NoDelivery)
							{
								if(CurrentPickup != PickupType.GoldenPaper)
								{
									GameObject Paper = (GameObject)Instantiate(Resources.Load("Player/Paper"), transform.position, Quaternion.identity);
									Paper.GetComponent<PaperController>().StartPaper(Coll.gameObject, false);
								}
								else
								{
									GameObject Paper = (GameObject)Instantiate(Resources.Load("Player/GoldenPaper"), transform.position, Quaternion.identity);
									Paper.GetComponent<PaperController>().StartPaper(Coll.gameObject, false);
									
									Global.Instance.Dollars += 4;
									ResetPickup();
								}
								House.NoDelivery = false;
							}
							House.Deliver();
						}
					}
					else if(CurrentPickup == PickupType.SteelPaper && Coll.tag == "Obstacle")
					{
						GameObject SteelPaper = (GameObject)Instantiate(Resources.Load("Player/SteelPaper"), transform.position, Quaternion.identity);

						GameObject DestroyParticle = (GameObject)Instantiate(Resources.Load("Obstacles/DestroyParticle"), Coll.transform.position, Quaternion.identity);
						
						Destroy (DestroyParticle, 0.5F);

						SteelPaper.GetComponent<PaperController>().StartPaper(Coll.gameObject, true);
						
						ResetPickup();
					}
				}
			}
			
#endif

            #endregion
        }
        else
        {
            if (Anim != null)
                Anim.enabled = false;
            transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    public void SetPickup(PickupType Type)
    {
        if (CurrentPickup == PickupType.None) // If the player does not have a pickup
        {
            //Joey
            Running.SetActive(true);
            CurrentPickup = Type; // Then set the pickup 

            switch (Type) // And check which pickup it is, to set it's properties to the player
            {
                case PickupType.Bike:
                    Anim.SetBool("Cycling", true);
                    //Joey
                    Running.SetActive(false);
                    CantPickUp = true;
                    break;

                case PickupType.ColaBottle:
                    GlobalInitSpeed = Global.Instance.Speed;
                    break;

                default: break;
            }
        }
    }
    

    private void ResetPickup()
    {
        
    }

    void StartColaPickup()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        Global.Instance.Speed = GlobalInitSpeed * 2;
    }

    void OnTriggerEnter2D(Collider2D Coll)
    {
        if (Global.Instance.IsPlaying && Coll.tag == "Obstacle")
        {
            if (CurrentPickup != PickupType.Bike)
            {
                Global.Instance.PlayerDead();
                // Joey Toegevoegd {
                Running.SetActive(false);
                AudioClip Die = audioPoint.GetComponent<AudioManager>().sound3();
                audioPoint.PlayOneShot(Die);
               // }
                IsDeath = true;
            }
            else
            {
                Spawner.Destroy(Coll.gameObject);
                CantPickUp = false;
            }
        }
    }
}
