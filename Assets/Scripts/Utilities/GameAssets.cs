using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance { get; private set; }
	public Texture2D mainCursor;
	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("There's more than one GameAssets!");
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public Transform pfDamagePopup;
}
