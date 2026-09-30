using UnityEngine;

public class EffectAutoDestruct : MonoBehaviour
{
    public bool onlyDeactivate;
    public float lifetime = 2f;

    [Header("随机旋转")]
    public bool randomRotation = true;
    public bool onlyZAxis = true;

    private void OnEnable()
    {
        if (randomRotation)
        {
            if (onlyZAxis)
                transform.Rotate(0f, 0f, Random.Range(0f, 360f), Space.Self);
            else
                transform.rotation = Random.rotation;
        }

        if (lifetime > 0f)
        {
            StartCoroutine(DestroyAfterDelay());
        }
    }

    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(lifetime);

        if (onlyDeactivate)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);
    }
}