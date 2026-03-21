using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public enum NetworkState
    {
        local,
        Multi_Host,
        Multi_Guest,
        END
    }

    public bool Initialize(bool _isBlack)
    {
        isBlack = _isBlack;
        if (!isBlack)
        {
            GameObject arm = GameObject.Find("Arm");
            arm.transform.Rotate(0.0f, 180.0f, 0.0f);
        }


        return true;
    }

    private void Start()
    {
        groundLayer = LayerMask.GetMask("Board");
        Hover = GameMaster.GetInstance().GetBoard().Hover;
        PlacePiece = GameMaster.GetInstance().Place;
        Initialize(false);
    }

    private void Update()
    {
        if (SelectedPiece != null)
        {
            SelectedPiece.gameObject.transform.position = position;
            SelectedPiece.HighlightTile();

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
        if (HoveringTile.GetState() != Tile.Status.Select)
            return;

        GameMaster.GetInstance().RequestMove(HoveringTile, SelectedTile);
    }

    public void Place()
    {
        SelectedPiece.DeHighlightTile();
        SelectedPiece.TileSearch();
        SelectedTile = null;
        SelectedPiece = null;
    }

    private void Hovering()
    {
        if (!Raycast(out HoveringTile))
            return;

        Hover(HoveringTile);
    }

    private void Deselect()
    {
        SelectedPiece.gameObject.transform.position = SelectedTile.gameObject.transform.position;
        SelectedPiece.DeHighlightTile();
        SelectedTile = null;
        SelectedPiece = null;
    }
    private void Select()
    {
        if (HoveringTile == null || HoveringTile.GetPiece() == null)
            return;
        SelectedTile = HoveringTile;
        SelectedPiece = HoveringTile.GetPiece();

        HoveringTile.GetPiece().TileSearch();
    }



    [SerializeField] private Piece SelectedPiece = null;
    [SerializeField] private Tile SelectedTile = null;
    [SerializeField] private Tile HoveringTile = null;
    [SerializeField] Vector3 position;

    delegate void HoverPointer(Tile tile);
    private HoverPointer Hover;
    delegate bool PlaceMethod(Tile HoveringTile, Tile SelectedTile);
    private PlaceMethod PlacePiece;
    private bool isBlack;
    private int groundLayer;


}
