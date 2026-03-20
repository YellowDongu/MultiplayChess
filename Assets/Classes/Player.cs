using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    void Start()
    {
        groundLayer = LayerMask.GetMask("Board");
        Hover = BoardManager.GetInstance().Hover;
    }

    void Update()
    {

        if (Selected != null)
        {
            Selected.transform.position = position;
            SelectedPiece.HighlightTile();

            if (Mouse.current.rightButton.wasPressedThisFrame)
                Deselect();
            else if (Mouse.current.leftButton.wasPressedThisFrame)
                Place();
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
        Vector2 mousePos = Mouse.current.position.ReadValue();
        output = null;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f)), out RaycastHit hit, 100f, groundLayer))
        {
            if (hit.collider.CompareTag("Board"))
                output = hit.transform.gameObject.GetComponent<Tile>();
            position = hit.point;
            position.y = 1.0f;
        }

        return output != null;
    }

    private bool Place()
    {
        if (HoveringTile.GetState() != Tile.Status.Select)
            return false;

        if (HoveringTile.SetPiece(SelectedPiece))
        {
            SelectedPiece.DeHighlightTile();
            SelectedPiece.TileSearch();
            Selected = null;
            SelectedTile = null;
            SelectedPiece = null;
            return true;
        }
        return false;
    }

    private void Hovering()
    {
        if (!Raycast(out HoveringTile))
            return;

        Hover(HoveringTile);
    }

    private void Deselect()
    {
        Selected.transform.position = SelectedTile.gameObject.transform.position;
        SelectedPiece.DeHighlightTile();
        Selected = null;
        SelectedTile = null;
        SelectedPiece = null;
    }
    private void Select()
    {
        if (Selected != null || HoveringTile == null || HoveringTile.GetPiece() == null)
            return;
        SelectedTile = HoveringTile;
        SelectedPiece = HoveringTile.GetPiece();
        Selected = SelectedPiece.gameObject;

        HoveringTile.GetPiece().TileSearch();
    }

    [SerializeField] private GameObject Selected = null;
    [SerializeField] private Piece SelectedPiece = null;
    [SerializeField] private Tile SelectedTile = null;
    [SerializeField] private Tile HoveringTile = null;
    [SerializeField] Vector3 position;

    delegate void HoverPointer(Tile tile);
    private HoverPointer Hover;

    private int groundLayer;


}
