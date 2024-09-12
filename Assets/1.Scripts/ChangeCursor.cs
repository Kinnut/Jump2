using UnityEngine;

public class ChangeCursor : MonoBehaviour
{
    public Texture2D cursorTexture;

    void Start()
    {
        Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.ForceSoftware);
    }
}
