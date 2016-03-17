using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HouseBehaviour : BasicObstacleBehaviour
{	
	private bool HasDelivered = false;
    public AudioSource audioPoint;

    public bool NoDelivery = true;

	public Sprite[] GoodHouseSprites;
    public Sprite[] GoodSpriteLight;
	public Sprite[] BadHouseSprites;
    public Sprite[] BadSpriteLight;

    private bool IsGoodHouse = true;

	public GameObject[] Trees;

	public bool IsRight;

	private float MinX = -1F, MinY = -3F, MaxX = 1F, MaxY = -0.3F;

	private RotateSide Side;

	List<GameObject> SpawnedTrees;

	private GameObject TreeContainer;

    private int randNewSprite;

    //Kiest nummer voor welke sprite het kiest
    void Start()
    {
        audioPoint = GameObject.Find("Main Camera").GetComponent<AudioSource>();        
    }
    

    public void SetRight(bool IsRight)
	{
		this.IsRight = IsRight;
	}

	private void SpawnTrees(float Count)
	{
		for(int i = 0; i < Count; ++i)
		{
			GameObject NewTree = (GameObject)Instantiate(Trees[Random.Range(0, Trees.Length)], transform.position, Quaternion.identity);

			Vector3 RandomLocation = Vector3.zero;
			RandomLocation.x = Random.Range(-0.9F, 0.6F);
			RandomLocation.y = Random.Range(0, 2) == 0 ? Random.Range(-3F, -3.5F) : Random.Range(0.5F, 3.5F);			
			
			NewTree.transform.localPosition = TreeContainer.transform.position + RandomLocation;
			NewTree.transform.parent = TreeContainer.transform;

			SpawnedTrees.Add(NewTree);
		}
	}

	public void Deliver()
	{
		if(HasDelivered)
			return;
        
        if (IsGoodHouse)
		{
            Global.Instance.Dollars++;
            //Gives ScoreMultiplier 0.01 each time this is played
            //If hitted a GoodHouse
			Global.Instance.ComboMultiplier += 0.01F;
            GetComponent<Animator>().SetTrigger("Deliver");
            transform.FindChild("Sprite").GetComponent<SpriteRenderer>().sprite = GoodSpriteLight[randNewSprite];
            transform.FindChild("Particle System").GetComponent<ParticleSystem>().Play();

            AudioClip sound = audioPoint.GetComponent<AudioManager>().sound1();
            audioPoint.PlayOneShot(sound);



        }
		else
		{
            //If hitted bad house
            //Sets ScoreMultiplier to 1x
			Global.Instance.ComboMultiplier = 1F;
            transform.FindChild("Destroy").GetComponent<ParticleSystem>().Play();
            transform.FindChild("Sprite").GetComponent<SpriteRenderer>().sprite = BadSpriteLight[randNewSprite];
           
            GameObject Dog = (GameObject)Instantiate (Resources.Load("Obstacles/DogObstacle"), transform.position, Quaternion.identity);
			Dog.GetComponent<DogObstacle>().IsRunningLeft = IsRight;
            AudioClip sound = audioPoint.GetComponent<AudioManager>().sound2();
            audioPoint.PlayOneShot(sound);
        }

		

		HasDelivered = true;
	}

	void OnEnable()
	{
		SpawnedTrees = new List<GameObject>();

		TreeContainer = new GameObject("TreeContainer");
		TreeContainer.AddComponent<BasicObstacleBehaviour>();

		TreeContainer.transform.position = transform.position;
		TreeContainer.transform.localScale = new Vector3(1, 1, 1);

		IsGoodHouse = Random.Range(0, 2) == 0? true : false;


        /*rand aangemaakt om zo te weten welke is gekozen
        zodat ik later dezelfde sprite maar dan met licht aan eraan kan koppelen.
        */
        int rand = Random.Range(0, GoodHouseSprites.Length);
        randNewSprite = rand;
        if (IsGoodHouse)
		{

            transform.FindChild("Sprite").GetComponent<SpriteRenderer>().sprite = GoodHouseSprites[rand];            
            transform.FindChild("Sprite").GetComponent<SpriteRenderer>().color = Color.white;
            

        }
		else
		{
			transform.FindChild("Sprite").GetComponent<SpriteRenderer>().sprite = BadHouseSprites[rand];
			transform.FindChild("Sprite").GetComponent<SpriteRenderer>().color = new Color(0.47F, 0.47F, 0.47F);
		}


        //
        //Added Joey Koedijk
        Vector2 SpriteSize = transform.FindChild("Sprite").GetComponent<SpriteRenderer>().sprite.bounds.size;
        gameObject.GetComponent<BoxCollider2D>().size = SpriteSize / 1.2f;
        gameObject.GetComponent<BoxCollider2D>().offset = new Vector2((SpriteSize.x-1.6f), 0.1f);
        //
        int RandomTreeCount = Random.Range(2, 6);
		SpawnTrees(RandomTreeCount);

		if(IsRight)
		{
			transform.localScale = new Vector3(3, 3, 1);
		}
		else
		{
			transform.localScale = new Vector3(-3, 3, 1);
		}

		if(Global.Instance.IsDisco) 
		{
        	SideToRotate = Random.Range(0, 2) == 0 ? RotateSide.Left : RotateSide.Right;
			StartCoroutine(WaitForColorChange());
		}
	}	

	void OnDisable()
	{
		if(IsGoodHouse && !HasDelivered)
			Global.Instance.ComboMultiplier = 1;

		HasDelivered = false;
		NoDelivery = true;

		Destroy(TreeContainer);

		StopAllCoroutines();
	}
}
