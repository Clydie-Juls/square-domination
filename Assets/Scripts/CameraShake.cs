using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public enum shakeAttributeEnum {shoot,hitted,die,hittedReflectedShake, dieReflectedShake}

    public List<Properties> cameraShakeProperties;

    public Dictionary<shakeAttributeEnum, Properties> cameraShakePropertiesDictionary = new Dictionary<shakeAttributeEnum, Properties>();

    void Awake()
    {
        string[] sAEnums = System.Enum.GetNames(typeof(shakeAttributeEnum));
        List<shakeAttributeEnum> enums = new List<shakeAttributeEnum>();
        for (int i = 0; i < sAEnums.Length; i++)
        {
            enums.Add((shakeAttributeEnum)System.Enum.Parse(typeof(shakeAttributeEnum),sAEnums[i]));
            cameraShakePropertiesDictionary.Add(enums[i], cameraShakeProperties[i]);
        }
    }


    public IEnumerator ShakeCamera(Properties pp)
    {
        float currentDuration = 0f;
        transform.localPosition = Vector3.zero;
        Vector3 posBeforeShake = transform.localPosition;

        while(currentDuration <= pp.duration)
        {
            float durationRatio = currentDuration / pp.duration;
            durationRatio = Mathf.Clamp(durationRatio, 0f, 1f);

            float randX = ((Random.value - 0.5f) * 2f) * (1f - durationRatio) * pp.magnitude;
            float randY = ((Random.value - 0.5f) * 2f) * (1f - durationRatio) * pp.magnitude;
            float randZ = ((Random.value - 0.5f) * 2f) * (1f - durationRatio) * pp.magnitude;

            transform.localPosition = new Vector3(randX, randY, randZ) + posBeforeShake;
            currentDuration += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = Vector3.zero;
    }


    [System.Serializable]
    public struct Properties
    {
        public float duration;
        public float magnitude;
    }
}
