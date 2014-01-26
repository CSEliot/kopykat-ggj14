using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	SpawnSpot[] spawnSpots;
	public Camera standbyCamera;

	// Use this for initialization
	void Start () {
		spawnSpots = GameObject.FindObjectsOfType<SpawnSpot> ();
		Connect ();
	}
	
	// Update is called once per frame
	void Connect(){
		PhotonNetwork.ConnectUsingSettings ("Kopy Kat");
	}

	void OnGUI(){
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());
	}

	void OnJoinedLobby(){
		Debug.Log ("Joined Lobby");
		PhotonNetwork.JoinRandomRoom ();
	}

	void OnPhotonRandomJoinFailed(){
		Debug.Log ("Failed to Join");
		PhotonNetwork.CreateRoom (null);
	}

	void OnJoinedRoom(){
		Debug.Log ("Joined Room");
		SpawnMyPlayer ();
	}

	void SpawnMyPlayer(){
		if (spawnSpots == null) {
			Debug.LogError ("Broken");
			return;
		}
		//spawns the player on a random spawn spot on the map
		SpawnSpot mySpawnSpot = spawnSpots[Random.Range (0, spawnSpots.Length)];

		//instantiate the prefab character
		GameObject myPlayer = (GameObject)PhotonNetwork.Instantiate ("First Person Controller", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
		standbyCamera.enabled = false;

		//disable the movement/camera scripts and main camera on the character 
		//prefab and then enable the scripts and main camera with this code.
		//these are the movement/camera scripts for FPS, just need to change to 3rd person
		((MonoBehaviour)myPlayer.GetComponent ("FPSInputController")).enabled = true;
		((MonoBehaviour)myPlayer.GetComponent ("MouseLook")).enabled = true;
		((MonoBehaviour)myPlayer.GetComponent ("CharacterMotor")).enabled = true;
		myPlayer.transform.FindChild ("Main Camera").gameObject.SetActive(true);
	}
}
