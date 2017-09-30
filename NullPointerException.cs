using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//---------- CHANGE THIS NAME HERE -------
public class NullPointerException : MonoBehaviour
{
	//private Vector3 position = new Vector3(20.0f, 0.0f, 20.0f);

	/// <summary>
	/// DO NOT MODIFY THIS! 
	/// vvvvvvvvv
	/// </summary>
	[SerializeField]
	public CharacterScript character1;
	[SerializeField]
	public CharacterScript character2;
	[SerializeField]
	public CharacterScript character3;
	/// <summary>
	/// ^^^^^^^^
	/// </summary>
	/// 

    // character1.getPrefabObject().GetComponent<TriggerEnter>().addListener();
    // Function will take typeOfItem, Vector3.

	// USEFUL VARIABLES
	private ObjectiveScript middleObjective;
	private ObjectiveScript leftObjective;
	private ObjectiveScript rightObjective;
	private float timer = 0;

	private team ourTeamColor;
	public static NullPointerException AddYourselfTo(GameObject host)
	{
		return host.AddComponent<NullPointerException>();
	}

	void Start()
	{
		// Set up code. This populates your characters with their controlling scripts
		character1 = transform.Find("Character1").gameObject.GetComponent<CharacterScript>();
		character2 = transform.Find("Character2").gameObject.GetComponent<CharacterScript>();
		character3 = transform.Find("Character3").gameObject.GetComponent<CharacterScript>();

		// populate the objectives
		middleObjective = GameObject.Find("MiddleObjective").GetComponent<ObjectiveScript>();
		leftObjective = GameObject.Find("LeftObjective").GetComponent<ObjectiveScript>();
		rightObjective = GameObject.Find("RightObjective").GetComponent<ObjectiveScript>();

		// save our team, changes every time
		ourTeamColor = character1.getTeam();
		// makes gametimer call every second
		InvokeRepeating("gameTimer", 0.0f, 1.0f);

	}

    float getDistance (Vector3 initial, Vector3 final) {
        return Vector3.Distance(initial, final);
    }

