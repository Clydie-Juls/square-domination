using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int healValue;

    public float roatationSpeed;

    public GameObject powerUpParticles;
    public GameObject healingParticles;

    public AudioClip powerUpSound;

    bool isCollided;
    Collider playerCollider;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(0, roatationSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player1") || other.gameObject.CompareTag("Player2"))
        {
            isCollided = true;
            playerCollider = other;
        }
    }

    private void FixedUpdate()
    {
        if(isCollided)
        {
            int randPowerUp = Random.Range(1, 2 + 1);
            if (randPowerUp == 1)
            {
                PlayerShooting ps = playerCollider.GetComponent<PlayerShooting>();
                int rand = Random.Range(1, ps.shootingTypeDictionary.Count);
                string[] names = System.Enum.GetNames(typeof(PlayerShooting.shootingType));
                PlayerShooting.shootingType st = (PlayerShooting.shootingType)System.Enum.Parse(typeof(PlayerShooting.shootingType), names[rand]);
                ps.bulletsRemaining = ps.shootingTypeDictionary[st].bulletCapacity * ps.shootingTypeDictionary[st].noOfBullets;
                playerCollider.GetComponent<PlayerShooting>().currentShootingType = st;

                GameObject particle = Instantiate(powerUpParticles, transform.position, powerUpParticles.transform.rotation, transform) as GameObject;
                particle.transform.SetParent(null);
            }

            else if (randPowerUp == 2)
            {
                PlayerHealth ph = playerCollider.GetComponent<PlayerHealth>();
                int healthIfHeal = (int)ph.currenthealth + healValue;
                if (healthIfHeal <= ph.health)
                    ph.currenthealth += healValue;
                else
                    ph.currenthealth = ph.health;

                GameObject particle = Instantiate(healingParticles, transform.position, powerUpParticles.transform.rotation, transform) as GameObject;
                particle.transform.SetParent(null);
            }

            AudioSource.PlayClipAtPoint(powerUpSound, GameObject.FindGameObjectWithTag("AudioListener").transform.position, FindObjectOfType<MusicManager>().soundValue);
            Destroy(gameObject);
        }
    }
}
