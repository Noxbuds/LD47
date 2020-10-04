using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
	public GameObject successScreen;
	public GameObject failScreen;
	public GameObject intoScreen;
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
			intoScreen.SetActive(true);
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
				}
			}
		}
	}
}
