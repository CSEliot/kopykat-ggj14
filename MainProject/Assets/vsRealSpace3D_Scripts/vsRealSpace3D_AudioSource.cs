// *******************************************************************************
// * Copyright (c) 2012, 2013, 2014 VisiSonics, Inc.
// * This software is the proprietary information of VisiSonics, Inc.
// * All Rights Reserved.
// ********************************************************************************


using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using vsRealSpace3D_StatusCodes;

[ExecuteInEditMode]
[System.Serializable]

public class vsRealSpace3D_AudioSource : MonoBehaviour {
	
	[DllImport("vsRealSpace3D")]
	private static extern void _vsAdjustVolume(string sSourceTag, double lfVolume);
	
	[DllImport("vsRealSpace3D")]
	private static extern vsSTATUS _vsInit3DSoundEngine(string sAppDataPath, string sEngineFlag, string sAudioComponent);
	
	[DllImport("vsRealSpace3D")]
	private static extern vsSTATUS _vsIs3DSoundEngineRunning();
	
	[DllImport("vsRealSpace3D")]
	private static extern vsSTATUS _vsMuteSound(string sSourceTag, bool fMute, double lfVolume = 0.0f);
	
	[DllImport("vsRealSpace3D")]
	private static extern void _vsPauseSound(string sSourceTag);
	
	[DllImport("vsRealSpace3D")]
	private static extern void _vsPlaySound(string sSourceTag);
	
	[DllImport("vsRealSpace3D")]
	private static extern vsSTATUS _vsPlaySoundContinuously(string sSourceTag);

	[DllImport("vsRealSpace3D")] 
	private static extern vsSTATUS _vsPlaySoundFromTo(string sSourceTag, vstime fromTime, vstime toTime);
		
	[DllImport("vsRealSpace3D")] 
	private static extern vsSTATUS _vsPlaySoundTo(string sSourceTag, vstime theTime);
	
	[DllImport("vsRealSpace3D")]
	private static extern void _vsResumeSound(string sSourceTag);
	
	[DllImport("vsRealSpace3D")]
	private static extern vsSTATUS _vsSetSoundSource(string sSoundFile, string sSourceTag);
	
	[DllImport("vsRealSpace3D")]
	private static extern void _vsStopSound(string sSourceTag);
	
	[DllImport("vsRealSpace3D")]
	private static extern void _vsUpdateSourcePosition(double lfX, double lfY, double lfZ, string sSourceTag);
	
	[StructLayout(LayoutKind.Sequential, Pack =1)]
	[System.Serializable]
	
	/// <summary>
	/// vstime. Time structure used in the vsRealSpace3D plugin
	/// </summary>
	public class vstime
	{
		public int nHours;
		public int nMinutes;
		public int nSeconds;
	}
	
	public AudioClip 			theAudioClip;
	public bool 				bMute = 			false;
	public bool					bPlayOnStart =		false;
	public bool					bPlayOnTrigger =	false;
	public bool 				bLoop =				false;
	public bool					bDurationSelected = false;
	public float	 			fVolume = 			1.0f;
	public string				sAudioClipPath = 	"";
	public string				sAudioLength = 		"";
	public string				sSoundSourceTag = 	"";
	
	public vstime				tFromTime;
	public vstime				tToTime;
	
	private bool 				bVSRealSpace3DIsRunning = 	false;
	private Transform  			theSoundSource; 		// the object that is running this puppy
	
	/// <summary>
	/// Awake this instance. The method grabs the parent object that this sound source is tied to.
	/// InitVSRealSpace3D a local method is called to handle the initialization method calls to the
	/// vsRealSpace3D plugin.
	/// </summary>
	void Awake () {
			
		Debug.Log("Awake on AudioSource: " + transform.name + " tag: " + sSoundSourceTag + " called...");
			
		// set the sound source from the transform
		theSoundSource = transform;

		if(theSoundSource == null)
		{
			Debug.LogError("Awake - GameObject: " + transform.name + " theSoundSource isn't valid. No 3D Sound will play for this Sound Source");
			return;
		}
		
		if(sSoundSourceTag == "")
		{
			Debug.LogError("Awake - GameObject: " + transform.name + " no unique tag name entered. No 3D Sound will play for this Sound Source.");
			return;
		}
		
		InitVSRealSpace3D();
		
		Debug.Log("Awake on AudioSource: " + transform.name + " tag: " + sSoundSourceTag + " completed...");
	}
		
