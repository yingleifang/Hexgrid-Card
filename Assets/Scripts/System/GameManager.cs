using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	string currentMap = "Alpha1";

	[SerializeField]
	HexGrid hexGrid;

	const int mapFileVersion = 5;

    private void Awake()
    {
		Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
	}

    // Start is called before the first frame update
    void Start()
	{
		Load(GetSelectedPath());
	}

	// Map file name needs to match scene name
	string GetSelectedPath()
	{
		return Path.Combine(Application.persistentDataPath, currentMap + ".map");
	}
	void Load(string path)
	{
		if (!File.Exists(path))
		{
			Debug.LogError("File does not exist " + path);
			return;
		}
		using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
		{
			int header = reader.ReadInt32();
			if (header <= mapFileVersion)
			{
				hexGrid.Load(reader, header);
				HexMapCamera.ValidatePosition();
			}
			else
			{
				Debug.LogWarning("Unknown map format " + header);
			}
		}
	}
}