    void Update()
    {
        List<GameObject> items = character1.getItemList();

        // Locations of characters in this update.
        Vector3 character1Loc = character1.getPrefabObject().transform.position;
        Vector3 character2Loc = character2.getPrefabObject().transform.position;
        Vector3 character3Loc = character3.getPrefabObject().transform.position;

        // Locations of objectives
        Vector3 middleObjectiveLoc = new Vector3(0f, 0.3000002f, 0f);
        Vector3 leftObjectiveLoc = new Vector3(-40f, 0.3000002f, -24f);
        Vector3 rightObjectiveLoc = new Vector3(40f, 0.3000002f, 24f);

        // Whether character direction and face initializations have taken place.
        bool char1Initialized = false;
        bool char2Initialized = false;
        bool char3Initialized = false;

        bool safeGroup1 = true;
        // Group contains char 2 and char 3.
        bool safeGroup2 = true;

        const int maxItemDistance = 30;

        // Set character loadouts, can only happen when the charwacters are at base.
        if (character1.getZone() == zone.BlueBase || character1.getZone() == zone.RedBase)
        {
            character1.setLoadout(loadout.MEDIUM);
        }
        if (character2.getZone() == zone.BlueBase || character2.getZone() == zone.RedBase)
        {
            character2.setLoadout(loadout.SHORT);
        }
        if (character3.getZone() == zone.BlueBase || character3.getZone() == zone.RedBase) 
        {
            character3.setLoadout(loadout.SHORT);
        }

        // If at base, move char 2 and char 3 to middle objective
        if (((character2.getZone() == zone.BlueBase && character3.getZone() == zone.BlueBase)
                || (character2.getZone() == zone.RedBase && character3.getZone() == zone.RedBase)))
        {
            moveTowards(character2, middleObjective);
            moveTowards(character3, middleObjective);

            char2Initialized = true;
            char3Initialized = true;
        }
		if (!char1Initialized
			&& (character1.getZone() == zone.BlueBase || character1.getZone() == zone.RedBase))
		{
            if (leftObjective.getControllingTeam() != ourTeamColor) {
                moveTowards(character1, leftObjective);
            } else if (rightObjective.getControllingTeam() != ourTeamColor) {
                moveTowards(character1, rightObjective);
            } else {
                moveTowards(character1, middleObjective);
            }
			char1Initialized = true;
		}
        if (!char2Initialized && (character2.getZone() == zone.BlueBase || character2.getZone() == zone.RedBase))
        {
            moveTowards(character2, character3);
            char2Initialized = true;
        }
        if (!char3Initialized && (character3.getZone() == zone.BlueBase || character3.getZone() == zone.RedBase))
        {
            moveTowards(character3, character2);
            return;
        }

        if (character1.visibleEnemyLocations.Count != 0) {
            
            moveTowards(character1, character1.visibleEnemyLocations[0]);
			if (getDistance(character1Loc, character2Loc) < 30)
			{
				moveTowards(character2, character1.visibleEnemyLocations[0]);
				moveTowards(character3, character1.visibleEnemyLocations[0]);

				char2Initialized = true;
				char3Initialized = true;
                safeGroup2 = false;
			}


            character1.visibleEnemyLocations = new List<Vector3>();
            char1Initialized = true;
            safeGroup1 = false;

        }

        if (character2.visibleEnemyLocations.Count != 0)
		{
			moveTowards(character2, character2.visibleEnemyLocations[0]);
			moveTowards(character3, character2.visibleEnemyLocations[0]);

			if (getDistance(character1Loc, character2Loc) < 30)
			{
                moveTowards(character1, character2.visibleEnemyLocations[0]);
                char1Initialized = true;
                safeGroup1 = false;
			}

            safeGroup2 = false;
            character2.visibleEnemyLocations = new List<Vector3>();
			char2Initialized = true;
			char3Initialized = true;

		}
        else if (character3.visibleEnemyLocations.Count != 0)
		{
			moveTowards(character2, character3.visibleEnemyLocations[0]);
			moveTowards(character3, character3.visibleEnemyLocations[0]);

			if (getDistance(character1Loc, character2Loc) < 30)
			{
				moveTowards(character1, character3.visibleEnemyLocations[0]);
				char1Initialized = true;
                safeGroup1 = false;
			}

            character3.visibleEnemyLocations = new List<Vector3>();

            safeGroup2 = false;
			char2Initialized = true;
			char3Initialized = true;

		}

		// face enemy and go to other characters if char 1 attacked
		if (character1.attackedFromLocations.Count != 0)
		{
			character1.MoveChar(character2Loc);
			character1.SetFacing(character1.attackedFromLocations[0]);

			if (getDistance(character1Loc, character2Loc) < 30)
			{
				moveTowards(character2, character1.visibleEnemyLocations[0]);
				moveTowards(character3, character1.visibleEnemyLocations[0]);

				safeGroup2 = false;
				char2Initialized = true;
				char3Initialized = true;
			}

			character1.attackedFromLocations = new List<Vector3>();

			safeGroup1 = false;
			char1Initialized = true;
		}

		// face enemy and call other character if char 2 or char 3 attacked
		if (character2.attackedFromLocations.Count != 0)
		{
			moveTowards(character2, character2.attackedFromLocations[0]);
			moveTowards(character3, character2.attackedFromLocations[0]);

			if (getDistance(character1Loc, character2Loc) < 30)
			{
				moveTowards(character1, character2.attackedFromLocations[0]);
				char1Initialized = true;
				safeGroup1 = false;
			}

			character2.attackedFromLocations = new List<Vector3>();

			safeGroup2 = false;
			char2Initialized = true;
			char3Initialized = true;
		}
		else if (character3.attackedFromLocations.Count != 0)
		{
			moveTowards(character2, character3.attackedFromLocations[0]);
			moveTowards(character2, character3.attackedFromLocations[0]);

			if (getDistance(character1Loc, character3Loc) < 30)
			{
				moveTowards(character1, character3.attackedFromLocations[0]);
				char1Initialized = true;
				safeGroup1 = false;
			}

			character3.attackedFromLocations = new List<Vector3>();

			safeGroup2 = false;
			char2Initialized = true;
			char3Initialized = true;
		}

        // send char 2 and 3 to capture objectives
        if ((!char2Initialized || !char3Initialized) 
            && middleObjective.getControllingTeam() != ourTeamColor
            && ((getDistance(character2Loc, rightObjective.transform.position)
                 > getDistance(character2Loc, middleObjective.transform.position))
                 || rightObjective.getControllingTeam() == ourTeamColor))
        {

			moveTowards(character2, middleObjective);
			moveTowards(character3, middleObjective);

            char2Initialized = true;
            char3Initialized = true;

        }
        if ((!char2Initialized || !char3Initialized)
            && rightObjective.getControllingTeam() != ourTeamColor
			&& ((getDistance(character2Loc, rightObjective.transform.position)
				 <= getDistance(character2Loc, middleObjective.transform.position))
                || middleObjective.getControllingTeam() == ourTeamColor))
        {

            moveTowards(character2, rightObjective);
			moveTowards(character3, rightObjective);


            char2Initialized = true;
            char3Initialized = true;

        }

        if ((!char2Initialized || !char3Initialized) 
            && getDistance(character2Loc, rightObjectiveLoc) < 5)
        {

            moveTowards(character2, middleObjective);
			moveTowards(character3, middleObjective);

			char2Initialized = true;
			char3Initialized = true;

        }
        if ((!char2Initialized || !char3Initialized) 
            && getDistance(character2Loc, middleObjectiveLoc) < 5)
        {

            moveTowards(character2, rightObjective);
			moveTowards(character3, rightObjective);

            char2Initialized = true;
            char3Initialized = true;

        }

		if (!char1Initialized 
            && ((getDistance(character1Loc, leftObjective.transform.position)
                 <= getDistance(character1Loc, middleObjective.transform.position))
			|| middleObjective.getControllingTeam() == ourTeamColor))
		{

			character1.MoveChar(leftObjective.transform.position);
			character1.SetFacing(leftObjective.transform.position);

			char1Initialized = true;

		}
		if (!char1Initialized
			&& ((getDistance(character1Loc, leftObjective.transform.position)
				 > getDistance(character1Loc, middleObjective.transform.position))
                || leftObjective.getControllingTeam() == ourTeamColor))
		{

			character1.MoveChar(middleObjective.transform.position);
			character1.SetFacing(middleObjective.transform.position);

			char1Initialized = true;

		}

    	if (!char1Initialized
              && getDistance(character1Loc, leftObjective.transform.position) < 5)
    	{

    		character1.MoveChar(middleObjective.transform.position);
    		character1.SetFacing(middleObjective.transform.position);

    		char1Initialized = true;

    	}
    	else if (!char1Initialized
                   && getDistance(character1Loc, middleObjective.transform.position) < 5)
    	{

            character1.MoveChar(leftObjective.transform.position);
            character1.SetFacing(leftObjective.transform.position);

    		char1Initialized = true;

    	}

        if (safeGroup1 && !char1Initialized)
		{
			foreach (GameObject item in items)
			{
				// Collect items if close.
				if (getDistance(character1Loc, item.transform.position) < maxItemDistance)
				{
					character1.MoveChar(item.transform.position);
					character1.SetFacing(item.transform.position);
				}
			}
		}
        if (safeGroup2 && (!char2Initialized || !char3Initialized))
		{
			foreach (GameObject item in items)
			{
				// Collect items if close.
				if (getDistance(character2Loc, item.transform.position) < maxItemDistance
                        && !char2Initialized)
				{
					character2.MoveChar(item.transform.position);
					character2.SetFacing(item.transform.position);
				}
				else if (getDistance(character3Loc, item.transform.position) < maxItemDistance
                        && !char3Initialized)
				{
					character3.MoveChar(item.transform.position);
					character3.SetFacing(item.transform.position);
				}
			}
		}

	}

    void moveTowards(CharacterScript character, ObjectiveScript location) 
    {
        character.MoveChar(location.transform.position);
        character.SetFacing(location.transform.position);
    }
	void moveTowards(CharacterScript character, CharacterScript location)
	{
		character.MoveChar(location.getPrefabObject().transform.position);
		character.SetFacing(location.getPrefabObject().transform.position);
	}
	void moveTowards(CharacterScript character, Vector3 location)
	{
        character.MoveChar(location);
		character.SetFacing(location);
	}


	// a simple function to track game time
	public void gameTimer()
	{
		timer += 1;
	}

}

