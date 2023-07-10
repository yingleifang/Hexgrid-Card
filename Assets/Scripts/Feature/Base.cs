using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Base : Feature
{
    public static Base basePrefab;

	public static void Load(BinaryReader reader, HexGrid grid)
	{
		HexCoordinates coordinates = HexCoordinates.Load(reader);
		float orientation = reader.ReadSingle();
		grid.AddFeature(
			Instantiate(basePrefab), grid.GetCell(coordinates), orientation
		);
	}
}
