using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    // ==============================================================================
    // Enum/Structs
    // ==============================================================================
    public enum Status
    {
        Highlight,
        Select,
        None
    }

    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    public void Initialize(int x, int y)
    {
        if (initiated)
            return;

        position.x = x;
        position.y = y;
        initiated = true;

#if UNITY_SERVER
#else
        modelRenderer = GetComponent<Renderer>();
        SetRender();
#endif
    }

    // ==============================================================================
    // Methods
    // ==============================================================================
    private void SetRender()
    {
#if UNITY_SERVER
        return;
#endif
        switch (status)
        {
            case Status.Highlight:
                modelRenderer.enabled = true;
                modelRenderer.material = highlightMaterial;
                break;
            case Status.Select:
                modelRenderer.enabled = true;
                modelRenderer.material = SelectableMaterial;
                break;
            case Status.None:
                modelRenderer.enabled = false;
                break;
            default:
                status = Status.None;
                modelRenderer.enabled = false;
                break;
        }
    }


    public void ReleasePiece()
    {
        if (piece != null)
            piece.Release();

        piece = null;
    }

    // ==============================================================================
    // variable & GetSet Methods
    // ==============================================================================

    public Vector2Int GetPosition() { return position; }
    public Piece GetPiece() { return piece; }
    public Status GetState() { return status; }
    public void SetPiece(Piece next) { piece = next; }
    public void SetState(Status next) { if (status == next) return; status = next; SetRender(); }

    private bool initiated = false;
    private Vector2Int position;
    private Piece piece = null;
    private Renderer modelRenderer = null;
    private Status status = Status.None;

    [SerializeField] private Material highlightMaterial = null;
    [SerializeField] private Material SelectableMaterial = null;
}
