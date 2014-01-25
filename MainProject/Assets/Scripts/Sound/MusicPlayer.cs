using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicPlayer : MonoBehaviour {
	
	public List<AudioClip> Playlist;
	private AudioSource audio;
	public bool ShouldLoop = true;
	private IEnumerator currentTrack;
	
	// Use this for initialization
	void Start () {
		//start playing music, and keep it going
		if(Playlist != null)
		{
			currentTrack = Playlist.GetEnumerator();
			currentTrack.MoveNext();
			audio = GetComponent<AudioSource>();
			if(audio != null)
			{
				audio.loop = ShouldLoop;
				audio.clip = getCurrentTrack();
				audio.Play();
			}
			else
			{
				Debug.Log ("MusicPlayer: couldn't find audio source!");
				this.enabled = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		//if the clip's not playing, we must have finished the song
		//go to the next song and play if we're not looping
		if(audio != null)
		{
			if(!audio.isPlaying && !ShouldLoop)
			{
				getNextTrack();
				audio.clip = getCurrentTrack();
				audio.Play();
			}
			else if (!audio.isPlaying && ShouldLoop)
			{
				audio.Play();
			}
		}
	}
	
	private AudioClip getCurrentTrack()
	{
		return (AudioClip)currentTrack.Current;
	}
	
	private void getNextTrack()
	{
		//try to go to the next track.
		//if we've moved past the end of the playlist...
		if(!currentTrack.MoveNext())
		{
			//go back to the start of the playlist
			currentTrack = Playlist.GetEnumerator();
			currentTrack.MoveNext();
		}
	}
}
