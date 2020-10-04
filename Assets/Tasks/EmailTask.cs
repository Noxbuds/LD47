using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailTask : TaskBase
{
	// Snippets of tesk
	public GameObject[] textSnippets;

	// Number of snippets to place
	public int minSnippets;
	public int emailWidth;
	public int emailHeight;

	// Key order
	private int[] keyOrder;
	private GameObject[] typedKeys;

	// Root transform for text, as well as start position and offset for each index
	public Transform textRoot;
	public Vector2 textStartPos;
	public Vector2 textStepPos;

	// Cursor position
	private Vector2 cursor;
	private int cursorIndex
	{
		get
		{
			return (int)cursor.y * emailWidth + (int)cursor.x;
		}
	}
	public GameObject nextKeyImage;
	public Text nextKeyText;

	// Backspace key
	private KeyCode backspace = KeyCode.Backspace;

	// Correct/incorrect snippet color
	public Color correctColor;
	public Color incorrectColor;

	/// <summary>
	/// Gets a key order of a specific length
	/// </summary>
	/// <param name="length">The number of keys in the sequence</param>
	/// <returns>The key order</returns>
	private int[] GetKeyOrder(int length)
	{
		int[] order = new int[length];
		for (int i = 0; i < length; i++)
			order[i] = Random.Range(0, keys.Count);
		return order;
	}
	
	private void MoveCursor(int dir)
	{
		// Move cursor
		if (dir > 0)
		{
			if (cursor.x < emailWidth)
				cursor.x += 1;
			else
			{
				cursor.x = 0;
				cursor.y += 1;
			}
		}
		else
		{
			if (cursor.x > 0)
				cursor.x -= 1;
			else
			{
				cursor.x = emailWidth;
				cursor.y -= 1;
			}
		}
	}

	/// <summary>
	/// Types a specific key
	/// </summary>
	/// <param name="key"></param>
	private void TypeKey(int key)
	{
		if (keyOrder != null)
		{
			if (cursorIndex < keyOrder.Length)
			{
				// Pick random sound
				int id = Random.Range(0, keySounds.Length);

				if (id < keySounds.Length)
					keySounds[id].Play();

				// Fetch correct and incorrect snippets to place
				GameObject correct = textSnippets[keyOrder[cursorIndex]];
				GameObject incorrect = textSnippets[Random.Range(0, textSnippets.Length)];

				// Place code snippet
				if (key == keyOrder[cursorIndex])
				{
					typedKeys[cursorIndex] = Instantiate(correct);
					typedKeys[cursorIndex].GetComponent<Image>().color = correctColor;
				}
				else
				{
					typedKeys[cursorIndex] = Instantiate(incorrect);
					typedKeys[cursorIndex].GetComponent<Image>().color = incorrectColor;
				}

				// Parent it
				typedKeys[cursorIndex].transform.SetParent(textRoot);

				// Reset its scale to 1
				typedKeys[cursorIndex].transform.localScale = Vector3.one;

				// Position it correctly
				typedKeys[cursorIndex].transform.localPosition = new Vector2(textStartPos.x + textStepPos.x * cursor.x, textStartPos.y + textStepPos.y * cursor.y);

				// Give it a name
				typedKeys[cursorIndex].gameObject.name = cursorIndex + ":" + key.ToString();

				MoveCursor(1);
			}
		}
	}

	private void DeletePrevious()
	{
		if (cursorIndex > 0 && typedKeys != null)
		{
			// Move cursor backwards
			MoveCursor(-1);

			// Delete the text
			Destroy(typedKeys[cursorIndex]);
		}
	}

	public override void Begin()
	{
		base.Begin();

		// Pick the length
		int length = Random.Range(minSnippets, emailWidth * emailHeight);

		progress = 0;
		MaxProgress = length;

		// Remove previous code
		for (int i = 0; i < textRoot.childCount; i++)
			if (textRoot.GetChild(i).name != "Key Image" && textRoot.GetChild(i).name != "Opening Line" && textRoot.GetChild(i).name != "Ending Line")
				Destroy(textRoot.GetChild(i).gameObject);

		// Reset typing
		cursor = Vector2.zero;
		typedKeys = new GameObject[length];

		// Generate the correct key order
		keyOrder = GetKeyOrder(length);
	}

	/// <summary>
	/// Shows the next key to press next to the current line
	/// </summary>
	private void ShowNextKey()
	{
		if (keyOrder != null)
		{
			// Fetch the next key
			if (cursorIndex < keyOrder.Length)
			{
				nextKeyImage.gameObject.SetActive(true);

				// Fetch the correct sprite
				int keyId = keyOrder[cursorIndex];

				// Set the text of the 'next key' image
				//nextKeyImage.sprite = sprite;
				nextKeyText.text = keys[keyId].ToString();
			}
			else
				nextKeyImage.gameObject.SetActive(false);
		}
	}

	public override void InterruptTask()
	{
		base.InterruptTask();

		DeletePrevious();
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		ShowNextKey();

		for (int i = 0; i < keys.Count; i++)
		{
			if (Input.GetKeyDown(keys[i]))
				TypeKey(i);
		}
		if (Input.GetKeyDown(backspace))
			DeletePrevious();

		// Set progress
		if (typedKeys != null)
		{
			progress = 0;
			for (int i = 0; i < typedKeys.Length; i++)
			{
				// Check the key object's name after the : to get its key ID, and check if that is correct
				if (typedKeys[i] != null)
				{
					string sName = typedKeys[i].name;

					if (sName.Substring(sName.IndexOf(':') + 1).Equals(keyOrder[i].ToString()))
						progress++;
				}
			}
		}
	}

	public override string CatInterruptMessage()
	{
		return "Cat pressed backspace";
	}

	public override string ToString()
	{
		return "Write Email";
	}
}
