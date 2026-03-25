using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // ==============================================================================
    // Enum/Structs
    // ==============================================================================
    public enum NetworkState
    {
        local,
        Multi_Host,
        Multi_Guest,
        END
    }

    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================

    public bool Initialize(bool _isBlack, bool _isLocal)
    {
        isBlack = _isBlack;
        isLocal = _isLocal;
        arm.transform.Rotate(0.0f, -arm.transform.eulerAngles.y, 0.0f);
        if (!isBlack)
        {
            arm.transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        gameObject.SetActive(_isLocal || _isBlack);

        return true;
    }

    private void Start()
    {
        groundLayer = LayerMask.GetMask("Board");
        Hover = GameMaster.GetInstance().GetBoard().Hover;
    }

    private void Update()
    {
        if (selectedPiece != null)
        {
            selectedPiece.gameObject.transform.position = position;
            selectedPiece.HighlightTile();

            if (Mouse.current.rightButton.wasPressedThisFrame)
                Deselect();
            else if (Mouse.current.leftButton.wasPressedThisFrame)
                RequestMove();
        }
        else
        {
            if (Mouse.current.leftButton.wasPressedThisFrame) // 인풋 시스템이 fixed update에 들어가면 제대로 클릭 감지를 못함. fixed update 메커니즘 때문인듯
                Select();
        }
    }
    private void FixedUpdate()
    {
        Hovering();
    }

    // ==============================================================================
    // Methods
    // ==============================================================================


    private bool Raycast(out Tile output)
    {
        Vector3 mousePosition = Vector3.zero;
        Vector2 mouse = Mouse.current.position.ReadValue();
        mousePosition.x = mouse.x;
        mousePosition.y = mouse.y;
        output = null;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out RaycastHit hit, 100f, groundLayer))
        {
            if (hit.collider.CompareTag("Board"))
                output = hit.transform.gameObject.GetComponent<Tile>();
            position = hit.point;
            position.y = 1.0f;
        }

        return output != null;
    }

    private void RequestMove()
    {
        if (hoveringTile.GetState() != Tile.Status.Select)
            return;

        GameMaster.GetInstance().RequestMove(hoveringTile, selectedTile);
    }
        
    public void Place()
    {
        selectedPiece.DeHighlightTile();
        selectedPiece.TileSearch();
        selectedTile = null;
        selectedPiece = null;
    }

    private void Hovering()
    {
        if (!Raycast(out hoveringTile))
            return;

        Hover(hoveringTile);
    }

    private void Deselect()
    {
        selectedPiece.gameObject.transform.position = selectedTile.gameObject.transform.position;
        selectedPiece.DeHighlightTile();
        selectedTile = null;
        selectedPiece = null;
    }

    private void Select()
    {
        if (hoveringTile == null || hoveringTile.GetPiece() == null)
            return;
        if (!isLocal && hoveringTile.GetPiece().isBlack != isBlack)
            return;

        selectedTile = hoveringTile;
        selectedPiece = hoveringTile.GetPiece();

        hoveringTile.GetPiece().TileSearch();
    }


    [SerializeField] GameObject arm;
    [SerializeField] private Piece selectedPiece = null;
    [SerializeField] private Tile selectedTile = null;
    [SerializeField] private Tile hoveringTile = null;
    [SerializeField] Vector3 position;

    delegate void HoverPointer(Tile tile);
    private HoverPointer Hover;
    private bool isBlack;
    private bool isLocal;
    private int groundLayer;

}
