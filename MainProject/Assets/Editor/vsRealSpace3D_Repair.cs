// *******************************************************************************
// * Copyright (c) 2012, 2013, 2014 VisiSonics, Inc.
// * This software is the proprietary information of VisiSonics, Inc.
// * All Rights Reserved.
// ********************************************************************************

using UnityEngine;
using UnityEditor;
using System.IO;

public class vsRepair : MonoBehaviour
{
	[MenuItem("vsRealSpace3D/Repair")]
		
	private static void Init()
	{
		string sNotice = "Notice: You are repairing a vsRealSpace3D internal system file. Only perform this action if you consistently do not hear vsRealSpace3D sounds.";
		
		Debug.LogWarning(sNotice);
		
		if(EditorUtility.DisplayDialog("Repair vsRealSpace3D", sNotice, "Repair", "Do Not Repair") == true)
		{
			string sSource = 		Application.streamingAssetsPath + "/vsRealSpace3D/DontTouch/xifsdor/boutwave.xml";
			string sDestination = 	Application.streamingAssetsPath + "/vsRealSpace3D/DontTouch/boutwave.xml";
			FileUtil.ReplaceFile(sSource, sDestination);
		}
	}
}
