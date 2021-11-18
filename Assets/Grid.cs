using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public      Player          player;
    public      LevelParser     levelParser;
    private     IEnumerator     coroutine;


    Vector2     gridSize        = new Vector2(16, 16);
    int         squareSize      = 32;

    public int currentLevel = 2;

    List<TileId> layerItems;

    List<GameObject> objectsLayerBackground = new List<GameObject>();
    List<GameObject> objectsLayerItems      = new List<GameObject>();

    void Start()
    {
        levelParser.GenerateLevels();
        layerItems = levelParser.levels[currentLevel].layerItem;
        LoadLevel(levelParser.levels[currentLevel].layerBackground, levelParser.levels[currentLevel].layerItem);

        gridSize = levelParser.levels[currentLevel].gridSize;

        coroutine = LerpPlayerPosition(player.gridPos);
        StopCoroutine(coroutine);

        player.move_event.AddListener( OnPlayerMove );
    }


    void OnPlayerMove()
    {
        /*  
         * Correct player position
         */
        if (player.nextGridPos.x < 0 ||
            player.nextGridPos.x >= gridSize.x ||
            player.nextGridPos.y < 0 ||
            player.nextGridPos.y >= gridSize.y)
        {
            player.nextGridPos = player.gridPos;
            return;
        }


        /*
         * Drop player blob
         */
        int i = GetIndexVector(player.gridPos, gridSize.y);
        levelParser.levels[currentLevel].layerItem[i] = TileId.Self;
        player.size -= player.sizeStepDown;

        /*
         * Get player grid position
         */
        float player_x = player.nextGridPos.x;
        float player_y = player.nextGridPos.y;

        /*
         * Set new player position
         */
        Vector3 player_real_pos     = new Vector3(0, 0, -10);
        player_real_pos.x           = player_x * squareSize;
        player_real_pos.y           = player_y * squareSize;

        Vector3 last_real_pos       = new Vector3(0, 0, -10);
        last_real_pos.x             = player.gridPos.x * squareSize;
        last_real_pos.y             = player.gridPos.y * squareSize;

        //player.transform.position = player_real_pos;
        StopCoroutine(coroutine);
        coroutine = LerpPlayerPosition(player_real_pos);
        StartCoroutine(coroutine);

        /*
         * Check what to do when hitting tile
         */
        int index = (int)(player_x * gridSize.y + player_y);
        TileId id = levelParser.levels[currentLevel].layerBackground[index];
        switch (id)
        {
            case TileId.Self:
                //Do item stuff
                break;
            case TileId.Player:
                //Throw error
                break;
            case TileId.Start:
                //Nothing
                break;
            case TileId.End:
                //Check for size / GotoNextLevel
                break;
            case TileId.Empty:
                //Do item stuff
                break;
            default:
                //Throw error
                break;
        }

        /*
         * Set player grid position
         */
        player.gridPos = player.nextGridPos;

        UpdateItemLayer();
    }

    private IEnumerator LerpPlayerPosition(Vector3 end)
    {
        bool loop = true;
        while (loop)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, end, Time.deltaTime * 10);

            if (player.transform.position.x < end.x + 0.1 &&
                player.transform.position.x > end.x - 0.1 &&
                player.transform.position.y < end.y + 0.1 &&
                player.transform.position.y > end.y - 0.1)
            {
                player.transform.position = end;
                loop = false;
            }
            yield return null;
        }
        yield return null;
    }

    void UpdateItemLayer()
    {
        for (int i = 0; i < objectsLayerItems.Count; i++)
        {
           Destroy(objectsLayerItems[i]);
        }
        objectsLayerItems.Clear();

        string name = "";
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 location = new Vector3(0, 0, -5);
                location.x = x * squareSize;
                location.y = y * squareSize;
                location.z = -5;

                int index = GetIndexVector(new Vector2(x, y), gridSize.y);
                TileId id = levelParser.levels[currentLevel].layerItem[index];
                switch (id)
                {
                    case TileId.Player:
                        player.transform.position.Set(location.x, location.y, player.transform.position.z);
                        break;
                    case TileId.Start:
                        player.transform.position.Set(location.x, location.y, player.transform.position.z);
                        break;
                    case TileId.End:
                        name = "BiggerBlob";
                        break;

                    case TileId.Shrink:
                        name = "WaterySpot";
                        break;
                    case TileId.Narrow:
                        name = "NarrowPassage";
                        break;
                    case TileId.Grow:
                        name = "Growblob";
                        break;
                    case TileId.Self:
                        name = "BiggerBlob";
                        break;

                    case TileId.Empty:
                        name = "Empty";
                        break;
                    default:
                        id      = TileId.Empty;
                        name    = "Empty";
                        break;
                }

                if (id != TileId.Empty)
                {
                    GameObject obj = (GameObject)Resources.Load(name);
                    GameObject instance = Instantiate(obj, location, new Quaternion(0, 0, 0, 0));
                    objectsLayerItems.Add(instance);
                }
            }
        }
    }

    void LoadLevel(List<TileId> levelBackground, List<TileId> itemLayer)
    {
        string name = "";
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                /*
                 * Draw first layer
                 */
                int index = GetIndexVector(new Vector2(x, y), gridSize.y);
                TileId id = levelBackground[index];

                Vector3 location = new Vector3(0, 0, -1);
                location.x = x * squareSize;
                location.y = y * squareSize;

                switch (id)
                {
                    case TileId.Water:
                        name = "Water";
                        break;
                    case TileId.Ground:
                        name = "Ground";
                        break;
                    default:
                        name = "Water";
                        break;
                }

                GameObject obj = (GameObject)Resources.Load(name);
                GameObject instance = Instantiate(obj, location, new Quaternion(0, 0, 0, 0));
                objectsLayerBackground.Add(instance);

                /*
                 * Draw second layer
                 */
                location.z = -10;
                id = itemLayer[index];
                switch (id)
                {
                    case TileId.Player:
                        player.transform.position.Set(location.x, location.y, player.transform.position.z);
                        break;
                    case TileId.Start:
                        player.transform.position.Set(location.x, location.y, player.transform.position.z);
                        break;
                    case TileId.End:
                        name = "BiggerBlob";
                        break;

                    case TileId.Shrink:
                        name = "WaterySpot";
                        break;
                    case TileId.Narrow:
                        name = "NarrowPassage";
                        break;
                    case TileId.Grow:
                        name = "Growblob";
                        break;

                    case TileId.Empty:
                        name = "Empty";
                        break;
                    default:
                        id = TileId.Empty;
                        name = "Empty";
                        break;
                }

                if (id != TileId.Empty)
                {
                    obj = (GameObject)Resources.Load(name);
                    instance = Instantiate(obj, location, new Quaternion(0, 0, 0, 0));
                    objectsLayerItems.Add(instance);
                }
            }
        }
    }

    int GetIndexVector(Vector2 index, float gridHeight)
    {
        return (int)(index.x * gridHeight + index.y);
    }
}
