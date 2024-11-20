using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    [HideInInspector]
    public string enemyTag;
    [HideInInspector]
    public string playerTag;

    public GameObject bulletExplosion;

    public AudioClip hitSound;

    bool isCollided;

    Collider playerCollider;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(enemyTag))
        {            
            isCollided = true;
            playerCollider = other;
        }
    }

    void FixedUpdate()
    {
        if (isCollided)
        {
            playerCollider.gameObject.GetComponent<PlayerHealth>().currenthealth--;
            playerCollider.gameObject.GetComponent<Player>().isWhiteFlash = true;
            isCollided = false;
            Destroy(gameObject);           
            Instantiate(playerCollider.GetComponent<Player>().playerBloodSpray, playerCollider.gameObject.transform.position + (transform.forward / 2f), transform.localRotation);
            Instantiate(bulletExplosion, playerCollider.transform.position - (transform.forward), Quaternion.identity);
            CameraShake enenmyCS = playerCollider.GetComponent<PlayerShooting>().CameraShaker.GetComponent<CameraShake>();
            PlayerShooting enemyPS = playerCollider.GetComponent<PlayerShooting>();
            enemyPS.HittedShake(enenmyCS, CameraShake.shakeAttributeEnum.hitted);
            CameraShake playerCS = GameObject.FindGameObjectWithTag(playerTag).GetComponent<PlayerShooting>().CameraShaker.GetComponent<CameraShake>();
            PlayerShooting playerPS = GameObject.FindGameObjectWithTag(playerTag).GetComponent<PlayerShooting>();
            AudioSource.PlayClipAtPoint(hitSound, GameObject.FindGameObjectWithTag("AudioListener").transform.position, FindObjectOfType<MusicManager>().soundValue);
            playerPS.HittedShake(playerCS, CameraShake.shakeAttributeEnum.hittedReflectedShake);

        }

        GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed * Time.deltaTime;
    }
}
