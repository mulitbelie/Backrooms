using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
	public bool OnlyDeactivate;
	public float lifetime = 0f;

	void OnEnable()
	{
		if (lifetime > 0f)
		{
			StartCoroutine(CheckIfAlive());
			StartCoroutine(TimeoutDestruction());
		}
		else
		{
			StartCoroutine(CheckIfAlive());
		}
	}

	IEnumerator TimeoutDestruction()
	{
		yield return new WaitForSeconds(lifetime);
		DestroyOrDeactivate();
	}

	IEnumerator CheckIfAlive()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			if (!GetComponent<ParticleSystem>().IsAlive(true))
			{
				if (lifetime <= 0f)
				{
					DestroyOrDeactivate();
				}
				break;
			}
		}
	}

	void DestroyOrDeactivate()
	{
		if (OnlyDeactivate)
		{
#if UNITY_3_5
			this.gameObject.SetActiveRecursively(false);
#else
			this.gameObject.SetActive(false);
#endif
		}
		else
			GameObject.Destroy(this.gameObject);
	}
}