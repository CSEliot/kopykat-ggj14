// *******************************************************************************
// * Copyright (c) 2012, 2013, 2014 VisiSonics, Inc.
// * This software is the proprietary information of VisiSonics, Inc.
// * All Rights Reserved.
// ********************************************************************************

using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using vsRealSpace3D_StatusCodes;

[ExecuteInEditMode]
[System.Serializable]

public class vsRealSpace3D_AudioListener : MonoBehaviour {
	
	[DllImport("vsRealSpace3D")]
	private static extern vsSTATUS _vsGetCurrentTags();
	
	[DllImport("vsRealSpace3D")]
	private static extern vsSTATUS _vsInit3DSoundEngine(string sAppDataPath, string sEngineFlag, string sAudioComponent);
	
	[DllImport("vsRealSpace3D")]
	private static extern vsSTATUS _vsSetEnvironmentSize(double lfX, double lfY, double lfZ);
	
	[DllImport("vsRealSpace3D")]
	private static extern void _vsSetListenerDistance(double lfDistanceRadius);
	
	[DllImport("vsRealSpace3D")]
	private static extern vsSTATUS _vsSetPersonalization(int nHRTFMode, float fHeadRadius, float fTorsoRadius, float fNeckHeight);
	
	[DllImport("vsRealSpace3D")]
	private static extern vsSTATUS _vsSetReverberation(int nReverbMode);
	
	[DllImport("vsRealSpace3D")]
	private static extern vsSTATUS _vsShutdown3DSoundEngine();
	
	[DllImport("vsRealSpace3D")]
	private static extern vsSTATUS _vsStart3DSoundEngine();
	
	/// <summary>
	/// The HRTF (Head Related Transfer Function) types
	/// </summary>
	[System.Serializable]
	public enum HRTFTypeState { HRTF_Default, HRTF_Personalize };
	
	/// <summary>
	/// The default HRTFTypeState
	/// </summary>
	[SerializeField]
	private HRTFTypeState	eHRTFTypeState = HRTFTypeState.HRTF_Default;	
	
	/// <summary>
	/// The reverberation states used in the vsRealSpace3D engine
	/// </summary>
	[System.Serializable]
	public enum REVERBTypeState { REVERB_NO_FILTER, REVERB_EXPONENTIAL_DECREASE, REVERB_GAUSSIAN_DECREASE, REVERB_LINEAR_DECREASE, REVERB_LOGARITHMIC_DECREASE };
	
	/// <summary>
	/// The default REVERBTypeState used in the vsRealSpace3D engine
	/// </summary>
	[SerializeField]
	private REVERBTypeState eREVERBTypeState = 		REVERBTypeState.REVERB_LOGARITHMIC_DECREASE;
	
	public Vector3 			theWorldSize =			new Vector3(300.0f, 300.0f, 300.0f);
	public float 			theListenerDistance = 	15.0f;
	
	private float 			fHeadRadiusDefault = 	0.115f;
	private float			fTorsoRadiusDefault = 	0.230f;
	private float			fNeckHeightDefault = 	0.055f;
	
	public float			fHeadRadius = 			0.115f;
	public float			fTorsoRadius = 			0.230f;
	public float			fNeckHeight = 			0.055f;
	
//reh 09oct13 TBD later
//	public List<string> m_CurrentTagsList = new List<string>();
	
	
	/// <summary>
	/// Awake this instance. This method sets up the vsRealSpace3D AudioListener. The following 
	/// vsRealSpace3D methods are called in Awake; vsInit3DSoundEngine, vsSetPersonalization,
	/// vsSetReverberation, vsSetEnvironmentSize. 
	/// </summary>
	void Awake () {
		
		Debug.Log("vsAudioListener Awake called...\n");

		vsSTATUS _kResult = vsSTATUS.kSUCCESS;
		
		_kResult = _vsInit3DSoundEngine(Application.streamingAssetsPath, "Unity", "AudioListener");

		if(_kResult != vsSTATUS.kSUCCESS)
		{
#if UNITY_EDITOR				
			Debug.LogError("_vsInit3DSoundEngine failed. " + _kResult);
#else				
			Debug.Log("_vsInit3DSoundEngine failed. " + _kResult);
#endif				
		}
		
		if(eHRTFTypeState == HRTFTypeState.HRTF_Default)
		{
			fHeadRadius = fHeadRadiusDefault;
			fTorsoRadius =fTorsoRadiusDefault;
			fNeckHeight = fNeckHeightDefault;
		}
		
		_kResult = _vsSetPersonalization((int) eHRTFTypeState, fHeadRadius, fTorsoRadius, fNeckHeight);
			
Debug.Log("fHeadRadius = " + fHeadRadius + ", fTorsoRadius = " + fTorsoRadius + ", fNeckHeight = " + fNeckHeight);		
		
		if(_kResult != vsSTATUS.kSUCCESS)
		{
#if UNITY_EDITOR				
			Debug.LogError("_vsSetPersonalization failed. " + _kResult);
#else				
			Debug.Log("_vsSetPersonalization failed. " + _kResult);
#endif				
		}
			
		_kResult = _vsSetReverberation((int) eREVERBTypeState);

		if(_kResult != vsSTATUS.kSUCCESS)
		{
#if UNITY_EDITOR				
			Debug.LogError("_vsSetReverberation failed. " + _kResult);
#else
			Debug.Log("_vsSetReverberation failed. " + _kResult);
#endif				
		}
				
		_kResult = _vsSetEnvironmentSize(theWorldSize.x, theWorldSize.y, theWorldSize.z);			
			
		if(_kResult != vsSTATUS.kSUCCESS)
		{
#if UNITY_EDITOR				
			Debug.LogError("_vsSetEnvironmentSize failed. " + _kResult);
#else
			Debug.Log("_vsSetEnvironmentSize failed. " + _kResult);
#endif				
		}

		_vsSetListenerDistance((double)theListenerDistance);			
		
		Debug.Log("vsAudioListener Awake completed...");
	}
		
