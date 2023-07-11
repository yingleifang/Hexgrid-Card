using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Component that manages the game UI.
/// </summary>
public class HexGameUI : MonoBehaviour {

	[SerializeField]
	HexGrid grid;

	HexCell currentCell;

	Feature selectedFeature;

	/// <summary>
	/// Set whether map edit mode is active.
	/// </summary>
	/// <param name="toggle">Whether edit mode is enabled.</param>
	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		grid.ShowUI(!toggle);
		//grid.ClearPath();
	}

	//void Update () {
	//	if (!EventSystem.current.IsPointerOverGameObject()) {
	//		if (Input.GetMouseButtonDown(0)) {
	//			DoSelection();
	//		}
	//		else if (selectedFeature) {
	//			if (Input.GetMouseButtonDown(1) && selectedFeature is HexUnit) {
	//				DoMove();
	//			}
	//			else {
	//				DoPathfinding();
	//			}
	//		}
	//	}
	//}

	//void DoSelection () {
	//	grid.ClearPath();
	//	UpdateCurrentCell();
	//	if (currentCell) {
	//		selectedFeature = currentCell.Feature;
	//	}
	//}

	//void DoPathfinding () {
	//	if (UpdateCurrentCell()) {
	//		if (currentCell && ((HexUnit)selectedFeature).IsValidDestination(currentCell)) {
	//			grid.FindPath(selectedFeature.Location, currentCell, (HexUnit)selectedFeature);
	//		}
	//		else {
	//			grid.ClearPath();
	//		}
	//	}
	//}

	//void DoMove () {
	//	if (grid.HasPath) {
	//		((HexUnit)selectedFeature).Travel(grid.GetPath());
	//		grid.ClearPath();
	//	}
	//}

	bool UpdateCurrentCell () {
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}
}