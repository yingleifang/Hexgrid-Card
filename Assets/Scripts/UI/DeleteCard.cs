using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteCard : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;
    Vector2 cursorHotspot;
    // Start is called before the first frame update
    void Start()
    {
        cursorHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
    }

    public void ChangeCursorToDelete()
    {
        GameManager.Instance.deleteMode = true;
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }
}
