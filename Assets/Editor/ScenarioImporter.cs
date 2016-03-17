﻿using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Collections;

public class ScenarioImporter {

	[MenuItem("Assets/Import accu:rate output")]
	
	static void importAccurateOutput() {
		if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ())
			return;

		bool continueOk = true;

		if (EditorSceneManager.GetActiveScene().name == "Base") 
			continueOk = !EditorUtility.DisplayDialog("duplicate scene", "It is recommend that you first duplicate the Scene (select it in the Scenes folder and use Edit > Duplicate), rename it optionally and doubleclick the new scene.", "ok, let me duplicate", "continue");

		if (continueOk) {
			var path = EditorUtility.OpenFilePanel ("", Application.dataPath + "/Resources/Data", "xml"); //Path.GetFileName(path))

			if (path == "") // = cancel was clicked in open file dialog
				return;

			new GameObject("World");
			new GameObject ("Pedestrians");

			RuntimeInitializer ri = GameObject.Find("RuntimeInitializer").GetComponent<RuntimeInitializer>();
			ri.geometryLoader = GameObject.Find("GeometryLoader").GetComponent<GeometryLoader>();
			ri.geometryLoader.setTheme (new MarketplaceThemingMode ()); // EvaktischThemingMode ());

			ScenarioLoader sl = new ScenarioLoader (path);
			sl.loadScenario ();
			ri.relativeTrajFilePath = sl.getRelativeTrajFilePath ();
		}
	}

	[MenuItem("Assets/dev")]

	static void dev() {
		/* this is the name the app gets on an android smartphone */
		//PlayerSettings.productName = "app_name";
		//Debug.Log (PlayerSettings.productName);

		/* to undo the texture-stretching on a cube that stretching the cube caused */
		//ReCalcCubeTexture script = GameObject.Find("Cube").GetComponent<ReCalcCubeTexture>();
		//script.reCalcCubeTexture ();

		/*
		string dir = "Assets/Resources/Data/_ignore/.../";
		FileInfo fi = new FileInfo (dir + "floor-0.csv");
		StreamReader reader = fi.OpenText ();
		StreamWriter writer = new StreamWriter(dir + "floor-0_reduced.csv", false);
		using (reader) {
			string line;
			int i = 0;
			while((line = reader.ReadLine()) != null && i++ < 100000) {
				writer.WriteLine (line); 
			}
			reader.Close ();
			writer.Close();
		}
		Debug.Log ("DONE");
		*/
	}
}
