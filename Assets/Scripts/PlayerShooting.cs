using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    public enum playerType { Player1, Player2 }
    public enum shootingType { oneBulletShot, tripleBulletShot, allAroundShot};

    public playerType pType;

    [Space]
    public float rotationSpeed;
    public float bulletSpeed;
    public float gunFireRate;

    public float gunRecoiledRotX;

    public string enemyTag;   
    
    public Transform gun;
    public Transform bulletOrigin;
    public Transform gunMesh;
    public Transform CameraShaker;

    public GameObject bullet;
    public GameObject muzzleFlash;

    public TextMeshProUGUI bulletCountUI;
    
    public AnimationCurve gunRecoilAnimation;

    public List<ShootingAttribute> shootingAttributes;

    public AudioClip shootSound;

    public Dictionary<shootingType, ShootingAttribute> shootingTypeDictionary = new Dictionary<shootingType, ShootingAttribute>();

    public int bulletsRemaining;

    float currentGunFireTime;

    Quaternion newRot;
    public shootingType currentShootingType;

    bool isRotating;

    InputManager im;

    void Start()
    {
        newRot = gun.rotation;
        currentGunFireTime = gunFireRate;
        im = FindObjectOfType<InputManager>();

        string[] shootingTypeNames = System.Enum.GetNames(typeof(shootingType));
        List<shootingType> shootingTypes = new List<shootingType>();

        for (int i = 0; i < shootingAttributes.Count; i++)
        {
            shootingTypes.Add((shootingType)System.Enum.Parse(typeof(shootingType), shootingTypeNames[i]));
            shootingTypeDictionary.Add(shootingTypes[i],shootingAttributes[i]);
        }

        bulletsRemaining = shootingTypeDictionary[shootingType.oneBulletShot].bulletCapacity;
    }

    void Update()
    {
        if (currentShootingType == shootingType.oneBulletShot)
        {
            bulletCountUI.text = "∞";
            bulletCountUI.fontSize = 52;
        }
        else
        {
            bulletCountUI.text = bulletsRemaining.ToString() + " / " + (shootingTypeDictionary[currentShootingType].bulletCapacity * shootingTypeDictionary[currentShootingType].noOfBullets).ToString();
            bulletCountUI.fontSize = 32;
        }

        if(bulletsRemaining <= 0)
        {
            currentShootingType = shootingType.oneBulletShot;
            bulletsRemaining = shootingTypeDictionary[shootingType.oneBulletShot].bulletCapacity;
        }

        float fireRateRatio = currentGunFireTime / gunFireRate;
        float abosluteRecoilRotValue = Mathf.Abs(gunRecoiledRotX);

        currentGunFireTime += currentGunFireTime < gunFireRate ? Time.deltaTime: 0f;

        string[] shootingTypeNames = System.Enum.GetNames(typeof(shootingType));
        

        for (int i = 0; i < shootingTypeDictionary.Count; i++)
        {
            if((shootingType)System.Enum.Parse(typeof(shootingType), shootingTypeNames[i]) != currentShootingType)
            {
                shootingTypeDictionary[(shootingType)System.Enum.Parse(typeof(shootingType), shootingTypeNames[i])].gunMesh.SetActive(false);
            }

            else
            {
                shootingTypeDictionary[(shootingType)System.Enum.Parse(typeof(shootingType), shootingTypeNames[i])].gunMesh.SetActive(true);
                gunMesh = shootingTypeDictionary[(shootingType)System.Enum.Parse(typeof(shootingType), shootingTypeNames[i])].gunMesh.transform;
            }
        }

        if (currentShootingType != shootingType.allAroundShot)
            gunMesh.localRotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(-abosluteRecoilRotValue, 0f, 0f), gunRecoilAnimation.Evaluate(fireRateRatio));
        else
        {
            if (gunMesh.gameObject.activeSelf)
            {
                Transform[] guns = new Transform[gunMesh.childCount];

                for (int i = 0; i < guns.Length; i++)
                {
                    guns[i] = gunMesh.GetChild(i);                  
                }

                guns[0].localRotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(-abosluteRecoilRotValue, 0f, 0f), gunRecoilAnimation.Evaluate(fireRateRatio));
                guns[1].localRotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(abosluteRecoilRotValue, 0f, 0f), gunRecoilAnimation.Evaluate(fireRateRatio));
                guns[2].localRotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(0f, 0f, abosluteRecoilRotValue), gunRecoilAnimation.Evaluate(fireRateRatio));
                guns[3].localRotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(0f, 0f, -abosluteRecoilRotValue), gunRecoilAnimation.Evaluate(fireRateRatio));
            }
        }

        if (!isRotating)
        {
            if (Input.GetKeyDown(im.playersKeyBindings[pType.ToString()]["RotateLeft"]))
            {
                newRot = Quaternion.Euler(0, -90f, 0) * gun.localRotation;
                isRotating = true;
            }

            if (Input.GetKeyDown(im.playersKeyBindings[pType.ToString()]["RotateRight"]))
            {
                newRot = Quaternion.Euler(0,90f,0) * gun.localRotation;
                isRotating = true;
            }

            if(Input.GetKey(im.playersKeyBindings[pType.ToString()]["Shoot"]) &&  currentGunFireTime >= gunFireRate)
            {
                SpawnBullet();                
                currentGunFireTime = 0;

                CameraShake cs = CameraShaker.GetComponent<CameraShake>();
                StartCoroutine(cs.ShakeCamera(cs.cameraShakePropertiesDictionary[CameraShake.shakeAttributeEnum.shoot]));
                AudioSource.PlayClipAtPoint(shootSound, GameObject.FindGameObjectWithTag("AudioListener").transform.position, FindObjectOfType<MusicManager>().soundValue);
            }
        }

        else
        {
            gun.localRotation = Quaternion.RotateTowards(gun.localRotation,newRot,rotationSpeed * Time.deltaTime);
        }

        if (gun.localRotation == newRot)
        {
            isRotating = false;
            newRot = gun.rotation;
        }             
    }

    void SpawnBullet()
    {
        float radius = shootingTypeDictionary[currentShootingType].radius;
        int noOfbullets = shootingTypeDictionary[currentShootingType].noOfBullets;
        float currentAngle = radius / -2f;
        float addAngle = radius / (noOfbullets - 1f);

        if (radius == 360f)
            addAngle = radius / noOfbullets;

        Quaternion gunLastRot = gun.localRotation;

        if(currentShootingType == shootingType.tripleBulletShot)
        {
            gun.localRotation *= Quaternion.Euler(0, currentAngle, 0);
        }

        for (int i = 0; i < noOfbullets; i++)
        {


            GameObject _bullet = Instantiate(bullet, bulletOrigin.position, gun.localRotation) as GameObject;
            Bullet bulletScript = _bullet.GetComponent<Bullet>();
            bulletScript.enemyTag = enemyTag;
            bulletScript.playerTag = transform.tag;
            _bullet.transform.localRotation = transform.rotation * gun.localRotation;
            Instantiate(muzzleFlash, bulletOrigin.position, gun.localRotation);

            if (currentShootingType == shootingType.allAroundShot || currentShootingType == shootingType.tripleBulletShot)
            {
                gun.localRotation *= Quaternion.Euler(0, addAngle, 0);
            }

            else
            {
                _bullet.transform.localRotation *= Quaternion.Euler(0, currentAngle, 0);
            }
            Destroy(_bullet, 10f);

            currentAngle += addAngle;

            if(currentShootingType != shootingType.oneBulletShot)
            {
                bulletsRemaining--;
            }
        }

        gun.localRotation = gunLastRot;
    }

    public void HittedShake(CameraShake cs, CameraShake.shakeAttributeEnum saEnum)
    {
        StartCoroutine(cs.ShakeCamera(cs.cameraShakePropertiesDictionary[CameraShake.shakeAttributeEnum.hitted]));
    }
}

[System.Serializable]
public struct ShootingAttribute
{
    public float radius;
    public int noOfBullets;
    public int bulletCapacity;
    public GameObject gunMesh;
}

