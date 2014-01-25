/*using UnityEditor;
using UnityEngine;
using System.Collections;
 
[CustomEditor( typeof( PlayerInput ) )]
public class PropertiesEditorExtension : Editor {
	
	PlayerInput m_Instance;
	PropertyField[] m_fields;
 
	public void OnEnable()
	{
		m_Instance = (PlayerInput)target;
		m_fields = ExposeProperties.GetProperties( m_Instance );
	}
 
	public override void OnInspectorGUI () {
 
		if ( m_Instance == null )
			return;
 
		this.DrawDefaultInspector();
 
		ExposeProperties.Expose( m_fields );
 
	}
}*/