	/// <summary>
	/// Start this instance. This method runs only while the application is running.
	/// It starts a coroutine that waits for the vsRealSpace3D engine to be up and running.
	/// </summary>
	void Start () {
		
		if(theSoundSource == null || sSoundSourceTag == "")
			return;
		
		Debug.Log("Start on AudioSource: " + transform.name + " tag: " + sSoundSourceTag + " called...");
		
		if(Application.isPlaying)
		{		
			Debug.Log("vsAudioSource - Start called, GameObject: " + transform.name + " tag: " + sSoundSourceTag);
		
			StartCoroutine(WaitForVSRealSpace3DSoundEngineReady());		
		}		
		
		Debug.Log("Awake on AudioSource completed: " + transform.name + " tag: " + sSoundSourceTag + " completed...");	
	}
	
	/// <summary>
	/// Waits for vsRealSpace3D sound engine ready. When the vsRealSpace3D engine is up
	/// and running if the sound source is set to play on start it will be, or if set to mute
	/// it will be. 
	/// </summary>
	/// <returns>
	/// The for vsRealSpace3D sound engine ready.
	/// </returns>
	IEnumerator WaitForVSRealSpace3DSoundEngineReady() {
		
		Debug.Log("vsAudioSource - Coroutine WaitForVSRealSpace3DSoundEngineReady: " + transform.name + " tag: " + sSoundSourceTag);
		
		while(_vsIs3DSoundEngineRunning() == vsSTATUS.kSTATUS_VSREALSPACE3D_NOT_RUNNING)
		{
			Debug.Log("vsAudioSource coroutine waiting... " + transform.name + " tag: " + sSoundSourceTag);
			
			yield return new WaitForSeconds(1.0f);
		}
		
		Debug.Log("vsAudioSource - Coroutine done: " + transform.name + " tag: " + sSoundSourceTag);
				
		if(bPlayOnStart)
			vsPlaySound();
		
		if(bMute)
			vsMuteSound(true);
		
		bVSRealSpace3DIsRunning = true;
	}
		
