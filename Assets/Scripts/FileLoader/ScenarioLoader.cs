﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Xml;

public class ScenarioLoader {

	private XmlDocument xmlDoc = new XmlDocument();
	private string directoryName;


	public ScenarioLoader(string filepath) {
		if (!System.IO.File.Exists (filepath)) {
			Debug.LogError ("Error: scenario file " + filepath + " not found.");
		} else {
			xmlDoc.LoadXml (System.IO.File.ReadAllText (filepath));
			directoryName = Path.GetDirectoryName (filepath);
		}
	}

	public string getTrajectoryFilePath() {
		XmlNode output = xmlDoc.SelectSingleNode("//output");
		string trajectoryFilePath = "";
		foreach(XmlElement floor in output.SelectNodes("floor")) { // TODO more floors
			trajectoryFilePath = directoryName + "/" + floor.GetAttribute("csvAt");
		}
		return trajectoryFilePath;
	}
		
	public void loadScenario() {
		XmlNode spatial = xmlDoc.SelectSingleNode("//spatial");
		foreach(XmlElement floor in spatial.SelectNodes("floor")) { // TODO: more floors
			float height = 1f; // TryParseWithDefault.ToSingle(floor.GetAttribute("height"), 1.0f);
			foreach (XmlElement collection in floor.SelectNodes("collection")) { // that's the new element in the XML format, added by DrGeli
				foreach (XmlElement geomObj in collection.SelectNodes("object")) {
					string name = collection.GetAttribute ("id") + "_" + geomObj.GetAttribute ("name");
					switch (geomObj.GetAttribute("type")) {
						case "openWall":
							WallExtrudeGeometry.create(name, parsePoints(geomObj), height, -0.2f);
							break;
						case "wall":
							ObstacleExtrudeGeometry.create(name, parsePoints(geomObj), height);
							break;
						case "origin":
						case "destination":
						case "scaledArea":
						case "waitingZone":
						case "portal":
						case "beamExit":
						case "eofWall":
						case "queuingArea":
							//AreaGeometry.createOriginTarget(geomObj.GetAttribute("name"), parsePoints(geomObj));
							break;
						default:
							Debug.Log("Warning: XML geometry parser: Don't know how to parse Object of type '" + geomObj.GetAttribute("type") + "'.");
							break;
					}
				}
			}
		}

	}

	/*
	static float minX = float.MaxValue; 
	static float maxX = 0;
	static float minY = float.MaxValue;
	static float maxY = 0;
	*/

	// Parse an XmlElement full of <point> XmlElements into a coordinate list 
	static List<Vector2> parsePoints(XmlElement polyPoints) {
		List<Vector2> list = new List<Vector2>();
		foreach(XmlElement point in polyPoints.SelectNodes("point")) {
			float x;
			float y;
			if (float.TryParse(point.GetAttribute("x"), out x) && float.TryParse(point.GetAttribute("y"), out y)) {
				list.Add(new Vector2(x, y));
			}
			/*
			if (x < minX)
				minX = x;
			if (x > maxX)
				maxX = x;
			if (y < minY)
				minY = y;
			if(y > maxY)
				maxY = y;
			*/
		}
		return list;
	}

	/*
	static string listToString(List<Vector2> list) {
		string str = "";
		foreach (Vector2 v in list) {
			str += "x:" + v.x + "/y:" + v.y + "_";
		}
		return str;
	}
	*/
}