	/// <summary>
	/// Start this instance. The method only performs if the application is actually
	/// running. It does nothing when the editor is loading or building. The method calls
	/// the vsRealSpace3D plugin method vsStart3DSoundEngine to start the 3D sound engine going. 
	/// If there is a failure in the 3D sound engine, the error will be logged, 
	/// and vsShutdown3DSoundEngine will be called. The vsRealSpace3D sound engine will cease running,
	/// and your application will continue but you will hear no vsRealSpace3D sound.
	/// If you are running in the editor check the Unity editor console for log errors
	/// or if running from stand-alone check the Unity log, "output_log.txt" this is found in your
	/// "applicationName_Data" folder.
	/// </summary>
	void Start () {

		Debug.Log("vsAudioListener Start Called...\n");
		
		if(Application.isPlaying)
		{
			Debug.Log("vsAudioListener Start Called and isPlaying...");	
			
			vsSTATUS _kResult = _vsStart3DSoundEngine();
		
			if(_kResult != vsSTATUS.kSUCCESS)
			{
#if UNITY_EDITOR				
				Debug.LogError("vsRealSpace3D failed to start the 3D sound engine: " + _kResult);			
				Debug.LogError("vsRealSpace3d_Start3DEngine failed. You will hear no 3D sound.");
#else
				Debug.Log("vsRealSpace3D failed to start the 3D sound engine: " + _kResult);			
				Debug.Log("vsRealSpace3d_Start3DEngine failed. You will hear no 3D sound.");
#endif				
				_vsShutdown3DSoundEngine();
			}	
			
			Debug.Log("vsAudioListener Start has completed...");
		}
	}
	
	/// <summary>
	/// Raises the application quit event. The vsRealSpace3D plugin method vsShutdown3DSoundEngine
	/// is called if the application is playing.
	/// </summary>
	void OnApplicationQuit() {
		
		Debug.Log("vsAudioListener OnApplicationQuit called...");
		
		if(Application.isPlaying)
		{
			Debug.Log("Stopping vsAudioListener...\n");
			_vsShutdown3DSoundEngine();				
		}
		
		Debug.Log("vsAudioListener OnApplicationQuit has completed");
	}
		
	/// <summary>
	/// Gets or sets the state of the hrtf.
	/// </summary>
	/// <value>
	/// The state of the hrtf.
	/// </value>
	public HRTFTypeState hrtfState { 
		
		get{ return eHRTFTypeState; }
 
		set{ eHRTFTypeState =  value; }
	}	
	
