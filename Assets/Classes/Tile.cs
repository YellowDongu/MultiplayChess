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
        modelRenderer = GetComponent<Renderer>();
        position.x = x;
        position.y = y;
        initiated = true;
        SetRender();
    }

    // ==============================================================================
    // Unity FrameCycle Methods
    // ==============================================================================

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (status == Status.None)
                status = Status.Highlight;
            else
                status = Status.None;
            SetRender();
        }
    }


    // ==============================================================================
    // Methods
    // ==============================================================================
    private void SetRender()
    {
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

    public bool SetPiece(Piece next)
    {
        if (next == null)
            return false;

        if (piece != null)
        {
            if (next.isBlack == piece.isBlack)
                return false;

            piece.Release();
        }

        piece = next;
        piece.gameObject.transform.position = gameObject.transform.position;
        piece.SetPosition(this);
        return true;
    }

    public void ReleasePiece()
    {
        if (piece != null)
            piece.Release();
        piece = null;
    }

    // ==============================================================================
    // variable GetSet Methods
    // ==============================================================================

    public Vector2Int GetPosition() { return position; }
    public Piece GetPiece() { return piece; }
    public Status GetState() { return status; }
    public void SetState(Status next) { if (status == next) return; status = next; SetRender(); }

    // ==============================================================================
    // variables
    // ==============================================================================

    private bool initiated = false;
    private Vector2Int position;
    private Piece piece = null;
    private Renderer modelRenderer = null;
    private Status status = Status.None;

    [SerializeField] private Material highlightMaterial = null;
    [SerializeField] private Material SelectableMaterial = null;
}
