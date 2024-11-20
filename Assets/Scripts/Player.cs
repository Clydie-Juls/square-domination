using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum playerType { Player1, Player2}

    public playerType pType;
    enum input { up,down,left,right};

    [Space]
    public float timeUntilRotation;

    public float moveSpeed;
    public float rotationSpeed;
    [Range(0f,1f)]
    public float cameraFollowSpeed;

    public float whiteFlashTime;

    public string enemyTag;

    public Player enemy;

    public LayerMask groundCheck;
    public LayerMask playerCheck;
    public LayerMask downwardRayMask;
    public LayerMask conveyerBeltMask;
    public Transform playerCamera;
    public Transform playerMesh;

    public GameObject jumpTrailParticle;
    public GameObject playerBloodSpray;
    public GameObject rotationMeterGameObject;

    public Material whiteFlashMaterial;

    public AnimationCurve moveAnimation;
    public AnimationCurve strechAnimation;

    public AudioClip jumpSound;    
    public AudioClip conveyerBeltSound;

    float currentTimeUntilRotation;   

    [HideInInspector]
    public Vector3 newPos;
    Vector3 rotatingMoveDirection;
    Vector3 lastPos;
    Vector3 playerStartPosition;    
    Quaternion newRot;
    [HideInInspector]
    public bool isMoving;
    [HideInInspector]
    public bool isRotating;
    bool spawnParticle;
    [HideInInspector]
    public bool isWhiteFlash;

    Material originalMaterial;

    input lastInput;

    AudioSource source;

    Transform[] SidesCheckOrigin = new Transform[4];

    InputManager im;

    List<GameObject> rotationMeterUI = new List<GameObject>();
  
    void Start()
    {
        source = GetComponent<AudioSource>();

        foreach(Transform t in rotationMeterGameObject.transform)
        {
            rotationMeterUI.Add(t.gameObject);
        }
        newPos = transform.position;
        playerStartPosition = transform.position;
        currentTimeUntilRotation = timeUntilRotation;
        originalMaterial = playerMesh.gameObject.GetComponent<MeshRenderer>().material;
        im = FindObjectOfType<InputManager>();
       
        SidesCheckOrigin[0] = new GameObject("Left Side Check Transform").transform;
        SidesCheckOrigin[1] = new GameObject("Right Side Check Transform").transform;
        SidesCheckOrigin[2] = new GameObject("Back Side Check Transform").transform;
        SidesCheckOrigin[3] = new GameObject("Forward Side Check Transform").transform;

        foreach (Transform t in SidesCheckOrigin)
        {
            t.SetParent(transform);
            t.position = transform.position;
        }

        SidesCheckOrigin[0].position += -transform.right;
        SidesCheckOrigin[1].position += transform.right;
        SidesCheckOrigin[2].position += -transform.forward;
        SidesCheckOrigin[3].position += transform.forward;

        SidesCheckOrigin[0].localRotation = Quaternion.Euler(0, -90f, 0);
        SidesCheckOrigin[1].localRotation = Quaternion.Euler(0, 90f, 0);
        SidesCheckOrigin[2].localRotation = Quaternion.Euler(0, 180f, 0);
        SidesCheckOrigin[3].localRotation = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        for (int i = 0; i < rotationMeterUI.Count; i++)
        {
            if(i + 1 <= (int)currentTimeUntilRotation)
            {
                rotationMeterUI[i].SetActive(true);
            }

            else
            {
                rotationMeterUI[i].SetActive(false);
            }
        }

        RaycastHit[] downwardhits = new RaycastHit[4];
        RaycastHit[] forwardhits = new RaycastHit[4];
        for (int i = 0; i < SidesCheckOrigin.Length; i++)
        {
            Ray downwardRay = new Ray(SidesCheckOrigin[i].position, -SidesCheckOrigin[i].up.normalized);
            Physics.Raycast(downwardRay, out downwardhits[i], 1f, downwardRayMask);
            Debug.DrawRay(SidesCheckOrigin[i].position, -SidesCheckOrigin[i].up, Color.green);

            Ray forwardRay = new Ray(SidesCheckOrigin[i].position - SidesCheckOrigin[i].forward, SidesCheckOrigin[i].forward.normalized);
            Physics.Raycast(forwardRay, out forwardhits[i], 1f,playerCheck);
            Debug.DrawRay(SidesCheckOrigin[i].position - SidesCheckOrigin[i].forward, SidesCheckOrigin[i].forward, Color.green);
        }

        if (currentTimeUntilRotation < timeUntilRotation && !isRotating)
        {
            currentTimeUntilRotation += Time.deltaTime;
        }       

        if (!isMoving && !isRotating)
        {
            newPos = transform.position;
          
            Vector3 probPos = newPos;
            Vector3 probPosDown = new Vector3();
            Vector3 probPosSidewayBoundary = new Vector3();

            Ray downRay = new Ray(transform.position, -transform.up);
            RaycastHit downHit = new RaycastHit();
            Physics.Raycast(downRay, out downHit, 1f, conveyerBeltMask);

            if (downHit.collider == null)
            {
                if (Input.GetKeyDown(im.playersKeyBindings[pType.ToString()]["Left"]) && (downwardhits[0].collider != null || currentTimeUntilRotation >= timeUntilRotation))
                {
                    if (forwardhits[0].collider != null && forwardhits[0].collider.CompareTag(enemyTag) ||
                        (downwardhits[0].collider != null && downwardhits[0].collider.CompareTag(enemyTag)))
                        return;

                    probPos += -transform.right;
                    probPosDown = probPos + -transform.up;
                    probPosSidewayBoundary = probPos;
                    if ((probPos == enemy.newPos || probPosDown == enemy.newPos) && enemy.gameObject.GetComponent<PlayerHealth>().health != 0)
                        return;

                    bool canMove = false;
                    for (int i = -1; i < 1 + 1; i++)
                    {
                        Vector3 pos = probPosSidewayBoundary + (transform.forward * i);
                        Vector3 BoundaryPos = -transform.right * -GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>().size;
                        //Debug.Log(BoundaryPos);
                        if (pos == enemy.newPos && (BoundaryPos.x == enemy.newPos.x || BoundaryPos.y == enemy.newPos.y || BoundaryPos.z == enemy.newPos.z) && enemy.gameObject.GetComponent<PlayerHealth>().health != 0)
                            canMove = true;

                    }

                    if (canMove)
                        return;


                    MovePlayer(-transform.right, Quaternion.Euler(0, 0, 90f), input.left,jumpSound);

                }

                else if (Input.GetKeyDown(im.playersKeyBindings[pType.ToString()]["Right"]) && (downwardhits[1].collider != null || currentTimeUntilRotation >= timeUntilRotation))
                {
                    if (forwardhits[1].collider != null && forwardhits[1].collider.CompareTag(enemyTag) ||
                        (downwardhits[1].collider != null && downwardhits[1].collider.CompareTag(enemyTag)))
                        return;

                    probPos += transform.right;
                    probPosDown = probPos + -transform.up;
                    probPosSidewayBoundary = probPos;
                    if ((probPos == enemy.newPos || probPosDown == enemy.newPos) && enemy.gameObject.GetComponent<PlayerHealth>().health != 0)
                        return;

                    bool canMove = false;
                    for (int i = -1; i < 1 + 1; i++)
                    {
                        Vector3 pos = probPosSidewayBoundary + (transform.forward * i);
                        Vector3 BoundaryPos = transform.right * -GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>().size;
                        //Debug.Log(BoundaryPos);
                        if (pos == enemy.newPos && (BoundaryPos.x == enemy.newPos.x || BoundaryPos.y == enemy.newPos.y || BoundaryPos.z == enemy.newPos.z) && enemy.gameObject.GetComponent<PlayerHealth>().health != 0)
                            canMove = true;

                    }

                    if (canMove)
                        return;

                    MovePlayer(transform.right, Quaternion.Euler(0, 0, -90f), input.right,jumpSound);
                }

                else if (Input.GetKeyDown(im.playersKeyBindings[pType.ToString()]["Down"]) && (downwardhits[2].collider != null || currentTimeUntilRotation >= timeUntilRotation))
                {
                    if (forwardhits[2].collider != null && forwardhits[2].collider.CompareTag(enemyTag) ||
                        (downwardhits[2].collider != null && downwardhits[2].collider.CompareTag(enemyTag)))
                        return;

                    probPos += -transform.forward;
                    probPosDown = probPos + -transform.up;
                    probPosSidewayBoundary = probPos;
                    if ((probPos == enemy.newPos || probPosDown == enemy.newPos) && enemy.gameObject.GetComponent<PlayerHealth>().health != 0)
                        return;

                    bool canMove = false;
                    for (int i = -1; i < 1 + 1; i++)
                    {
                        Vector3 pos = probPosSidewayBoundary + (transform.right * i);
                        Vector3 BoundaryPos = -transform.forward * -GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>().size;
                        //Debug.Log(BoundaryPos);
                        if (pos == enemy.newPos && (BoundaryPos.x == enemy.newPos.x || BoundaryPos.y == enemy.newPos.y || BoundaryPos.z == enemy.newPos.z) && enemy.gameObject.GetComponent<PlayerHealth>().health != 0)
                            canMove = true;

                    }

                    if (canMove)
                        return;

                    MovePlayer(-transform.forward, Quaternion.Euler(-90f, 0, 0), input.down,jumpSound);
                }

                else if (Input.GetKeyDown(im.playersKeyBindings[pType.ToString()]["Up"]) && (downwardhits[3].collider != null || currentTimeUntilRotation >= timeUntilRotation))
                {
                    if (forwardhits[3].collider != null && forwardhits[3].collider.CompareTag(enemyTag) ||
                        (downwardhits[3].collider != null && downwardhits[3].collider.CompareTag(enemyTag)))
                        return;

                    probPos += transform.forward;
                    probPosDown = probPos + -transform.up;
                    probPosSidewayBoundary = probPos;
                    if ((probPos == enemy.newPos || probPosDown == enemy.newPos) && enemy.gameObject.GetComponent<PlayerHealth>().health != 0)
                        return;

                    bool canMove = false;
                    for (int i = -1; i < 1 + 1; i++)
                    {
                        Vector3 pos = probPosSidewayBoundary + (transform.right * i);
                        Vector3 BoundaryPos = transform.forward * -GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>().size;
                        //Debug.Log(BoundaryPos);
                        if (pos == enemy.newPos && (BoundaryPos.x == enemy.newPos.x || BoundaryPos.y == enemy.newPos.y || BoundaryPos.z == enemy.newPos.z) && enemy.gameObject.GetComponent<PlayerHealth>().health != 0)
                            canMove = true;

                    }

                    if (canMove)
                        return;

                    MovePlayer(transform.forward, Quaternion.Euler(90f, 0, 0), input.up,jumpSound);
                }
            }

            else
            {
                input _input = new input();
                MovePlayer(downHit.collider.transform.forward, DirToRot(downHit.collider.transform.forward, out _input), _input,conveyerBeltSound);
                Vector3 CheckEnemyForwardPos = downHit.collider.transform.position + (downHit.collider.transform.forward);
                Vector3 CheckEnemyDownPos = downHit.collider.transform.position + (downHit.collider.transform.forward) + (-downHit.collider.transform.up);

                if(CheckEnemyForwardPos == (downHit.collider.transform.forward * 6) && currentTimeUntilRotation != timeUntilRotation)
                {
                    currentTimeUntilRotation = timeUntilRotation;
                }
                if (CheckEnemyForwardPos == enemy.newPos)
                {
                    enemy.MovePlayer(downHit.collider.transform.forward, DirToRot(downHit.collider.transform.forward, out _input), _input,conveyerBeltSound);
                }

                else if(CheckEnemyDownPos == enemy.newPos)
                {
                    enemy.MovePlayer(-downHit.collider.transform.up, DirToRot(downHit.collider.transform.forward, out _input), _input,conveyerBeltSound);
                }
            }
        }

        if (transform.position != newPos && isMoving && !isRotating)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, moveSpeed * Time.deltaTime);
            float distance = Vector3.Distance(transform.position, newPos);
            playerMesh.localPosition = Vector3.up * moveAnimation.Evaluate(distance);
            playerMesh.localScale = new Vector3(transform.localScale.x, transform.localScale.y + strechAnimation.Evaluate(distance), transform.localScale.z);
        }
        else
        {
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            playerMesh.localScale = Vector3.one;
            Debug.DrawRay(transform.position, -transform.up, Color.red);
            if (!Physics.Raycast(transform.position, -transform.up, 1f, groundCheck) && !isRotating)
            {               
                lastPos = transform.position;
                isRotating = true;
            }           
            isMoving = false;
        }

        if (spawnParticle && !isMoving)
        {
            Instantiate(jumpTrailParticle, transform.position - (transform.up / 1.5f), transform.rotation);
            spawnParticle = false;
        }

        if (isRotating)
        {
            currentTimeUntilRotation = 0;
            Vector3 dir = new Vector3();

            if (lastInput == input.left || lastInput == input.right)
                dir = new Vector3(Mathf.Abs(transform.right.x), Mathf.Abs(transform.right.y), Mathf.Abs(transform.right.z));
            if (lastInput == input.down || lastInput == input.up)
                dir = new Vector3(Mathf.Abs(transform.forward.x), Mathf.Abs(transform.forward.y), Mathf.Abs(transform.forward.z));

            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
            playerCamera.rotation = Quaternion.RotateTowards(playerCamera.rotation, newRot, rotationSpeed * Time.deltaTime);

            transform.position = new Vector3(lastPos.x + (rotatingMoveDirection.x * dir.x),
             lastPos.y + (rotatingMoveDirection.y * dir.y),
             lastPos.z + (rotatingMoveDirection.z * dir.z));

            if (transform.rotation == newRot)
            {
                isRotating = false;
                newPos = transform.position;
            }
        }    
        
        if(isWhiteFlash)
        {
            StartCoroutine(WhiteFlash());
            isWhiteFlash = false;
        }

        playerCamera.position = Vector3.Lerp( playerCamera.transform.position , (transform.position - playerStartPosition) / 2f, cameraFollowSpeed * Time.deltaTime);
    }

    void MovePlayer(Vector3 _newPos, Quaternion rot, input _lastInput,AudioClip sfx)
    {       
        newPos += _newPos;
        newRot = transform.rotation * rot;
        isMoving = true;
        rotatingMoveDirection = -transform.up;
        lastInput = _lastInput;
        spawnParticle = true;
        AudioSource.PlayClipAtPoint(sfx, GameObject.FindGameObjectWithTag("AudioListener").transform.position, FindObjectOfType<MusicManager>().soundValue);
    }

    Quaternion DirToRot(Vector3 dir, out input _input)
    {
        Quaternion rot = new Quaternion();
        input _lastInput =  new input();

        if (dir == -transform.right)
        {
            rot = Quaternion.Euler(0, 0, 90f);
            _lastInput = input.left;
        }
        else if (dir == transform.right)
        {
            rot = Quaternion.Euler(0, 0, -90f);
            _lastInput = input.right;
        }
        else if (dir == -transform.forward)
        {
            rot = Quaternion.Euler(-90f, 0, 0);
            _lastInput = input.down;
        }
        else if (dir == transform.forward)
        {
            rot = Quaternion.Euler(90f, 0, 0);
            _lastInput = input.up;
        }

        _input = _lastInput;
        return rot;
    }

    IEnumerator WhiteFlash()
    {
        playerMesh.gameObject.GetComponent<MeshRenderer>().material = whiteFlashMaterial;
        yield return new WaitForSeconds(whiteFlashTime);
        playerMesh.gameObject.GetComponent<MeshRenderer>().material = originalMaterial;
    }
}
