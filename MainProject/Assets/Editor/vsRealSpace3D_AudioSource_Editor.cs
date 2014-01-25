// *******************************************************************************
// * Copyright (c) 2012, 2013, 2014 VisiSonics, Inc.
// * This software is the proprietary information of VisiSonics, Inc.
// * All Rights Reserved.
// ********************************************************************************

// 
// custom gui for the vsRealSpace3D_AudioSource
//

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(vsRealSpace3D_AudioSource))]
[CanEditMultipleObjects]

/// <summary>
/// Editor class for the vsRealSpace3D AudioSource
/// </summary>
public class vsRealSpace3D_AudioSource_Editor : Editor {
	
	bool bShowSoundOptions =true;
	bool bFromTime = 		true;
	bool bPlaySoundGroup = 	true;
	bool bToTime = 			true;
	
	private Texture2D 		vsLogo;
	
	/// <summary>
	/// Raises the inspector GU event. Provides GUI and handles the inputs for the 
	/// vsRealSpace3D AudioSource.
	/// </summary>
	public override void OnInspectorGUI() {
		
		GUI.changed = 	false;
		
		int	_nHours = 	new int();
		int	_nMins = 	new int();
		int	_nSecs = 	new int();
			
		// grab the AudioSource to edit
		vsRealSpace3D_AudioSource _theTarget = (vsRealSpace3D_AudioSource) target;
		
		// pimp ourself
		vsLogo = (Texture2D) Resources.LoadAssetAtPath("Assets/Editor/Images/visisonics.jpg", typeof(Texture2D));
		
		if(!vsLogo)
			Debug.LogError("Missing Texture...in OnInspectorGUI");
		else
			GUILayout.Label(vsLogo);		
		
		// grabs the audio clip
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(new GUIContent("Audio Clip", "Select audio clip from StreamingAssets/Sound3D"));	
			_theTarget.theAudioClip = 	(AudioClip) EditorGUILayout.ObjectField(_theTarget.theAudioClip, typeof(AudioClip), true);		
		EditorGUILayout.EndHorizontal();
		
		// check to see if the audio clip has been entered
		if(_theTarget.theAudioClip.Equals("None (AudioClip)"))
		{
			Debug.LogError("The Audio Clip is empty. Please select an AudioClip");
		}
		else
		{		
			_theTarget.sAudioClipPath = AssetDatabase.GetAssetPath(_theTarget.theAudioClip);		
			ConvertAudioLength(_theTarget.theAudioClip.length, out _nHours, out _nMins, out _nSecs);		
		}
		
		TimeSpan _theLength = 			new TimeSpan(_nHours, _nMins, _nSecs);
		
		_theTarget.sAudioLength = 		EditorGUILayout.TextField(new GUIContent("Audio Length", "Time length of selected sound clip."), _theLength.ToString());
			
		_theTarget.sSoundSourceTag = 	EditorGUILayout.TextField(new GUIContent("Sound Source Tag", "Provide a unique descriptor name for the sound source."), _theTarget.sSoundSourceTag);

		// check to see if the sound source tag has been entered				
		if(_theTarget.sSoundSourceTag.Equals(""))
		{
			Debug.LogError("The SoundSourceTag is not set.");							
		}
		
//reh 08oct13 tbd later	
		if(_theTarget.vsIsTagInUse())
		{
			EditorUtility.DisplayDialog("WARNING", "The Sound Source Tag: " + "\"" + _theTarget.sSoundSourceTag + "\"" + " is currently in use." + "\n" + "Do you wish to overwrite it?" + "\n" + "If not, cancel and enter a new tag name.", "Overwrite", "Cancel");
		}
		
		_theTarget.fVolume = 	EditorGUILayout.Slider(new GUIContent("Volume", "0.0 - 10.0"), _theTarget.fVolume, 0.0f, 10.0f);

		bShowSoundOptions = 	EditorGUILayout.Foldout(bShowSoundOptions, "Sound Options"); 
		
		if(bShowSoundOptions)
		{
			if(Selection.activeTransform)
			{
				_theTarget.bMute = 			EditorGUILayout.Toggle(new GUIContent("Mute", "Mutes the sound"), _theTarget.bMute);
				_theTarget.bPlayOnStart = 	EditorGUILayout.Toggle(new GUIContent("Play On Start", "Play the sound when the scene starts"),_theTarget.bPlayOnStart);
				_theTarget.bPlayOnTrigger =	EditorGUILayout.Toggle(new GUIContent("Play On Trigger", "Play the sound on user defined trigger"), _theTarget.bPlayOnTrigger);
				
				DoSoundOptions(ref _theTarget);			
			}	
		}
		
		// display the Advance settings
		if(GUI.changed)
		{
			EditorUtility.SetDirty(_theTarget);
		}
	}
	
	private void ConvertAudioLength(float _fAudioLength, out int _nHours, out int _nMins, out int _nSecs)
	{
		_nSecs = (int)  _fAudioLength % 60;
		_nMins = (int) (_fAudioLength / 60) % 60;
		_nHours =(int)  _fAudioLength / 3600;
	}
	
	private void DoSoundOptions(ref vsRealSpace3D_AudioSource _theTarget) 
	{
		_theTarget.bDurationSelected = EditorGUILayout.BeginToggleGroup("Sound Duration", _theTarget.bDurationSelected);

		// reset the loop toggle if duration isn't selected
		if(!bPlaySoundGroup)
		{
			_theTarget.bLoop = false;
			return;
		}
			
		// handle sound loop
		_theTarget.bLoop = EditorGUILayout.Toggle(new GUIContent("Loop", "Play the sound continuously"), _theTarget.bLoop);
		
		// do if loop isn't selected
		if(!_theTarget.bLoop)
		{
			bFromTime = EditorGUILayout.Foldout(bFromTime, new GUIContent("From Time", "Starting play length time.")); 
	
			if(bFromTime)
			{
				if(Selection.activeTransform)
				{
					// handle the from time
					EditorGUILayout.BeginVertical();
						_theTarget.tFromTime.nHours = 	EditorGUILayout.IntField("Hour:", _theTarget.tFromTime.nHours);
						_theTarget.tFromTime.nMinutes = EditorGUILayout.IntField("Minutes:", _theTarget.tFromTime.nMinutes);
						_theTarget.tFromTime.nSeconds = EditorGUILayout.IntField("Seconds:", _theTarget.tFromTime.nSeconds);
					EditorGUILayout.EndVertical();					
				}
			}
			
			bToTime = EditorGUILayout.Foldout(bToTime, new GUIContent("To Time", "Ending play length time.")); 
	
			if(bToTime)
			{
				if(Selection.activeTransform)
				{
					// handle the from time
					EditorGUILayout.BeginVertical();
						_theTarget.tToTime.nHours =	  EditorGUILayout.IntField("Hour:", _theTarget.tToTime.nHours);
						_theTarget.tToTime.nMinutes = EditorGUILayout.IntField("Minutes:", _theTarget.tToTime.nMinutes);
						_theTarget.tToTime.nSeconds = EditorGUILayout.IntField("Seconds:", _theTarget.tToTime.nSeconds);
					EditorGUILayout.EndVertical();	
				}
			}			
		}
		
		EditorGUILayout.EndToggleGroup();
	}
}