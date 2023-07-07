using UnityEngine;

/// <summary>
/// Component that manages cell data used by shaders.
/// </summary>
public class HexCellShaderData : MonoBehaviour {

	Texture2D cellTexture;

	Color32[] cellTextureData;

	bool[] visibilityTransitions;
	public HexGrid Grid { get; set; }

	/// <summary>
	/// Initialze the map data.
	/// </summary>
	/// <param name="x">Map X size.</param>
	/// <param name="z">Map Z size.</param>
	public void Initialize (int x, int z) {
		if (cellTexture) {
			cellTexture.Reinitialize(x, z);
		}
		else {
			cellTexture = new Texture2D(
				x, z, TextureFormat.RGBA32, false, true
			);
			cellTexture.filterMode = FilterMode.Point;
			cellTexture.wrapMode = TextureWrapMode.Clamp;
			Shader.SetGlobalTexture("_HexCellData", cellTexture);
		}
		Shader.SetGlobalVector(
			"_HexCellData_TexelSize",
			new Vector4(1f / x, 1f / z, x, z)
		);

		if (cellTextureData == null || cellTextureData.Length != x * z) {
			cellTextureData = new Color32[x * z];
			visibilityTransitions = new bool[x * z];
		}
		else {
			for (int i = 0; i < cellTextureData.Length; i++) {
				cellTextureData[i] = new Color32(0, 0, 0, 0);
				visibilityTransitions[i] = false;
			}
		}
		enabled = true;
	}

	/// <summary>
	/// Refresh the terrain data of a cell. Supports water surfaces up to 30 units high.
	/// </summary>
	/// <param name="cell">Cell with changed terrain type.</param>
	public void RefreshTerrain (HexCell cell) {
		Color32 data = cellTextureData[cell.Index];
		data.b = cell.IsUnderwater ? (byte)(cell.WaterSurfaceY * (255f / 30f)) : (byte)0;
		data.a = (byte)cell.TerrainTypeIndex;
		cellTextureData[cell.Index] = data;
		enabled = true;
	}

	/// <summary>
	/// Refresh visibility of a cell.
	/// </summary>
	/// <param name="cell">Cell with changed visibility.</param>
	public void RefreshVisibility (HexCell cell) {
		int index = cell.Index;
		cellTextureData[index].r = cell.IsVisible ? (byte)255 : (byte)0;
		cellTextureData[index].g = cell.IsExplored ? (byte)255 : (byte)0;
		enabled = true;
	}

	/// <summary>
	/// Set arbitrary map data of a cell, overriding water data.
	/// </summary>
	/// <param name="cell">Cell to apply data for.</param>
	/// <param name="data">Cell data value, 0-1 inclusive.</param>
	public void SetMapData (HexCell cell, float data) {
		cellTextureData[cell.Index].b =
			data < 0f ? (byte)0 : (data < 1f ? (byte)(data * 255f) : (byte)255);
		enabled = true;
	}

	void LateUpdate () {
		cellTexture.SetPixels32(cellTextureData);
		cellTexture.Apply();
	}

	bool UpdateCellData (HexCell cell, int delta) {
		int index = cell.Index;
		Color32 data = cellTextureData[index];
		bool stillUpdating = false;

		if (!stillUpdating) {
			visibilityTransitions[index] = false;
		}
		cellTextureData[index] = data;
		return stillUpdating;
	}
}