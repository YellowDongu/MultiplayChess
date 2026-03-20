using UnityEngine;
using UnityEngine.Analytics;

public class BoardManager : MonoBehaviour
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    public BoardManager() { boardStatus = new Tile[8, 8]; }
    public static BoardManager GetInstance() { return singleInstance; }

    // ==============================================================================
    // Unity FrameCycle Methods
    // ==============================================================================
    private void Awake()
    {
        if(singleInstance != null)
            Destroy(this.gameObject);

        singleInstance = this;
    }

    private void Start()
    {
        Vector3 position;
        GameObject newInstance;
        position.y = 0.1f;
        for (int i = 0; i < 8; i++)
        {
            position.x = GetPosition(i);
            for (int j = 0; j < 8; j++)
            {
                newInstance = Instantiate(tilePrefab);

                if(newInstance == null)
                {
                    Debug.LogError("[Error] Instantiate failed :: Tiles " +  i.ToString() + ":" + j.ToString());
                    continue;
                }
                boardStatus[i, j] = newInstance.GetComponent<Tile>();
                boardStatus[i, j].Initialize(i, j);
                position.z = GetPosition(j);
                newInstance.transform.position = position;
            }
        }
    }

    private void Update()
    {
        
    }


    public void Hover(Tile tile)
    {
        if (hover == tile)
        {
            if (hover.GetState() != Tile.Status.Highlight)
                hover.SetState(Tile.Status.Highlight);
            return;

        }
        if (hover != null)
            hover.SetState(Tile.Status.None);
        if (tile == null)
        {
            hover = null;
            return;
        }
        hover = tile;
        hover.SetState(Tile.Status.Highlight);
    }

    private float GetPosition(int index) { return (float)(index - 4) * tilesize + (tilesize * 0.5f); }

    // ==============================================================================
    // variable GetSet Methods
    // ==============================================================================
    public Tile[,] LinkBoard() { return boardStatus; }
    public float tTileSize() { return tilesize; }
    public Tile GetTile(int x, int y) { if (x < 0 || y < 0 || y >= 8 || x >= 8) return null; return boardStatus[x, y]; }

    // ==============================================================================
    // variables
    // ==============================================================================

    private Tile[,] boardStatus;
    private static BoardManager singleInstance = null;

    [SerializeField] private Tile hover = null;
    [SerializeField] private float tilesize = 1.5f;
    [SerializeField] private GameObject tilePrefab = null;
}
