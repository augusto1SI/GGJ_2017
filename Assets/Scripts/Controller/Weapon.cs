using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour {

	public float rateOfFire=1f;
	private bool firing = false;
	public float range = 100f;
	public float arc = 5f;

	public float hitForce = 100f;

	public Transform firePoint;

	public float accuracyError = 5f;

	public virtual void Start()
	{
		if(!firePoint)
			firePoint = transform;
	}

	public virtual void Update()
	{
		
	}

	public bool Fire
	{
		set{
			if(firing)
				return;
			if(value)
				StartCoroutine(DoFire());
		}
	}

	IEnumerator DoFire()
	{
		firing = true;

		if(firePoint != transform && accuracyError != 0)
			firePoint.Rotate(Random.Range(-accuracyError, accuracyError), Random.Range(-accuracyError, accuracyError), 0f);

		OnFire();

		if(firePoint != transform && accuracyError != 0)
			firePoint.rotation = transform.rotation;

		yield return new WaitForSeconds(rateOfFire);
		firing = false;
	}

	protected abstract void OnFire();
}
