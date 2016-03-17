using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DiscoSetting : MonoBehaviour
{
	private static bool spawned = false;

	public AudioClip DiscoClip;

	public bool IsDisco = false; 

	void Awake()
	{
		if(DiscoClip == null)
			DiscoClip = (AudioClip)Resources.Load("Disco");

		if(spawned == false)
		{
			spawned = true;
			
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			DestroyImmediate(gameObject);
		}
	}

	void Start()
	{
        
        if (SceneManager.GetActiveScene().name == "PlayScene")
		{
			Global.Instance.IsDisco = IsDisco;
			Camera.main.gameObject.GetComponent<AudioSource>().clip = DiscoClip;
		}
    }
}
