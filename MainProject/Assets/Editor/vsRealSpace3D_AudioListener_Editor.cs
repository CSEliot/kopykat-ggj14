// *******************************************************************************
// * Copyright (c) 2012, 2013, 2014 VisiSonics, Inc.
// * This software is the proprietary information of VisiSonics, Inc.
// * All Rights Reserved.
// ********************************************************************************

// 
// custom gui for the vsRealSpace3D_AudioListener
//

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(vsRealSpace3D_AudioListener))]
[CanEditMultipleObjects]

/// <summary>
/// Editor class for vsRealSpace3D AudioListener
/// </summary>
public class vsRealSpace3D_AudioListener_Editor : Editor {
	
	bool bShowAdvance = true;
	
	private Texture2D vsLogo;
	
	/// <summary>
	/// Raises the inspector GU event. Provides the GUI and handles the inputs
	/// for the vsRealSpace3D AudioListener
	/// </summary>
	public override void OnInspectorGUI() {
		
		GUI.changed = false;
		
		// grab the AudioListener to edit
		vsRealSpace3D_AudioListener _theTarget = (vsRealSpace3D_AudioListener) target;
		
		// pimp ourself
		vsLogo = (Texture2D) Resources.LoadAssetAtPath("Assets/Editor/Images/visisonics.jpg", typeof(Texture2D));
		
		if(!vsLogo)
			Debug.LogError("Missing Texture...in OnInspectorGUI");
		else
			GUILayout.Label(vsLogo);		
		
		_theTarget.theWorldSize = EditorGUILayout.Vector3Field("World Size", _theTarget.theWorldSize);			

		if(_theTarget.theWorldSize.x == 0.0f && _theTarget.theWorldSize.y == 0.0f && _theTarget.theWorldSize.z == 0.0f)
		{
			Debug.LogError("The World Size is set to 0.0f. Please enter your world size.");
		}
		
		
		//
		// do bounds limits on the worldsize
		//
		if(_theTarget.theWorldSize.x < 10.0f)
		{
			Debug.LogWarning("The World Size has to be at least 10.0 meters.");
			_theTarget.theWorldSize.x = 10.0f;
		}
		if(_theTarget.theWorldSize.x > 1000.0f)
		{
			Debug.LogWarning("The World Size maximum is 1000.0 meters.");
			_theTarget.theWorldSize.x = 1000.0f;
		}

		if(_theTarget.theWorldSize.y < 10.0f)
		{
			Debug.LogWarning("The World Size has to be at least 10.0 meters.");
			_theTarget.theWorldSize.y = 10.0f;
		}
		if(_theTarget.theWorldSize.y > 1000.0f)
		{
			Debug.LogWarning("The World Size maximum is 1000.0 meters.");
			_theTarget.theWorldSize.y = 1000.0f;
		}
		
		if(_theTarget.theWorldSize.z < 10.0f)
		{
			Debug.LogWarning("The World Size has to be at least 10.0 meters.");
			_theTarget.theWorldSize.z = 10.0f;
		}
		if(_theTarget.theWorldSize.z > 1000.0f)
		{
			Debug.LogWarning("The World Size maximum is 1000.0 meters.");
			_theTarget.theWorldSize.z = 1000.0f;
		}
		
		_theTarget.theListenerDistance = EditorGUILayout.Slider(new GUIContent("Listener Radius", "1.0 - 100.0"), _theTarget.theListenerDistance, 1.0f, 100.0f);
		
		//
		// display the Advance settings
		//
		
		bShowAdvance = EditorGUILayout.Foldout(bShowAdvance, "3D Sound Advance Settings"); 
		
		if(bShowAdvance)
		{
			if(Selection.activeTransform)
			{
				// handle the hrtf settings
				_theTarget.hrtfState = (vsRealSpace3D_AudioListener.HRTFTypeState) EditorGUILayout.EnumPopup(new GUIContent("Personalize HRTF", "Select HRTF type"), _theTarget.hrtfState);
				
				if(_theTarget.hrtfState == vsRealSpace3D_AudioListener.HRTFTypeState.HRTF_Personalize)
				{
					// handle head, torso, neck size 
					EditorGUILayout.BeginVertical();				
						_theTarget.fHeadRadius = 	EditorGUILayout.Slider(new GUIContent("Head Radius", "[0.06 - 0.12m]"), _theTarget.fHeadRadius, 0.06f, 0.12f);
						_theTarget.fTorsoRadius =	EditorGUILayout.Slider(new GUIContent("Torso Radius", "[0.12 - 0.32m]"), _theTarget.fTorsoRadius, 0.12f, 0.32f);
						_theTarget.fNeckHeight =	EditorGUILayout.Slider(new GUIContent("Neck Height", "[0.00 - 0.08m]"), _theTarget.fNeckHeight, 0.0f, 0.08f);
					EditorGUILayout.EndVertical();
				}
				
				// handle the reverb settings
				_theTarget.reverbState = (vsRealSpace3D_AudioListener.REVERBTypeState) EditorGUILayout.EnumPopup(new GUIContent("Reverberation", "Select Reverb type"), _theTarget.reverbState);
			}
		}
		
		if(GUI.changed)
			EditorUtility.SetDirty(_theTarget);
	}
}
