using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
	// Holds player data between scenes

	public static int Level = 0; // Initialise level to 0 when starting

	public static bool LevelSuccess = false; // Whether the level just completed was successful

	public static bool OnMainMenu = true; // Whether the player is on the main menu

	public static bool GameFinished = false; // Whether the player has finished the game
}
