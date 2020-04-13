using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicator : MonoBehaviour
{
    public GameObject mainTile;
    public GameObject mainPlayer;
    GameObject playerOnTorus;

    private Dictionary<int, GameObject> tiles = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    private int[] xCoords = new int[] { -1, -1, 0, 1, 1, 1, 0, -1 };
    private int[] yCoords = new int[] { 0, 1, 1, 1, 0, -1, -1, -1 };

    private float size;

    float halfSize;

    private Vector3 mid;

    public static GameObject tileChanged = null;

    public Material mat;

    PolygonManager pb;

    private Vector3 position;
    // Use this for initialization
    void Start()
    {
        if (Wrapper.isEuclidean)
        {
            mid = mainTile.transform.position;
            size = mainTile.GetComponent<MeshRenderer>().bounds.size.x;
            halfSize = size / 2f;
            for (int i = 0; i < 8; i++)
            {
                GameObject tile0 = Instantiate(mainTile);
                tile0.name = "Tile " + i;
                tile0.transform.position = mainTile.transform.position + xCoords[i] * size * Vector3.right + yCoords[i] * size * Vector3.forward;
                tiles[i] = tile0;
                if (!Wrapper.isTorus && i != 0 && i != 4)
                {
                    tile0.transform.localScale = new Vector3(-tile0.transform.localScale.x, tile0.transform.localScale.y, tile0.transform.localScale.z);
                }
                GameObject player0 = Instantiate(mainPlayer);
                player0.name = "Player " + i;
                player0.GetComponent<PlayerController>().isMoved = false;
                player0.transform.position = mainTile.transform.position + xCoords[i] * size * Vector3.right + yCoords[i] * size * Vector3.forward;
                player0.GetComponent<TrailRenderer>().Clear();
                players[i] = player0;
            }
            tiles[8] = mainTile;
            players[8] = mainPlayer;

            playerOnTorus = Instantiate(mainPlayer);
            playerOnTorus.GetComponent<Rigidbody>().useGravity = false;
            Destroy(playerOnTorus.GetComponent<PlayerController>());
            playerOnTorus.transform.position = TorusScript.toTorus(mainPlayer.transform.position - tiles[8].transform.position, new Vector2(size, size)) + TorusScript.torus.transform.position;
            playerOnTorus.GetComponent<TrailRenderer>().Clear();
        }
        else
        {
            mainTile.SetActive(false);
            int p = 4;
            int q = 6;
            pb = new PolygonManager(p, q, mat);
            position = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Wrapper.isEuclidean)
        {
            if (mainPlayer.transform.position.magnitude > 100)
            {
                var move = new Vector3(mainPlayer.transform.position.x, 0, mainPlayer.transform.position.z);
                mainPlayer.transform.position -= move;
                mainPlayer.GetComponent<TrailRenderer>().Clear();
                foreach (var tile in tiles)
                {
                    tile.Value.transform.position -= move;
                }
                foreach (var player in players)
                {
                    player.Value.transform.position -= move;
                    player.Value.GetComponent<TrailRenderer>().Clear();
                }
                foreach (var bullet in GameObject.FindGameObjectsWithTag("Bullet"))
                {
                    bullet.transform.position -= move;
                }
            }
            foreach (var bullet in GameObject.FindGameObjectsWithTag("Bullet"))
            {
                var center = tiles[8].transform.position;
                var oneAndHalf = size + halfSize;
                var bPos = bullet.transform.position;
                if (bPos.x > center.x + oneAndHalf)
                {
                    bullet.transform.position = new Vector3(center.x - oneAndHalf, bPos.y, bPos.z);
                }
                else if (bPos.x < center.x - oneAndHalf)
                {
                    bullet.transform.position = new Vector3(center.x + oneAndHalf, bPos.y, bPos.z);
                }
                else if (bPos.z < center.z - oneAndHalf)
                {
                    if (Wrapper.isTorus)
                    {
                        bullet.transform.position = new Vector3(bPos.x, bPos.y, center.z + oneAndHalf);
                    }
                    else
                    {
                        bullet.transform.position = new Vector3(bPos.x + 2 * (center.x - bPos.x), bPos.y, center.z + oneAndHalf);
                    }
                }
                else if (bPos.z > center.x + oneAndHalf)
                {
                    if (Wrapper.isTorus)
                    {
                        bullet.transform.position = new Vector3(bPos.x, bPos.y, center.z - oneAndHalf);
                    }
                    else
                    {
                        bullet.transform.position = new Vector3(bPos.x + 2 * (center.x - bPos.x), bPos.y, center.z - oneAndHalf);
                    }
                }
            }
            for (int i = 0; i < 8; i++)
            {
                var player = players[i];
                player.transform.position = mainPlayer.transform.position + xCoords[i] * size * Vector3.right + yCoords[i] * size * Vector3.forward;
                if (!Wrapper.isTorus && i != 0 && i != 4)
                {
                    player.transform.position = new Vector3(player.transform.position.x + 2 * (tiles[i].transform.position.x - player.transform.position.x), player.transform.position.y, player.transform.position.z);
                }
            }
            if (mainPlayer.transform.position.z >= tiles[8].transform.position.z + halfSize)
            {
                AddTiles(2);
            }
            else if (mainPlayer.transform.position.z <= tiles[8].transform.position.z - halfSize)
            {
                AddTiles(6);
            }
            if (mainPlayer.transform.position.x >= tiles[8].transform.position.x + halfSize)
            {
                AddTiles(4);
            }
            else if (mainPlayer.transform.position.x <= tiles[8].transform.position.x - halfSize)
            {
                AddTiles(0);
            }
            if (tileChanged != null)
            {
                //			var index = tiles [8].GetComponent <tileScript> ().pickUps.IndexOf (tileChanged);
                //			foreach (var tile in tiles) {
                //				tile.Value.GetComponent <tileScript> ().pickUps [index].SetActive (false);
                //			}
                tileChanged = null;
            }
            if (Wrapper.isTorus)
            {
                var normalizedPosition = mainPlayer.transform.position - tiles[8].transform.position;
                playerOnTorus.transform.position = TorusScript.toTorus(normalizedPosition, new Vector2(size, size)) + TorusScript.torus.transform.position;
            }
        }
        else
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            var direction = mainPlayer.GetComponent<PlayerController>().direction;
            var movement = (moveVertical * direction + moveHorizontal * Vector3.Cross(direction, Vector3.down));
            var velocity = new Vector3(movement.x, 0, movement.z) * PlayerController.speed;
            //			var scaledVelocity = FundamentalPolygon.Scale (velocity);
            if (velocity.magnitude > 0)
            {
                //				var scaledPos = FundamentalPolygon.Scale (position);
                //				var goal = scaledPos + scaledVelocity;
                //				var f = PoincareDisc.Distance (scaledPos, goal);
                //				Debug.Log ("distance from " + scaledPos + " to " + goal + " is " + f); 
                position += velocity;
                pb.polygon.MoveTo(position, true);
                Debug.Log("move origin to " + position);
                movePlayerOnSurface();
            }
        }
    }

    void movePlayerOnSurface()
    {
        //		playerOnTorus.transform.position = toSurface ();
    }

    Dictionary<int, GameObject> bullets;

    void AddTiles(int i)
    {
        bullets = new Dictionary<int, GameObject>();
        foreach (var bullet in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            var number = int.Parse(bullet.name);
            bullets[number] = bullet;
        }
        if (i == 2)
        {
            var arr = new int[] {
                7, 6, 5
            };
            moveObjects(arr, Vector3.forward);
            setTiles(new int[] { 1, 7, 6, 5, 3, 4, 8, 0, 2 });
        }
        else if (i == 6)
        {
            var arr = new int[] {
                1, 2, 3
            };
            moveObjects(arr, Vector3.back);
            setTiles(new int[] { 7, 0, 8, 4, 5, 3, 2, 1, 6 });
        }
        else if (i == 4)
        {
            var arr = new int[] {
                1, 0, 7
            };
            moveObjects(arr, Vector3.right);
            setTiles(new int[] { 8, 2, 3, 1, 0, 7, 5, 6, 4 });
        }
        else if (i == 0)
        {
            var arr = new int[] {
                3, 4, 5
            };
            moveObjects(arr, Vector3.left);
            setTiles(new int[] { 4, 3, 1, 2, 8, 6, 7, 5, 0 });
        }
    }

    void moveObjects(int[] arr, Vector3 v)
    {
        foreach (var j in arr)
        {
            tiles[j].transform.position += 3 * size * v;
            if (bullets.ContainsKey(j))
            {
                bullets[j].transform.position += 3 * size * v;
            }
            players[j].GetComponent<TrailRenderer>().enabled = false;
            players[j].transform.position += 3 * size * v;
            players[j].GetComponent<TrailRenderer>().Clear();
            players[j].GetComponent<TrailRenderer>().enabled = true;
        }
    }

    void setTiles(int[] n)
    {
        Dictionary<int, GameObject> newTiles = new Dictionary<int, GameObject>();
        Dictionary<int, GameObject> newPlayers = new Dictionary<int, GameObject>();
        for (int i = 0; i < 9; i++)
        {
            newTiles[i] = tiles[n[i]];
        }
        for (int i = 0; i < 8; i++)
        {
            if (n[i] != 8)
            {
                newPlayers[i] = players[n[i]];
            }
            else
            {
                newPlayers[i] = players[n[8]];
            }
        }
        for (int i = 0; i < newPlayers.Count; i++)
        {
            var item = newPlayers[i];
            Debug.Log("In " + i + ": " + item.name);
        }
        tiles = newTiles;
        players = newPlayers;
    }
}
