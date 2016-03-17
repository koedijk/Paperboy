using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using UnityEngine.Events;

using Extensions;
using UnityEngine.SceneManagement;

public class ScoreMenuHandlers : MonoBehaviour 
{
	private string HighscoreURL = "http://www.basegames.nl/highscores.pl";

    public Slider AudioSlider;
    private bool Readonly = true;

    public Text EndScoreText;
	public Text DistanceScoreText;
	public Text PaperScoreText;

	public float StartDelay = 0.5F;
	public float TypeDelay = 0.01F;

	private Animator Anim;

	public bool IsVisible = false;
    //public bool OptionVis = false;

    void Start()
	{
		Anim = GetComponent<Animator>();
        GetAudioVolume();
	}

	void Update()
	{
		DistanceScoreText.text = "Score: " + Mathf.RoundToInt(Global.Instance.TotalScore);
		PaperScoreText.text = "Papers: " + Global.Instance.Dollars;
	}

	public void SubmitScore()
	{
		string Name = SystemInfo.deviceName;
		Debug.Log(Name + " Submited the score: " + Mathf.RoundToInt(Global.Instance.TotalScore) );
	}

	public void Home()
	{
        SceneManager.LoadScene(0);
	}
	public void Retry()
	{
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Options()
    {
        Anim.SetTrigger("StartOptionsFadeIn");        
    }
    public void HideOptions()
    {
        Anim.SetTrigger("StartOptionsFadeOut");
    }


	public void ShowScoreMenu()
	{
		IsVisible = true;

		Anim.SetTrigger("StartScoreFadeIn");
		
		int CurrentTotalScore = Mathf.RoundToInt(Global.Instance.TotalScore);

		StartCoroutine(this.TypeIn("Score: " + Mathf.RoundToInt(Global.Instance.TotalScore), StartDelay, TypeDelay));
        //Joey Koedijk Top 3 HighScore
        int Highscore1 = PlayerPrefs.GetInt("Highscore", 0);
        int Highscore2 = PlayerPrefs.GetInt("Highscore1", 0);
        int Highscore3 = PlayerPrefs.GetInt("Highscore2", 0);
        if (CurrentTotalScore > PlayerPrefs.GetInt("Highscore", 0))
        {
            PlayerPrefs.SetInt("Highscore", CurrentTotalScore);
            PlayerPrefs.SetInt("Highscore1", Highscore1);
            PlayerPrefs.SetInt("Highscore2", Highscore2);
            PlayerPrefs.SetInt("Highscore3", Highscore2);
            return;
        }
        else if (CurrentTotalScore <= PlayerPrefs.GetInt("Highscore", 0) && CurrentTotalScore >= PlayerPrefs.GetInt("Highscore1", 0))
        {
            PlayerPrefs.SetInt("Highscore1", CurrentTotalScore);
            PlayerPrefs.SetInt("Highscore2", Highscore2);
            return;
        }
        else if (CurrentTotalScore <= PlayerPrefs.GetInt("Highscore1", 0) && CurrentTotalScore >= PlayerPrefs.GetInt("Highscore2", 0))
        {
            PlayerPrefs.SetInt("Highscore2", CurrentTotalScore);
            
        }
        //

    }
	public void ShowPauseMenu()
	{
		IsVisible = true;
        GameObject.Find("Running").SetActive(false);

        Anim.SetTrigger("StartPauseFadeIn");
	}
	public void HidePauseMenu()
	{
		IsVisible = false;
		
		Anim.SetTrigger("StartPauseFadeOut");
		
		Global.Instance.IsPlaying = true;
	}
    public void GetAudioVolume()
    {
        AudioSlider.value = PlayerPrefs.GetFloat("AudioVolume");
        Readonly = false;
    }
    public void SetAudioVolume()
    {
        if (Readonly)
            return;
        PlayerPrefs.SetFloat("AudioVolume", AudioSlider.value);
    }


}
