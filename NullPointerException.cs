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

	void Update()
	{
		Vector3 character1Loc = character1.getPrefabObject().transform.position;
		Vector3 character2Loc = character2.getPrefabObject().transform.position;
		Vector3 character3Loc = character3.getPrefabObject().transform.position;

		Vector3 middleObjectiveLoc = new Vector3 (0f, 0.3000002f, 0f);
		Vector3 leftObjectiveLoc = new Vector3 (-40f, 0.3000002f, -24f);
		Vector3 rightObjectiveLoc = new Vector3 (40f, 0.3000002f, 24f);

		bool char2Initialized = false;
		bool char3Initialized = false;

		// Set character loadouts, can only happen when the characters are at base.
		if (character1.getZone() == zone.BlueBase || character1.getZone() == zone.RedBase)
			character1.setLoadout(loadout.LONG);
		if (character2.getZone() == zone.BlueBase || character2.getZone() == zone.RedBase)
			character2.setLoadout(loadout.MEDIUM);
		if (character3.getZone() == zone.BlueBase || character3.getZone() == zone.RedBase)
			character3.setLoadout(loadout.MEDIUM);

		// Place sniper in position, run to cover and call other characters if attacked
		if (character1.attackedFromLocations.Capacity == 0)
		{
			character1.MoveChar(new Vector3(-8.8f, 1.5f, 13.5f));
			character1.SetFacing(middleObjective.transform.position);
		}
		else if (character1.getZone() == zone.BlueBase || character1.getZone() == zone.RedBase)
		{
			character1.SetFacing (character1.attackedFromLocations [0]);
		}
		else
		{
			character1.MoveChar(character1.FindClosestCover(character1.attackedFromLocations[0]));
			character2.MoveChar(character1.attackedFromLocations[0]);
			character2.SetFacing(character1.attackedFromLocations[0]);
			character3.MoveChar(character1.attackedFromLocations[0]);
			character3.SetFacing(character1.attackedFromLocations[0]);

			character1.attackedFromLocations = new List<Vector3>();

			char2Initialized = true;
			char3Initialized = true;
		}

		// If at base, move char 2 and char 3 to middle objective
		if (!char2Initialized && !char3Initialized 
			&& ((character2.getZone() == zone.BlueBase && character3.getZone() == zone.BlueBase) 
				|| (character2.getZone() == zone.RedBase && character3.getZone() == zone.RedBase)))
		{
			character2.MoveChar(middleObjective.transform.position);
			character2.SetFacing(middleObjective.transform.position);
			character3.MoveChar(middleObjective.transform.position);
			character3.SetFacing(middleObjective.transform.position);
			char2Initialized = true;
			char3Initialized = true;
		}
		if (!char2Initialized 
			&& (character2.getZone() == zone.BlueBase || character2.getZone() == zone.RedBase))
		{
			character2.SetFacing(character3Loc);
			character2.MoveChar(character3Loc);
			char2Initialized = true;
		}
		if (!char3Initialized 
			&& (character3.getZone () == zone.BlueBase || character3.getZone () == zone.RedBase)) 
		{
			character3.SetFacing (character2Loc);
			character3.MoveChar (character2Loc);
			char3Initialized = true;
		}



		// face enemy and call other character if char 2 or char 3 attacked
		if (character2.attackedFromLocations.Capacity != 0)
		{
			character2.MoveChar(character2.attackedFromLocations[0]);
			character2.SetFacing(character2.attackedFromLocations[0]);
			character3.MoveChar(character2.attackedFromLocations[0]);
			character3.SetFacing(character2.attackedFromLocations[0]);

			character2.attackedFromLocations =  new List<Vector3>();

			char2Initialized = true;
			char3Initialized = true;
		}
		else if (character3.attackedFromLocations.Capacity != 0)
		{
			character2.MoveChar(character3.attackedFromLocations[0]);
			character2.SetFacing(character3.attackedFromLocations[0]);
			character3.MoveChar(character3.attackedFromLocations[0]);
			character3.SetFacing(character3.attackedFromLocations[0]);

			character3.attackedFromLocations = new List<Vector3>();

			char2Initialized = true;
			char3Initialized = true;
		}

		// send char 2 and 3 to capture objectives
		if (!char2Initialized && !char3Initialized) {
			
			if (middleObjective.getControllingTeam () != ourTeamColor) {
				
				character2.MoveChar (middleObjective.transform.position);
				character2.SetFacing (middleObjective.transform.position);
				character3.MoveChar (middleObjective.transform.position);
				character3.SetFacing (middleObjective.transform.position);

				char2Initialized = true;
				char3Initialized = true;

			} else if (rightObjective.getControllingTeam () != ourTeamColor) {
				
				character2.MoveChar (rightObjective.transform.position);
				character2.SetFacing (rightObjective.transform.position);
				character3.MoveChar (rightObjective.transform.position);
				character3.SetFacing (rightObjective.transform.position);

				char2Initialized = true;
				char3Initialized = true;

			} else if (Vector3.Distance (character2Loc, rightObjectiveLoc) < 5
			           && rightObjective.getControllingTeam () == ourTeamColor) {

				character2.MoveChar (middleObjective.transform.position);
				character2.SetFacing (middleObjective.transform.position);
				character3.MoveChar (middleObjective.transform.position);
				character3.SetFacing (middleObjective.transform.position);

				char2Initialized = true;
				char3Initialized = true;

			} else if (Vector3.Distance (character2Loc, middleObjectiveLoc) < 5
			           && middleObjective.getControllingTeam () == ourTeamColor) {

				character2.MoveChar (rightObjective.transform.position);
				character2.SetFacing (rightObjective.transform.position);
				character3.MoveChar (rightObjective.transform.position);
				character3.SetFacing (rightObjective.transform.position);

				char2Initialized = true;
				char3Initialized = true;

			}
		}
	}

	// a simple function to track game time
	public void gameTimer()
	{
		timer += 1;
	}

}

