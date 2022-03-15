using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetColliderSize : MonoBehaviour
{
	public void GetMySize()
	{
		Vector3 size = GetComponent<Collider>().bounds.size;

		Debug.Log(string.Format("Object {0} is of size {1}", name, size));
		Debug.Log("ytyp");
	}
}