	/// <summary>
	/// Raises the application quit event.
	/// </summary>
	void OnApplicationQuit() {
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {	
		
		if(Application.isPlaying)
		{		
			if(theSoundSource == null || sSoundSourceTag == "")
				return;
			
			// if vsRealSpace3D engine isn't running bail
			if(!bVSRealSpace3DIsRunning)
				return;
		
			//							y
			//						    |   z
			//							|/
			//						    |
			// unity coord system -x ------ x
			// 

			// grab the player (the first person camera)
			var _thePlayer = Camera.main;
		
			// get the root objects (sound source) current position 
			var _soundSourceRelativeToPlayer = _thePlayer.transform.InverseTransformPoint(theSoundSource.position);
	
			// update the player position to source position
			_vsUpdateSourcePosition(_soundSourceRelativeToPlayer.x, _soundSourceRelativeToPlayer.y, _soundSourceRelativeToPlayer.z, sSoundSourceTag);						
		}
	}	
	
	private vsSTATUS InitVSRealSpace3D()
	{
		Debug.Log("InitVSRealSpace3D vsAudioSource called - GameObject: " + transform.name + " tag: " + sSoundSourceTag);
		
		vsSTATUS _kResult = _vsInit3DSoundEngine(Application.streamingAssetsPath, "Unity", "AudioSource");
			
		if(_kResult != vsSTATUS.kSUCCESS)
#if UNITY_EDITOR				
			Debug.LogError("Result of Init3DSoundEngine: " + _kResult + " tag: " + sSoundSourceTag);
#else
			Debug.Log("Result of Init3DSoundEngine: " + _kResult);
#endif			
		Debug.Log("Setting Sound Source - vsAudioSource - GameObject: " + transform.name + " tag: " + sSoundSourceTag);
		
		//
		// set the sound source name
		//
			
		// strip away "Assets" (six characters)
		string _sSoundSource = 	sAudioClipPath.Remove(0, 6);
		_sSoundSource = 		Application.dataPath + _sSoundSource;
			
		Debug.Log("_sSoundSource = " + _sSoundSource + " tag: " + sSoundSourceTag);
		
		_kResult = _vsSetSoundSource(_sSoundSource, sSoundSourceTag);	
						
		if(_kResult != vsSTATUS.kSUCCESS)
#if UNITY_EDITOR				
			Debug.LogError("vsRealSpace3D failed to set Sound Source for: " + transform.name);			
#else			
			Debug.Log("vsRealSpace3D failed to set Sound Source for: " + transform.name);			
#endif		
		
		Debug.Log("InitVSRealSpace3D vsAudioSource completed - GameObject: " + transform.name + " tag: " + sSoundSourceTag);
		
		return _kResult;
	}
	
	/// <summary>
	/// vsAdjustVolume calls the vsRealSpace3D plugin to adjust the volume of the sound source identified by 
	/// its unique source tag.
	/// </summary>
	/// <param name='sSourceTag'>
	/// S source tag. Unique string identifier for each sound source.
	/// </param>
	/// <param name='lfVolume'>
	/// The volume the sound source should be played. Valid range 0.0 - 10.0 (0.0 = no sound, 10.0 = very loud)
	/// </param>		
	public void vsAdjustVolume(float _fVolume)
	{
		fVolume = _fVolume;
		_vsAdjustVolume(sSoundSourceTag, (double) fVolume);
	}
	
	/// <summary>
	/// vsInit3DSoundEngine calls the vsRealSpace3D plugin setting the applicatin data path, the 
	/// game engine being used, and which component it is (audio source or audio listener).
	/// The call is made from the scripts Awake method. In game mode this is the first method called.
	/// During editing Unity calls the Awake method during loading and building. Unity destroys all obejects
	/// on load and build.
	/// </summary>
	/// <returns>
	/// vsStatus is returned. See vsRealSpace3D StatusCodes.cs for errors returned.
	/// </returns>
	/// <param name='sAppDataPath'>
	/// the Unity StreamingAssets data path
	/// </param>
	/// <param name='sEngineFlag'>
	/// flag indicating which game engine is being used.
	/// </param>
	/// <param name='sAudioComponent'>
	/// Identifies the vsRealSpace3D audio component (i.e., AudioSource, AudioListener)
	/// </param>	
	public int vsInit3DSoundEngine(string sAppDataPath, string sEngineFlag, string sAudioComponent)
	{
		vsSTATUS _kResult = _vsInit3DSoundEngine(sAppDataPath, sEngineFlag, sAudioComponent);
			
		if(_kResult != vsSTATUS.kSUCCESS)
#if UNITY_EDITOR				
			Debug.LogError("Result of Init3DSoundEngine: " + _kResult + " tag: " + sSoundSourceTag);
#else
			Debug.Log("Result of Init3DSoundEngine: " + _kResult);
#endif			
		
		return (int) _kResult;
		
	}
	
	/// <summary>
	/// vsIs3DSoundEngineRunning calls the vsRealSpace3D plugin to see if it is up and running. 
	/// "Now witness the firepower of this fully ARMED and OPERATIONAL battle station!" - Emperor Palpatine
	/// This method is called from the WaitForVSRealSpace3DSoundEngineReady co-routine method.
	/// </summary>
	/// <returns>
	/// vsSTATUS is returned. Either kSTATUS_VSREALSPACE3D_IS_RUNNING or kSTATUS_VSREALSPACE3D_IS_NOT_RUNNING
	/// will be returned.
	/// </returns>	
	public int vsIs3DSoundEngineRunning()
	{
		vsSTATUS _kResult = _vsIs3DSoundEngineRunning();
		
		return (int) _kResult;
	}
	
	public bool vsIsTagInUse()
	{
		return false;
	}
	
	/// <summary>
	/// vsMuteSound calls the vsRealSpace3D plugin to mute the sound source identified by the 
	/// unique sound source tag. The sound source will continue to play, you just will not hear it.
	/// </summary>
	/// <returns>
	/// vsSTATUS is returned. See vsRealSpace3D_StatusCodes.cs for errors returned..
	/// </returns>
	/// <param name='sSourceTag'>
	/// Unique string identifier.
	/// </param>
	/// <param name='fMute'>
	/// A boolean that indicates if mute is on/off.
	/// </param>	
	public void vsMuteSound(bool _bFlag) {
			
		Debug.Log("MuteSound called: " + transform.name + " tag: " + sSoundSourceTag + " _bFlag = " + _bFlag);
		
		if(_bFlag)
			_vsMuteSound(sSoundSourceTag, _bFlag);
		else
			_vsMuteSound(sSoundSourceTag, _bFlag, fVolume);
		
		Debug.Log("MuteSound completed: " + transform.name + " tag: " + sSoundSourceTag + " _bFlag = " + _bFlag);	
	}
	
	public void vsOnTrigger() {
	}
	
	/// <summary>
	/// vsPauseSound calls the vsRealSpace3D plugin to pause the sound of the sound source
	/// identified by its unique source tag.
	/// </summary>
	/// <param name='sSourceTag'>
	/// S source tag. Unique string identifier for each sound source.
	/// </param>
	public void vsPauseSound(string sSourceTag)
	{
		_vsPauseSound(sSourceTag);
	}
	
	/// <summary>
	/// vsPlaySound calls the vsRealSpace3D plugin to play the sound of the sound source
	/// identified by its unique source tag.
	/// </summary>
	/// <param name='sSourceTag'>
	/// S source tag. Unique string identifier for each sound source.
	/// </param>	
	public void vsPlaySound () {
		
		Debug.Log("PlaySound called: " + transform.name + " tag: " + sSoundSourceTag);
		
		if(bMute)
			return;
			
		_vsAdjustVolume(sSoundSourceTag, (double) fVolume);

		vsSTATUS _kResult = vsSTATUS.kSUCCESS;
		
		if(bDurationSelected)
		{
			if(bLoop)
			{
				_kResult = _vsPlaySoundContinuously(sSoundSourceTag);
				
				if(_kResult != vsSTATUS.kSUCCESS)
				{
#if UNITY_EDITOR				
					Debug.LogError("_vsPlaySoundContinuously failed - Object: " + transform.name + " tag: " + sSoundSourceTag);			
#else			
					Debug.Log("_vsPlaySoundContinuously failed - Object: " + transform.name + " tag: " + sSoundSourceTag);			
#endif						
				}
				else
				{
					Debug.Log("vsAudioSource - Object: " + transform.name + " tag: " + sSoundSourceTag + " is playing");
				}
			}		
			else
			{
				_kResult = _vsPlaySoundFromTo(sSoundSourceTag, tFromTime, tToTime);
				
				if(_kResult != vsSTATUS.kSUCCESS)
				{
#if UNITY_EDITOR				
					Debug.LogError("_vsPlaySoundFromTo failed - Object: " + transform.name + " tag: " + sSoundSourceTag);			
#else			
					Debug.Log("vsPlaySoundFromTo failed - Object: " + transform.name + " tag: " + sSoundSourceTag);			
#endif						
				}
				else
				{
					Debug.Log("vsAudioSource - Object: " + transform.name + " tag: " + sSoundSourceTag + " is playing");
				}	
			}
		}		
		else
		{
			_vsPlaySound(sSoundSourceTag);		
		}	
	}
	
	/// <summary>
	/// vsPlaySoundContinuously calls the vsRealSpace3D plugin to continuously play the sound source
	/// identified by its unique source tag. When the sound comes to its completion it will repeat from 
	/// the beginning. 
	/// </summary>
	/// <returns>
	/// vsSTATUS is returned. See vsRealSpace3D_StatusCodes.cs for errors returned.
	/// </returns>
	/// <param name='sSourceTag'>
	/// S source tag. Unique string identifier for each sound source.
	/// </param>	
	public int vsPlaySoundContinuously(string sSourceTag)
	{
		vsSTATUS _kResult = _vsPlaySoundContinuously(sSourceTag);
		
		return (int) _kResult;
	}
	
	/// <summary>
	/// vsPlaySoundFromTo calls the vsRealSpace3D plugin to play the sound source identified by
	/// its unique sound source tag. The sound is played from the specified vstime struct to the 
	/// specified vstime struct. 
	/// </summary>
	/// <returns>
	/// vsSTATUS is returned. See vsRealSpace3D_StatusCodes.cs for errors returned.
	/// </returns>
	/// <param name='sSourceTag'>
	/// S source tag. Unique string identifier for each sound source. 
	/// </param>
	/// <param name='fromTime'>
	/// From time. vstime struct.
	/// </param>
	/// <param name='toTime'>
	/// To time. vstime struct.
	/// </param>	
	public int vsPlaySoundFromTo(string sSourceTag, vstime fromTime, vstime toTime)
	{
		vsSTATUS _kResult = _vsPlaySoundFromTo(sSourceTag, fromTime, toTime);
		
		return (int) _kResult;
	}
	
	/// <summary>
	/// vsPlaySoundTo calls the vsRealSpace3D plugin to play the sound source, identified by
	/// its unique sound source tag, up to the specified time indicated in the vstime structure.
	/// </summary>
	/// <returns>
	/// vsSTATUS is returned. See vsRealSpace3D_StatusCodes.cs for errors returned.
	/// </returns>
	/// <param name='sSourceTag'>
	/// S source tag. Unique string identifier for each sound source.
	/// </param>
	/// <param name='theTime'>
	/// The vstime structure. hrs:mins:secs
	/// </param>	
	public int vsPlaySoundTo(string sSourceTag, vstime theTime)
	{
		vsSTATUS _kResult = _vsPlaySoundTo(sSourceTag, theTime);
		
		return (int) _kResult;
	}
	
	/// <summary>
	/// vsResumeSound calls the vsRealSpace3D plugin to resume playing a paused sound source
	/// identified by its unique sound source tag. 
	/// </summary>
	/// <param name='sSourceTag'>
	/// S source tag. Unique string identifier for each sound source.
	/// </param>	
	public void vsResumeSound(string sSourceTag)
	{
		_vsResumeSound(sSourceTag);
	}
	
	/// <summary>
	/// vsSetSoundSource calls the vsRealSpace3D plugin to setup the sound source. The sound source file 
	/// and unique sound source tag is passed. 
	/// </summary>
	/// <returns>
	/// vsSTATUS is returned. See vsRealSpace3D_StatusCodes.cs for errors returned.
	/// </returns>
	/// <param name='sSoundFile'>
	/// The sound file to be played. Currently all vsRealSpace3D sounds must be located in the StreamingAssets/3DSounds
	/// folder. 
	/// </param>
	/// <param name='sSourceTag'>
	/// S source tag. Unique string identifier for each sound source.
	/// </param>	
	public int vsSetSoundSource(string sSoundFile, string sSourceTag)
	{
		vsSTATUS _kResult = _vsSetSoundSource(sSoundFile, sSourceTag);
		
		return (int) _kResult;
	}
	
	/// <summary>
	/// vsStopSound calls the vsRealSpace3D pluging to halt the sound source playing, indicated by its sound source tag.
	/// </summary>
	/// <param name='sSourceTag'>
	/// S source tag. Unique string identifier for each sound source.
	/// </param>	
	public void vsStopSound () {
		
		Debug.Log("vsAudioSource StopSound called - Object: " + transform.name + " tag: " + sSoundSourceTag);
		
		_vsStopSound(sSoundSourceTag);
		
		Debug.Log("vsAudioSource StopSound completed - Object: " + transform.name + " tag: " + sSoundSourceTag);	
	}
	
	/// <summary>
	/// vsUpdateSourcePosition calls the vsRealSpace3D plugin to update the sound sources position. The method takes the 
	/// the sound sources current x,y,z coordinate position, and its sound source tag. The sound source can be stationary
	/// or moving. This method is called in the Unity script's Update method.
	/// </summary>
	/// <param name='lfX'>
	/// Lf x. The sound source's x coord position.
	/// </param>
	/// <param name='lfY'>
	/// Lf y. The sound source's y coord position.
	/// </param>
	/// <param name='lfZ'>
	/// Lf z. The sound source's z coord position.
	/// </param>
	/// <param name='sSourceTag'>
	/// S source tag. Unique string identifier for each sound source. 
	/// </param>	
	public void vsUpdateSourcePosition(double lfX, double lfY, double lfZ, string sSourceTag)
	{
		_vsUpdateSourcePosition(lfX, lfY, lfZ, sSourceTag);
	}	
}
