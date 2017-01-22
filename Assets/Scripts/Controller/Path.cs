using UnityEngine;
using System.Collections;

public class Path : MonoBehaviour {

	public Transform[] pathnodes;
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Transform GetClosest(Vector3 position)
	{
		if (pathnodes.Length == 0)
			return null;

		if (pathnodes.Length == 1)
			return pathnodes [0];

		Transform closest = null;
		float distance = 0f;

		foreach (Transform pathnode in pathnodes) 
		{
			float d = Vector3.Distance( position, pathnode.position);

			if(!closest || d < distance)
			{
				closest = pathnode;
				distance = d;
			}
		}

		return closest;
	}

	public Transform GetNext(Transform current)
	{
		if (pathnodes.Length == 0)
			return null;
		
		if (pathnodes.Length == 1)
			return pathnodes [0];

		if (!current)
			return pathnodes [0];

		int index = System.Array.IndexOf (pathnodes, current);

		if (index == -1)
			return pathnodes [0];

		index++;
		if (index >= pathnodes.Length)
			return pathnodes [0];

		return pathnodes [index];
	}
}
