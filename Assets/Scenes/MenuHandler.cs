using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
	public GameObject successScreen;
	public GameObject failScreen;
	public Text failMessage;
	public GameObject introScreen;
	public GameObject gameFinishScreen;

	/// <summary>
	/// Starts the game at the level in PlayerData.Level
	/// </summary>
	public void StartGame()
	{
		PlayerData.OnMainMenu = false;
		SceneManager.LoadScene("MainGame");
	}

	private void Start()
	{
		// Show the correct menu
		// They are all disabled by default
		if (PlayerData.OnMainMenu)
		{
			introScreen.SetActive(true);
		}
		else
		{
			if (PlayerData.GameFinished)
			{
				gameFinishScreen.SetActive(true);
			}
			else
			{
				if (PlayerData.LevelSuccess)
				{
					successScreen.SetActive(true);
				}
				else
				{
					failScreen.SetActive(true);
					failMessage.text = "" + PlayerData.Level;
				}
			}
		}
	}
}
