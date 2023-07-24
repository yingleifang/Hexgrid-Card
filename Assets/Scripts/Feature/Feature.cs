using UnityEngine;
using System.IO;
using System;

public class Feature : MonoBehaviour
{
	public float orientation;

	public HexCell location;

	public FeatureSelectedVisuals featureSelectedVisuals;

	public event EventHandler FeatureSelected;

	public event EventHandler FeatureDeSelected;


	private void Awake()
    {
		featureSelectedVisuals = GetComponentInChildren<FeatureSelectedVisuals>();
	}

    /// <summary>
    /// Cell that the unit occupies.
    /// </summary>
    public virtual HexCell Location
	{ get; set; }

	/// <summary>
	/// Orientation that the unit is facing.
	/// </summary>
	public float Orientation
	{
		get => orientation;
		set
		{
			orientation = value;
			transform.localRotation = Quaternion.Euler(0f, value, 0f);
		}
	}

	/// <summary>
	/// Save the unit data.
	/// </summary>
	/// <param name="writer"><see cref="BinaryWriter"/> to use.</param>
	public void Save(BinaryWriter writer)
	{
		location.Coordinates.Save(writer);
		writer.Write(orientation);
	}

	/// <summary>
	/// Validate the position of the unit.
	/// </summary>
	public void ValidateLocation() => transform.localPosition = location.Position;

	public static void Load(BinaryReader reader, HexGrid grid, Feature feature)
	{
		HexCoordinates coordinates = HexCoordinates.Load(reader);
		float orientation = reader.ReadSingle();
		grid.AddFeature(
			Instantiate(feature), grid.GetCell(coordinates), orientation
		);
	}

	public void RaiseFeatureSelectedEvent()
	{
		FeatureSelected?.Invoke(this, EventArgs.Empty);
	}

	public void RaiseFeatureDeSelectedEvent()
	{
		FeatureDeSelected?.Invoke(this, EventArgs.Empty);
	}
}
