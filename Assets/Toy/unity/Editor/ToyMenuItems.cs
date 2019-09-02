using System.IO;
using UnityEngine;
using UnityEditor;

public class ToyMenuItems {
	[MenuItem("Assets/Create/Toy Script")]
	private static void AddToyScript() {
		//doesn't have the default "renaming" functionality because I spent an hour searching for it and couldn't find it
		string fpath = AssetDatabase.GetAssetPath(Selection.activeObject) + "/NewToyScript.toy";

		StreamWriter streamWriter = new StreamWriter(fpath);
		streamWriter.Close();

		AssetDatabase.ImportAsset(fpath);
	}
}
