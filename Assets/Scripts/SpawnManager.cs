using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{   
    public float timeTillRespawn;
    public float timeUntilVunerable;
    [Range(0,10)]
    public float cameraLerpSpeed;

    public float maxNoOfPowerUps;
    public float timeToSpawnPowerUp;

    public GameObject ground;
    public GameObject powerUp;
    public GameObject conveyerBelt;
    public GameObject powerUpParticles;
    public GameObject conveyerBeltParticles;
    public List< GameObject> players;
    public List<GameObject> playerCameras;
    public List<GameObject> playerRespawnParticles;

    public TextMeshProUGUI[] scoresUI;

    bool respawnPlayer1;
    bool respawnPlayer2;

    bool lerpRotatePlayer1Camera;
    bool lerpRotatePlayer2Camera;

    GameObject[] currentObjects;

    int rand;
    [HideInInspector]
    public int[] scores = new int[2];
    [HideInInspector]
    public float size;

    Vector3 pos;
    Vector3 rot;

    List<TransformVectorsInfo> transformInfo = new List<TransformVectorsInfo>();

    // Start is called before the first frame update
    void Awake()
    {

        size = -((ground.transform.localScale.x / 2f) + 0.5f);
        for (int x = 0; x < ground.transform.localScale.x + 2; x++)
        {
            for (int y = 0; y < ground.transform.localScale.y + 2; y++)
            {
                for (int z = 0; z < ground.transform.localScale.z + 2; z++)
                {
                    if ((x == 0 && (y != 0 && y != ground.transform.localScale.y) && (z != 0 && z != ground.transform.localScale.z)) ||
                        (x == ground.transform.localScale.x && (y != 0 && y != ground.transform.localScale.y) && (z != 0 && z != ground.transform.localScale.z)) ||
                        (y == 0 && (x != 0 && x != ground.transform.localScale.x) && (z != 0 && z != ground.transform.localScale.z)) ||
                        (y == ground.transform.localScale.x && (x != 0 && x != ground.transform.localScale.x) && (z != 0 && z != ground.transform.localScale.z)) ||
                        (z == 0 && (y != 0 && y != ground.transform.localScale.y) && (x != 0 && x != ground.transform.localScale.x)) ||
                        (z == ground.transform.localScale.x && (y != 0 && y != ground.transform.localScale.y) && (x != 0 && x != ground.transform.localScale.x)))
                    {

                    }

                    if ((x == 0 && (y != 0 && y != ground.transform.localScale.y + 1) && (z != 0 && z != ground.transform.localScale.z + 1)))
                        transformInfo.Add(new TransformVectorsInfo(new Vector3(x + size, y + size, z + size), new Vector3(0, 0, 90f), 1));

                    if ((x == ground.transform.localScale.x + 1 && (y != 0 && y != ground.transform.localScale.y + 1) && (z != 0 && z != ground.transform.localScale.z + 1)))
                        transformInfo.Add(new TransformVectorsInfo(new Vector3(x + size, y + size, z + size), new Vector3(0, 0, -90f), 2));

                    if ((y == 0 && (x != 0 && x != ground.transform.localScale.x + 1) && (z != 0 && z != ground.transform.localScale.z + 1)))
                        transformInfo.Add(new TransformVectorsInfo(new Vector3(x + size, y + size, z + size), new Vector3(180f, 0, 0), 3));

                    if ((y == ground.transform.localScale.y + 1 && (x != 0 && x != ground.transform.localScale.x + 1) && (z != 0 && z != ground.transform.localScale.z + 1)))
                        transformInfo.Add(new TransformVectorsInfo(new Vector3(x + size, y + size, z + size), new Vector3(0, 0, 0), 4));

                    if ((z == 0 && (y != 0 && y != ground.transform.localScale.y + 1) && (x != 0 && x != ground.transform.localScale.x + 1)))
                        transformInfo.Add(new TransformVectorsInfo(new Vector3(x + size, y + size, z + size), new Vector3(-90f, 0, 0), 5));

                    if ((z == ground.transform.localScale.z + 1 && (y != 0 && y != ground.transform.localScale.y + 1) && (x != 0 && x != ground.transform.localScale.x + 1)))
                        transformInfo.Add(new TransformVectorsInfo(new Vector3(x + size, y + size, z + size), new Vector3(90f, 0, 0), 6));

                }
            }
        }        

        rand = Random.Range(0, transformInfo.Count);
        pos = transformInfo[rand].pos;
        rot = transformInfo[rand].rot;

        players[0].transform.position = pos;
        players[0].transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);
        playerCameras[0].transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);

        int lastTransforminfo = rand;

        rand = Random.Range(0, transformInfo.Count);
        while (rand == lastTransforminfo || transformInfo[rand].side == transformInfo[lastTransforminfo].side) 
        {
            rand = Random.Range(0, transformInfo.Count);
        }

        pos = transformInfo[rand].pos;
        rot = transformInfo[rand].rot;

        players[1].transform.position = pos;
        players[1].transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);
        playerCameras[1].transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);        
    }

    private void Start()
    {
        StartCoroutine(SpawnPowerUps());
    }

    private void Update()
    {
        scoresUI[0].text = scores[0].ToString();
        scoresUI[1].text = scores[1].ToString();

        currentObjects = GameObject.FindGameObjectsWithTag("SpawnedObjects");

        if (players[0].GetComponent<PlayerHealth>().currenthealth <= 0 && !respawnPlayer1)
        {
            respawnPlayer1 = true;
            StartCoroutine (RespawnPlayer(players[0],0,1));
            scores[1]++;
        }
        if (players[1].GetComponent<PlayerHealth>().currenthealth <= 0 && !respawnPlayer2)
        {
            respawnPlayer2 = true;
            StartCoroutine(RespawnPlayer(players[1], 1,0));
            scores[0]++;
        }

        if (lerpRotatePlayer1Camera)
            CameraFollowPlayer(0);

        if (lerpRotatePlayer2Camera)
            CameraFollowPlayer(1);

        if(players[0].GetComponent<Player>().isRotating)
        {
            if (lerpRotatePlayer1Camera)
            {
                playerCameras[0].transform.rotation = players[0].transform.rotation;
                lerpRotatePlayer1Camera = false;               
            }
        }

        if (players[1].GetComponent<Player>().isRotating)
        {
            if (lerpRotatePlayer2Camera)
            {
                playerCameras[1].transform.rotation = players[1].transform.rotation;
                lerpRotatePlayer2Camera = false;
            }
        }

    }

    IEnumerator RespawnPlayer(GameObject player, int index,int enemyIndex)
    {
        yield return new WaitForSeconds(timeTillRespawn);
        rand = Random.Range(0, transformInfo.Count);

        for (int i = 0; i < currentObjects.Length; i++)
        {
            if(transformInfo[rand].pos == players[enemyIndex].GetComponent<Player>().newPos || transformInfo[rand].pos == currentObjects[i].transform.position)
            {
                i = 0;
                rand = Random.Range(0, transformInfo.Count);
            }
        }
        pos = transformInfo[rand].pos;
        rot = transformInfo[rand].rot;

        players[index].transform.position = pos;
        players[index].transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
        players[index].GetComponent<Player>().newPos = players[index].transform.position;
        playerCameras[index].transform.position = Vector3.zero;
        player.GetComponent<PlayerHealth>().currenthealth = player.GetComponent<PlayerHealth>().health;
        Instantiate(playerRespawnParticles[index], players[index].transform.position, players[index].transform.rotation);
        StartCoroutine(players[index].GetComponent<PlayerHealth>().ColliderActiveDelay());

        if (index == 0)
        {
            respawnPlayer1 = false;
            lerpRotatePlayer1Camera = true;
        }
        if (index == 1)
        {
            respawnPlayer2 = false;
            lerpRotatePlayer2Camera = true;
        }


    }

    void CameraFollowPlayer(int index)
    {
        playerCameras[index].transform.rotation = Quaternion.Lerp(playerCameras[index].transform.rotation, players[index].transform.rotation, cameraLerpSpeed * Time.deltaTime);
    }

    IEnumerator SpawnPowerUps()
    {
        yield return new WaitForSeconds(timeToSpawnPowerUp);
        int rand = Random.Range(0, transformInfo.Count);

        int randobject = Random.Range(0, 1 + 1);
        Check();

        void Check()
        {
            foreach(GameObject g in currentObjects)
            {
                if((transformInfo[rand].pos == players[0].transform.position ||
                transformInfo[rand].pos == players[1].transform.position ||
                transformInfo[rand].pos == g.transform.position))
                {                    
                    rand = Random.Range(0, transformInfo.Count);                   
                    Check();
                }

                else
                {
                    if (randobject == 1)
                    {
                        if (Mathf.Abs(transformInfo[rand].pos.x) == 5 || Mathf.Abs(transformInfo[rand].pos.y) == 5 || Mathf.Abs(transformInfo[rand].pos.z) == 5)
                        {
                            rand = Random.Range(0, transformInfo.Count);
                            Check();
                        }
                    }
                }
            }
        }     

        if (randobject == 0)
        {
            GameObject _powerUp = Instantiate(powerUp, transformInfo[rand].pos, powerUp.transform.rotation) as GameObject;
            _powerUp.transform.rotation *= Quaternion.Euler(transformInfo[rand].rot.x, transformInfo[rand].rot.y, transformInfo[rand].rot.z);
            Destroy(_powerUp, maxNoOfPowerUps * timeToSpawnPowerUp);

            GameObject particle = Instantiate(powerUpParticles, transformInfo[rand].pos, powerUpParticles.transform.rotation, _powerUp.transform) as GameObject;
            particle.transform.SetParent(null);
        }

        else if(randobject == 1)
        {
            GameObject conveyerBeltObj = Instantiate(conveyerBelt, transformInfo[rand].pos, conveyerBelt.transform.rotation) as GameObject;
            conveyerBeltObj.transform.rotation *= Quaternion.Euler(transformInfo[rand].rot.x, transformInfo[rand].rot.y, transformInfo[rand].rot.z);
            Quaternion[] randRot = new Quaternion[4]
            {
                Quaternion.Euler(0,0,0),
                Quaternion.Euler(0,90,0),
                Quaternion.Euler(0,180,0),
                Quaternion.Euler(0,270,0)
            };

            int randRotIndex = Random.Range(0, randRot.Length);

            conveyerBeltObj.transform.rotation *= randRot[randRotIndex];

            Destroy(conveyerBeltObj, maxNoOfPowerUps * timeToSpawnPowerUp);

            GameObject particle = Instantiate(conveyerBeltParticles, transformInfo[rand].pos, conveyerBeltParticles.transform.rotation, conveyerBeltObj.transform) as GameObject;
            particle.transform.SetParent(null);
        }

        StartCoroutine(SpawnPowerUps());
    }   

    [System.Serializable]
    public struct TransformVectorsInfo
    {
        public Vector3 pos;
        public Vector3 rot;
        public int side;

        public TransformVectorsInfo(Vector3 _pos,Vector3 _rot, int _side)
        {
            pos = _pos;
            rot = _rot;
            side = _side;
        }
    }
}