	/// <summary>
	/// Gets or sets the state of the reverb.
	/// </summary>
	/// <value>
	/// The state of the reverb.
	/// </value>
	public REVERBTypeState reverbState { 
		
		get{ return eREVERBTypeState; }
 
		set{ eREVERBTypeState =  value; }
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
	public int vsInit3DSoundEngine(string sAppDataPath, string sEngineFlag, string sAudioComponent) {
	
		vsSTATUS _kResult = _vsInit3DSoundEngine(sAppDataPath, sEngineFlag, sAudioComponent);
		
		return (int) _kResult;
	}
	
	/// <summary>
	/// vsSetEnvironmentSize calls the vsRealSpace3D plugin setting the world size that the 3D sound will be playing in.
	/// The inputs represent meters. This call is made from the scripts Awake method. 
	/// In game mode this is the first method called.
	/// During editing Unity calls the Awake method during loading and building. Unity destroys all obejects
	/// on load and build.
	/// </summary>
	/// <returns>
	/// vsStatus is returned. See vsRealSpace3D StatusCodes.cs for errors returned.
	/// </returns>
	/// <param name='lfX'>
	/// The world size x coordinate value in meters
	/// </param>
	/// <param name='lfY'>
	/// The world size y coordinate value in meters
	/// </param>
	/// <param name='lfZ'>
	/// The world size z coordinate value in meters
	/// </param>	
	public int vsSetEnvironmentSize(double lfX, double lfY, double lfZ)
	{
		vsSTATUS _kResult = _vsSetEnvironmentSize(lfX, lfY, lfZ);
		
		return (int) _kResult;
	}
	
	/// <summary>
	/// vsSetListenerSize calls the vsRealSpace3D plugin to set the size of the listener sphere 
	/// around the listener.
	/// </summary>
	/// <param name='lfX'>
	/// listener's x coordinate
	/// </param>
	/// <param name='lfY'>
	/// listener's y coordinate
	/// </param>
	/// <param name='lfZ'>
	/// listener's z coordinate
	/// </param>
	public void vsSetListenerSize(double lfDistanceRadius)
	{
		_vsSetListenerDistance(lfDistanceRadius);
	}
	
	/// <summary>
	/// vsSetPersonalization calls the vsRealSpace3D plugin setting the HRTF (Head Related Transfer Function)
	/// mode, the listeners head radius, torso radius, and neck height.
	/// This call is made from the Unity script's Awake method. 
	/// In game mode this is the first method called.
	/// During editing Unity calls the Awake method during loading and building. Unity destroys all obejects
	/// on load and build.
	/// </summary>
	/// <returns>
	/// vsStatus is returned. See vsRealSpace3D StatusCodes.cs for errors returned.
	/// </returns>
	/// <param name='nHRTFMode'>
	/// This value should be either the HRTFTypeState HRTF_Default or HRTF_Personalize
	/// </param>
	/// <param name='fHeadRadius'>
	/// The listener's head raduis, valid values from 0.06 - 0.12 meters.
	/// </param>
	/// <param name='fTorsoRadius'>
	/// The listener's torso radius, valid values from 0.12 - 0.32 meters.
	/// </param>
	/// <param name='fNeckHeight'>
	/// The listener's neck height, valid values from 0.00 - 0.08 meters.
	/// </param>
	public int vsSetPersonalization(int nHRTFMode, float fHeadRadius, float fTorsoRadius, float fNeckHeight)
	{
		vsSTATUS _kResult = _vsSetPersonalization(nHRTFMode, fHeadRadius, fTorsoRadius, fNeckHeight);
		
		return (int) _kResult;
	}
	
	/// <summary>
	/// vsSetReverberation calls the vsRealSpace3D plugin setting the reverberation mode to be used.
	/// The reverberation mode to be used is passed. The value should be one of the REVERBTypeState 
	/// values. This call is made from the Unity script's Awake method. 
	/// In game mode this is the first method called.
	/// During editing Unity calls the Awake method during loading and building. Unity destroys all obejects
	/// on load and build.
	/// </summary>
	/// <returns>
	/// vsStatus is returned. See vsRealSpace3D StatusCodes.cs for errors returned.
	/// </returns>
	/// <param name='nReverbMode'>
	/// The mode should be one of the following..
	/// 
	/// REVERB_NO_FILTER: 				Do not apply a filter.
	/// 
	/// REVERB_EXPONENTIAL_DECREASE: 	Exponential decrease of cut-off frequency with time.
	/// 
	/// REVERB_GAUSSIAN_DECREASE: 		Gaussian profile decrease of cut-off frequency with time.
	/// 
	/// REVERB_LINEAR_DECREASE: 		Linear decrease of cut-off frequency with time.
	/// 
	/// REVERB_LOGARITHMIC_DECREASE: 	Logarithmic profile decrease of cut-off frequency with time.
	/// 
	/// </param>	
	public int vsSetReverberation(int nReverbMode)
	{
		vsSTATUS _kResult = _vsSetReverberation(nReverbMode);
		
		return (int) _kResult;
	}
	
	/// <summary>
	/// vsShutdown3DSoundEngine calls the vsRealSpace3D plugin, shutting down the plugin.
	/// This method is called from the Unity script's OnApplicationQuit. 
	/// </summary>
	/// <returns>
	/// vsStatus is returned. See vsRealSpace3D StatusCodes.cs for errors returned.
	/// </returns>	
	public int vsShutdown3DSoundEngine()
	{
		vsSTATUS _kResult =  _vsShutdown3DSoundEngine();
		
		return (int) _kResult;
	}
	
	/// <summary>
	/// vsStart3DSoundEngine calls the vsRealSpace3D plugin, starting the VisiSonics
	/// 3D sound engine. This method is called from the Unity script's Start method.
	/// </summary>
	/// <returns>
	/// vsStatus is returned. See vsRealSpace3D StatusCodes.cs for errors returned.
	/// </returns>	
	public int vsStart3DSoundEngine()
	{
		vsSTATUS _kResult = _vsStart3DSoundEngine();
		
		return (int) _kResult;
	}
}


