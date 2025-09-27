using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


public class LevelGenerator : MonoBehaviour
{

	public GameObject layoutRoom;
	int distanceToEnd;
	public static int roomCout;

	public Transform generatorPoint;
	public enum Direction { up, right, down, left };
	public enum Room { layoutRoom }
	public Room selectedRoom;
	public Direction selectedDirection;

	[SerializeField] private float xOffset;
	[SerializeField] private float yOffset;

	public LayerMask whatIsRoom;

	private GameObject endRoom;

	private List<GameObject> layoutRoomObjects = new List<GameObject>();

	public RoomPrefabs rooms;
	public RoomVariations roomVariations;

	void Start()
	{
		distanceToEnd = roomVariations.Send(distanceToEnd);
		roomCout = distanceToEnd;
		GameObject randomRoom = roomVariations.GetRandomRoom();
		Instantiate(randomRoom, generatorPoint.position, generatorPoint.rotation);

		selectedDirection = (Direction)Random.Range(0, 4);
		MoveGenerationPoint();

		for (int i = 0; i < distanceToEnd; i++)
		{
			GameObject newVariation = roomVariations.GetRandomRoom();
			GameObject newRoom = Instantiate(newVariation, generatorPoint.position, generatorPoint.rotation);

			layoutRoomObjects.Add(newRoom);

			if (i == distanceToEnd)
			{
				layoutRoomObjects.RemoveAt(layoutRoomObjects.Count - 1);

				endRoom = newRoom;
			}

			selectedDirection = (Direction)Random.Range(0, 4);
			MoveGenerationPoint();

			while (Physics2D.OverlapCircle(generatorPoint.position, 5f, whatIsRoom))
			{
				MoveGenerationPoint();
			}
		}

		CraeteRoomOutline(Vector3.zero);
		foreach (GameObject room in layoutRoomObjects)
		{
			CraeteRoomOutline(room.transform.position);
		}
	}

	void Update()
	{

	}

	public void MoveGenerationPoint()
	{
		switch (selectedDirection)
		{
			case Direction.up:
				generatorPoint.position += new Vector3(0f, yOffset, 0f);
				break;
			case Direction.left:
				generatorPoint.position += new Vector3(-xOffset, 0f, 0f);
				break;
			case Direction.down:
				generatorPoint.position += new Vector3(0f, -yOffset, 0f);
				break;
			case Direction.right:
				generatorPoint.position += new Vector3(xOffset, 0f, 0f);
				break;
		}
	}

	public void CraeteRoomOutline(Vector3 roomPosition)
	{
		bool roomAbove = Physics2D.OverlapCircle(roomPosition + new Vector3(0f, yOffset, 0f), .2f, whatIsRoom);
		bool roomBelow = Physics2D.OverlapCircle(roomPosition + new Vector3(0f, -yOffset, 0f), .2f, whatIsRoom);
		bool roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0f, 0f), .2f, whatIsRoom);
		bool roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0f, 0f), .2f, whatIsRoom);

		int directionCout = 0;
		if (roomAbove)
		{
			directionCout++;
		}
		if (roomBelow)
		{
			directionCout++;
		}
		if (roomLeft)
		{
			directionCout++;
		}
		if (roomRight)
		{
			directionCout++;
		}

		switch (directionCout)
		{
			case 0:
				Debug.LogError("Found no room");
				break;
			case 1:
				if (roomAbove)
					Instantiate(rooms.singleUp, roomPosition, transform.rotation);
				if (roomBelow)
					Instantiate(rooms.singleDown, roomPosition, transform.rotation);
				if (roomLeft)
					Instantiate(rooms.singleLeft, roomPosition, transform.rotation);
				if (roomRight)
					Instantiate(rooms.singleRight, roomPosition, transform.rotation);
				break;
			case 2:
				if (roomAbove & roomBelow)
					Instantiate(rooms.doubleUpDown, roomPosition, transform.rotation);
				if (roomLeft & roomRight)
					Instantiate(rooms.doubleLeftRight, roomPosition, transform.rotation);
				if (roomAbove & roomRight)
					Instantiate(rooms.doubleUpRight, roomPosition, transform.rotation);
				if (roomRight & roomBelow)
					Instantiate(rooms.doubleRightDown, roomPosition, transform.rotation);
				if (roomBelow & roomLeft)
					Instantiate(rooms.doubleDownLeft, roomPosition, transform.rotation);
				if (roomAbove & roomLeft)
					Instantiate(rooms.doubleUpLeft, roomPosition, transform.rotation);
				break;
			case 3:
				if (roomAbove & roomRight & roomBelow)
					Instantiate(rooms.tripleLeft, roomPosition, transform.rotation);
				if (roomRight & roomBelow & roomLeft)
					Instantiate(rooms.tripleUp, roomPosition, transform.rotation);
				if (roomAbove & roomBelow & roomLeft)
					Instantiate(rooms.tripleRight, roomPosition, transform.rotation);
				if (roomAbove & roomRight & roomLeft)
					Instantiate(rooms.tripleDown, roomPosition, transform.rotation);
				break;
			case 4:
				if (roomAbove & roomRight & roomBelow & roomLeft)
					Instantiate(rooms.fourth, roomPosition, transform.rotation);
				break;
		}
	}
}

[System.Serializable]
public class RoomPrefabs
{
	public GameObject singleUp, singleLeft, singleDown, singleRight,
		doubleUpDown, doubleLeftRight, doubleUpRight, doubleRightDown,
		doubleDownLeft, doubleUpLeft, tripleLeft, tripleUp, tripleRight,
		tripleDown, fourth;
}

[System.Serializable]
public class RoomVariant
{
	public string RoomVariantName;
	public int count = 0;
	public List<GameObject> roomPrefab = new List<GameObject>();

	public GameObject GetRandomPrefab()
	{
		int roll = Random.Range(0, roomPrefab.Count);
		return roomPrefab[roll];
	}
}

[System.Serializable]
public class RoomVariations
{
	public List<RoomVariant> roomVariants = new List<RoomVariant>();

	private LevelGenerator levelGenerator;
	private RandomRoom randRoom;

	bool firstRoom = false;
	int variationsAvailable = 0;

	public int Send(int x)
	{
		foreach (var rv in roomVariants)
		{
			variationsAvailable += rv.count;
		}
		return variationsAvailable;
	}

	public GameObject GetRandomRoom()
	{
		while (variationsAvailable > 0)
		{
			int roll = Random.Range(0, roomVariants.Count);
			if (firstRoom == false)
			{
				firstRoom = true;
				return roomVariants[0].GetRandomPrefab();
			}

			if (roomVariants[roll].count > 0 && variationsAvailable != 0)
			{
				foreach (var rv in roomVariants)
				{
					roomVariants[roll].count--;
					return roomVariants[roll].GetRandomPrefab();
				}
			}
		}
		return null;
	}
}
