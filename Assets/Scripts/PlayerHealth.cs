using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int health;

    public float timeUntilActivateCollider;

    public GameObject deathParticles;
    public GameObject playerMesh;

    public Image playerHealthUI;

    [HideInInspector]
    public float currenthealth;

    public AudioClip deathSound;

    bool canSpawnDeathParticles;
    bool spawnDeathParticles;
    bool colliderIsEnabled;

    Material playerOriginalMaterial;

    void Start()
    {
        currenthealth = health;
        playerOriginalMaterial = GetComponent<Player>().playerMesh.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if(currenthealth <= 0)
        {            
            GetComponent<Player>().enabled = false;
            GetComponent<PlayerShooting>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            playerMesh.SetActive(false);
            colliderIsEnabled = false;

            if(canSpawnDeathParticles)
                spawnDeathParticles = true;
        }

        else
        {
            GetComponent<Player>().enabled = true;
            GetComponent<PlayerShooting>().enabled = true;           
            playerMesh.SetActive(true);
            canSpawnDeathParticles = true;
        }

        if(spawnDeathParticles)
        {
            CameraShake playerCS = GetComponent<PlayerShooting>().CameraShaker.GetComponent<CameraShake>();
            StartCoroutine(playerCS.ShakeCamera(playerCS.cameraShakePropertiesDictionary[CameraShake.shakeAttributeEnum.die]));
            CameraShake enemyCS = GameObject.FindGameObjectWithTag(GetComponent<PlayerShooting>().enemyTag).GetComponent<PlayerShooting>().CameraShaker.GetComponent<CameraShake>();
            StartCoroutine(enemyCS.ShakeCamera(enemyCS.cameraShakePropertiesDictionary[CameraShake.shakeAttributeEnum.dieReflectedShake]));
            Instantiate(deathParticles, transform.position, transform.rotation);
            canSpawnDeathParticles = false;
            spawnDeathParticles = false;
            AudioSource.PlayClipAtPoint(deathSound, GameObject.FindGameObjectWithTag("AudioListener").transform.position, FindObjectOfType<MusicManager>().soundValue);
            GetComponent<Player>().newPos -= transform.up;
        }

        float healthRatio = currenthealth / health;
        playerHealthUI.fillAmount = healthRatio;
    }

    public IEnumerator ColliderActiveDelay()
    {
        StartCoroutine(ToggleWhiteFlash());
        yield return new WaitForSeconds(timeUntilActivateCollider);
        GetComponent<BoxCollider>().enabled = true;
        colliderIsEnabled = true;
    }

    IEnumerator ToggleWhiteFlash()
    {
        yield return new WaitForSeconds(0.15f);
        Material whiteFlashMaterial = GetComponent<Player>().whiteFlashMaterial;
        GetComponent<Player>().playerMesh.GetComponent<MeshRenderer>().material = whiteFlashMaterial;
        yield return new WaitForSeconds(0.10f);
        GetComponent<Player>().playerMesh.GetComponent<MeshRenderer>().material = playerOriginalMaterial;
        if (!colliderIsEnabled)
            StartCoroutine(ToggleWhiteFlash());
    }

}
