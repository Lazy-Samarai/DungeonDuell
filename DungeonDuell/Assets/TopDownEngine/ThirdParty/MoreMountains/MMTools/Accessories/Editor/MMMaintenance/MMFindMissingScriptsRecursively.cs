﻿// Original FindMissingScriptsRecursively script by SimTex and Clement
// http://wiki.unity3d.com/index.php?title=FindMissingScripts

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MoreMountains.Tools
{
	public class MMFindMissingScriptsRecursively : EditorWindow 
	{
		static int go_count = 0, components_count = 0, missing_count = 0;
	 
		[MenuItem("Tools/More Mountains/Find missing scripts recursively", false, 505)]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(MMFindMissingScriptsRecursively));
		}
	 
		#if  UNITY_EDITOR
		public void OnGUI()
		{
			if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
			{
				FindInSelected();
			}
		}
		#endif
		
		private static void FindInSelected()
		{
			GameObject[] go = Selection.gameObjects;
			go_count = 0;
			components_count = 0;
			missing_count = 0;
			foreach (GameObject g in go)
			{
				FindInGO(g);
			}
		}
	 
		private static void FindInGO(GameObject g)
		{
			go_count++;
			Component[] components = g.GetComponents<Component>();
			for (int i = 0; i < components.Length; i++)
			{
				components_count++;
				if (components[i] == null)
				{
					missing_count++;
					string s = g.name;
					Transform t = g.transform;
					while (t.parent != null) 
					{
						s = t.parent.name +"/"+s;
						t = t.parent;
					}
					Debug.Log (s + " has an empty script attached in position: " + i, g);
				}
			}
			// Now recurse through each child GO (if there are any):
			foreach (Transform childT in g.transform)
			{
				FindInGO(childT.gameObject);
			}
		}
	}
}
#endif