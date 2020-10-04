using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgrammingTask : TaskBase
{
	// For the programming task, we need to write some code.
	// In order to do so, you need to press C/V a certain number of times
	// in a specific order. This will put code snippets on the screen.
	// When the cat interrupts, it will erase some of your code

	// The 'next key' sprite
	public Image nextKeyImage;
	public Text nextKeyText;

	// The key order and the correct and wrong code snippets that show for each
	private int[] keyOrder;
	private int cursor; // cursor vertical position

	// The backspace key
	public KeyCode backspace;

	// All the code snippet prefabs available - 0 should be opening line, 1 curly brackets, the rest will be random snippets
	[Header("Code")]
	public GameObject[] codeSnippets;

	// Correct and wrong code snippet IDs to show
	private int[] correctSnippets;
	private int[] wrongSnippets;

	// The root object for code snippets
	public Transform codeRoot;

	// All the code snippets that have been entered
	private GameObject[] typedSnippets;

	[Header("UI")]
	// The colors to set the code to when correct/incorrect
	public Color correctColor;
	public Color incorrectColor;

	// The cursor sprite
	public GameObject cursorSprite;
	private float cursorTimer;

	// Code snippet positioning
	private static Vector2 codeStartPos = new Vector2(64, -2);
	private static Vector2 codePosStep = new Vector2(0, -4); // The step to take for each new line of code
	private static Vector2 keyIconStartPos = new Vector2(48, -2); // Where to start placing the key icons from

	/// <summary>
	/// Creates a random list of keys to press
	/// </summary>
	/// <param name="length">The length of the key list</param>
	/// <returns>The keys to press (IDs for the keys array)</returns>
	private int[] CreateKeyOrder(int length)
	{
		int[] newKeys = new int[length];

		for (int i = 0; i < length; i++)
			newKeys[i] = Random.Range(0, keys.Count);

		return newKeys;
	}

	/// <summary>
	/// Generates a list of code snippets to place
	/// </summary>
	/// <param name="length">The length of the code</param>
	/// <returns>The list of code snippet IDs to show</returns>
	public static int[] GetSnippetOrder(int length, int numCodeSnippets)
	{
		// We need to start with opening line, followed by the code body, followed by another curly bracket.
		// The length for the code body will be length - 2
		if (length < 3)
			return GetRandomSnippets(length, numCodeSnippets); //TODO: figure out something more elegant later

		// Create the snippet order and place in the 3 fixed snippets
		int[] snippets = new int[length];
		snippets[0] = 0;
		snippets[1] = 1;
		snippets[length - 1] = 1;

		// Place the code body in
		for (int i = 2; i < length - 1; i++)
			snippets[i] = Random.Range(2, numCodeSnippets);

		return snippets;
	}

	/// <summary>
	/// Generates a random list of code snippets to be used as the wrong code snippets
	/// </summary>
	/// <param name="length">The length of the code</param>
	/// <returns>A list of random code snippets</returns>
	public static int[] GetRandomSnippets(int length, int numCodeSnippets)
	{
		int[] snippets = new int[length];

		for (int i = 0; i < length; i++)
			snippets[i] = Random.Range(0, numCodeSnippets);

		return snippets;
	}

	/// <summary>
	/// Types a code snippet
	/// </summary>
	/// <param name="key">The key press in the keys array</param>
	private void TypeKey(int key)
	{
		if (keyOrder != null)
		{
			if (cursor < keyOrder.Length)
			{
				// Pick random sound
				int id = Random.Range(0, keySounds.Length);

				if (id < keySounds.Length)
					keySounds[id].Play();

				// Fetch the correct and incorrect code snippets here
				GameObject correct = codeSnippets[correctSnippets[cursor]];
				GameObject incorrect = codeSnippets[wrongSnippets[cursor]];

				// Place code snippet at cursor and set its color
				if (key == keyOrder[cursor])
				{
					typedSnippets[cursor] = Instantiate(correct);
					typedSnippets[cursor].GetComponent<Image>().color = correctColor;
				}
				else
				{
					typedSnippets[cursor] = Instantiate(incorrect);
					typedSnippets[cursor].GetComponent<Image>().color = incorrectColor;
				}

				// Parent it to the code root
				typedSnippets[cursor].transform.SetParent(codeRoot);

				// Reset the scale
				typedSnippets[cursor].transform.localScale = Vector3.one;

				// Position the snippet correctly
				typedSnippets[cursor].transform.localPosition = new Vector2(codeStartPos.x + codePosStep.x * cursor, codeStartPos.y + codePosStep.y * cursor);

				// Give it its name
				typedSnippets[cursor].gameObject.name = cursor.ToString() + ":" + key.ToString();

				// Move cursor
				cursor++;
			}
		}
	}

	/// <summary>
	/// Deletes the previous line, if there are any
	/// </summary>
	private void DeletePreviousLine()
	{
		if (cursor > 0 && typedSnippets != null)
		{
			// Move cursor backwards
			cursor--;

			// Delete the line
			Destroy(typedSnippets[cursor]);
		}
	}

	public override void Begin()
	{
		base.Begin();

		// Pick the length and set the progress
		int length = Random.Range(5, 9);
		progress = 0;
		MaxProgress = length;

		// Remove code snippets
		for (int i = 0; i < codeRoot.childCount; i++)
			if (codeRoot.GetChild(i).name != "Key Image" && codeRoot.GetChild(i).name != "Cursor")
				Destroy(codeRoot.GetChild(i).gameObject);

		// Reset typing
		cursor = 0;
		typedSnippets = new GameObject[length];

		// Create a list of key presses, correct snippets and incorrect snippets
		keyOrder = CreateKeyOrder(length);
		correctSnippets = GetSnippetOrder(length, codeSnippets.Length);
		wrongSnippets = GetRandomSnippets(length, codeSnippets.Length);
	}

	public override int End()
	{
		performanceRating = 10;
		return base.End();
	}

	public override void InterruptTask()
	{
		base.InterruptTask();

		// Remove characters each time
		DeletePreviousLine();
	}

	/// <summary>
	/// Shows the next key to press next to the current line
	/// </summary>
	private void ShowNextKey()
	{
		if (keyOrder != null)
		{
			// Fetch the next key
			if (cursor < keyOrder.Length)
			{
				nextKeyImage.gameObject.SetActive(true);

				// Fetch the correct sprite
				int keyId = keyOrder[cursor];
				
				// Set the sprite of the 'next key' image
				//nextKeyImage.sprite = sprite;
				nextKeyText.text = keys[keyId].ToString();

				// Place it correctly
				nextKeyImage.transform.localPosition = new Vector2(keyIconStartPos.x, keyIconStartPos.y + codePosStep.y * cursor);

				// Place cursor
				cursorSprite.transform.localPosition = new Vector2(2, codeStartPos.y + codePosStep.y * cursor);
			}
			else
				nextKeyImage.gameObject.SetActive(false);
		}
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check for any valid key presses
		for (int i = 0; i < keys.Count; i++)
		{
			if (Input.GetKeyUp(keys[i]))
				TypeKey(i);
		}
		if (Input.GetKeyUp(backspace))
			DeletePreviousLine();

		if (keyOrder != null)
		{
			// Flicker cursor
			cursorTimer -= Time.deltaTime;
			if (cursorTimer < 0)
			{
				cursorTimer = 0.5f;

				if (cursor < keyOrder.Length)
					cursorSprite.SetActive(!cursorSprite.activeSelf);
				else
					cursorSprite.SetActive(false);
			}

			// Set progress
			progress = 0;
			for (int i = 0; i < typedSnippets.Length; i++)
			{
				// Check the key object's name after the : to get its key ID, and check if that is correct
				if (typedSnippets[i] != null)
				{
					string sName = typedSnippets[i].name;

					if (sName.Substring(sName.IndexOf(':') + 1).Equals(keyOrder[i].ToString()))
						progress++;
				}
			}
		}

		ShowNextKey();
	}

	public override string CatInterruptMessage()
	{
		return "Cat pressed backspace";
	}

	public override string ToString()
	{
		return "Write Code";
	}
}
