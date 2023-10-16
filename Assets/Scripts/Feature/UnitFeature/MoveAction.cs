using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MoveAction : BaseAction
{
	public event EventHandler StartMoving;

	public event EventHandler StopMoving;

	/// <summary>
	/// Travel along a path.
	/// </summary>
	/// <param name="path">List of cells that describe a valid path.</param>
	public void Travel(bool hasPath)
	{
		if (!hasPath)
        {
			return;
        }
		unit.location.unitFeature = null;
		unit.location = HexGrid.Instance.curPath[^1];
		unit.location.unitFeature = unit;
		StopAllCoroutines();
		StartBlockingCoroutine(TravelPath());
        RquestSyncMapLocationServerRpc(NetworkManager.Singleton.LocalClientId, unit.location.Coordinates.X, unit.location.Coordinates.Z, unit.GetComponent<NetworkObject>().NetworkObjectId);
    }

	public IEnumerator TravelPath()
	{
		StartMoving?.Invoke(this, EventArgs.Empty);
		Vector3 a, b, c = HexGrid.Instance.curPath[0].Position;
		yield return unit.TurnTo(HexGrid.Instance.curPath[1].Position);

		if (!unit.currentTravelLocation)
		{
			unit.currentTravelLocation = HexGrid.Instance.curPath[0];
		}
		int currentColumn = unit.currentTravelLocation.ColumnIndex;

		float t = Time.deltaTime * unit.travelSpeed;
		for (int i = 1; i < HexGrid.Instance.curPath.Count; i++)
		{
			unit.currentTravelLocation = HexGrid.Instance.curPath[i];
			a = c;
			b = HexGrid.Instance.curPath[i - 1].Position;

			int nextColumn = unit.currentTravelLocation.ColumnIndex;
			if (currentColumn != nextColumn)
			{
				//HexGrid.Instance.MakeChildOfColumn(transform, nextColumn);
				currentColumn = nextColumn;
			}

			c = (b + unit.currentTravelLocation.Position) * 0.5f;

			for (; t < 1f; t += Time.deltaTime * unit.travelSpeed)
			{
				transform.localPosition = Bezier.GetPoint(a, b, c, t);
				Vector3 d = Bezier.GetDerivative(a, b, c, t);
				d.y = 0f;
				transform.localRotation = Quaternion.LookRotation(d);
				yield return null;
			}
			t -= 1f;
		}
		unit.currentTravelLocation = null;

		a = c;
		b = unit.location.Position;
		c = b;
		for (; t < 1f; t += Time.deltaTime * unit.travelSpeed)
		{
			transform.localPosition = Bezier.GetPoint(a, b, c, t);
			Vector3 d = Bezier.GetDerivative(a, b, c, t);
			d.y = 0f;
			transform.localRotation = Quaternion.LookRotation(d);
			yield return null;
		}

		transform.localPosition = unit.location.Position;
		unit.orientation = transform.localRotation.eulerAngles.y;
		ListPool<HexCell>.Add(HexGrid.Instance.curPath);
		HexGrid.Instance.curPath = null;
		StopMoving?.Invoke(this, EventArgs.Empty);
	}

	public void DoMove()
	{
		if (HexGrid.Instance.HasPath)
		{
			unit.canMove = false;
			Travel(HexGrid.Instance.GetPath());
			HexGrid.Instance.ClearCellColor(Color.blue);
			HexGrid.Instance.ClearCellColor(Color.white);
		}
	}

    [ServerRpc(RequireOwnership = false)]
    public void RquestSyncMapLocationServerRpc(ulong clientId, int x, int z, ulong objId)
    {
		if (clientId == NetworkManager.Singleton.ConnectedClients[0].ClientId)
        {
			SyncMapLocationClientRpc(x, z, objId, GameManagerClient.Instance.clientRpcParams);
		}
		else
		{
			SyncMapLocationClientRpc(x, z, objId, GameManagerClient.Instance.HostRpcParams);
		}
    }

	[ClientRpc]
	public void SyncMapLocationClientRpc(int x, int z, ulong objId, ClientRpcParams clientRpcParams = default)
	{
		Debug.Log("SyncMapLocationClientRpc");
		HexCoordinates coordinates = new(x, z);
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects[objId].TryGetComponent<HexUnit>(out var target))
        {
            Debug.LogError("Hexunit is null");
        }
		target.location.unitFeature = null;
		target.location = HexGrid.Instance.GetCell(coordinates);
		target.location.unitFeature = target;
    }
}
