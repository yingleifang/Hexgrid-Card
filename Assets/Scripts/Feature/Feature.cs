using UnityEngine;
using System.IO;

public class Feature : MonoBehaviour
{
    public HexGrid Grid { get; set; }

	protected float orientation;

	protected HexCell location;
	/// <summary>
	/// Cell that the unit occupies.
	/// </summary>
	public HexCell Location
	{
		get => location;
		set
		{
			if (location)
			{
				location.Feature = null;
			}
			location = value;
			value.Feature = this;
			transform.localPosition = value.Position;
			Grid.MakeChildOfColumn(transform, value.ColumnIndex);
		}
	}

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
}
