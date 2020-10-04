using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenController : MonoBehaviour
{
	// Whether the player and the cat are in the trigger collider
	private bool playerIsHere;
	private bool catIsHere;

	public GameObject highlightOverlay;

	public KeyCode feedCatKey;

	// Reference to the cat controller
	private CatController _cat;

    // Start is called before the first frame update
    void Start()
    {
		// Fetch cat
		_cat = FindObjectOfType<CatController>();
    }

    // Update is called once per frame
    void Update()
    {
		highlightOverlay.SetActive(playerIsHere && catIsHere);

		if (playerIsHere && catIsHere && Input.GetKeyDown(feedCatKey))
		{
			// Feed the cat
			_cat.Feed();
		}
    }

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			playerIsHere = true;
		}
		if (collision.tag == "Cat")
		{
			catIsHere = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			playerIsHere = false;
		}
		if (collision.tag == "Cat")
		{
			catIsHere = false;
		}
	}
}
