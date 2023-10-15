using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

/// <summary>
/// Component that applies actions from the save-load menu UI to the hex map.
/// Public methods are hooked up to the in-game UI.
/// </summary>
public class SaveLoadMenu : MonoBehaviour {

	const int mapFileVersion = 5;

	[SerializeField]
	Text menuLabel, actionButtonLabel;

	[SerializeField]
	InputField nameInput;

	[SerializeField]
	RectTransform listContent;

	[SerializeField]
	SaveLoadItem itemPrefab;

	[SerializeField]
	HexGrid hexGrid;

	bool saveMode;

    public void Open (bool saveMode) {
		this.saveMode = saveMode;
		if (saveMode) {
			menuLabel.text = "Save Map";
			actionButtonLabel.text = "Save";
		}
		else {
			menuLabel.text = "Load Map";
			actionButtonLabel.text = "Load";
		}
		FillList();
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}

	public void Close () {
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
	}

	public void Action () {
		string mapPath = GetSelectedPath(HexGrid.Instance.mapSuffix);
		string featurePath = GetSelectedPath(HexGrid.Instance.featureSuffix);
		if (mapPath == null || featurePath == null) {
			Debug.LogError("No map name entered!");
			return;
		}
		if (saveMode) {
			SaveMap(mapPath);
			SaveFeature(featurePath);
		}
		else {
			LoadMap(mapPath);
			LoadFeature(featurePath);
		}
		Close();
	}

    public void SelectItem (string name) => nameInput.text = name;

	public void Delete () {
		string path = GetSelectedPath(HexGrid.Instance.mapSuffix);
		if (path == null) {
			return;
		}
		if (File.Exists(path)) {
			File.Delete(path);
		}
        path = GetSelectedPath(HexGrid.Instance.featureSuffix);
        if (path == null)
        {
            return;
        }
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        nameInput.text = "";
		FillList();
	}

	void FillList () {
		for (int i = 0; i < listContent.childCount; i++) {
			Destroy(listContent.GetChild(i).gameObject);
		}
		string[] paths =
			Directory.GetFiles(Application.persistentDataPath, "*" + HexGrid.Instance.mapSuffix);
		for (int i = 0; i < paths.Length; i++) {
			SaveLoadItem item = Instantiate(itemPrefab);
			item.Menu = this;
			item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
			item.transform.SetParent(listContent, false);
		}
	}

	string GetSelectedPath (string fileSuffix) {
		string mapName = nameInput.text;
		if (mapName.Length == 0) {
			return null;
		}
		return Path.Combine(Application.persistentDataPath, mapName + fileSuffix);
	}

	void SaveMap (string path) {
		using (
			BinaryWriter writer =
			new BinaryWriter(File.Open(path, FileMode.Create))
		) {
			writer.Write(mapFileVersion);
			hexGrid.SaveMap(writer);
		}
    }
    void SaveFeature(string path)
    {
        using (
            BinaryWriter writer =
            new BinaryWriter(File.Open(path, FileMode.Create))
        )
        {
            writer.Write(mapFileVersion);
            hexGrid.SaveFeature(writer);
        }
    }

    void LoadMap (string path) {
		if (!File.Exists(path)) {
			Debug.LogError("File does not exist " + path);
			return;
		}
		using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
			int header = reader.ReadInt32();
			if (header <= mapFileVersion) {
				hexGrid.LoadMap(reader, header);
				HexMapCamera.ValidatePosition();
			}
			else {
				Debug.LogWarning("Unknown map format " + header);
			}
		}
	}
    private void LoadFeature(string path)
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
                hexGrid.LoadFeature(reader, header);
                hexGrid.LoadFeature(reader, header);
            }
            else
            {
                Debug.LogWarning("Unknown map format " + header);
            }
        }
    }

}