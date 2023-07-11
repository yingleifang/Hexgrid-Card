using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtAction : BaseAction
{
	public IEnumerator LookAt(Vector3 point)
	{
		point.y = transform.localPosition.y;
		Quaternion fromRotation = transform.localRotation;
		Quaternion toRotation =
			Quaternion.LookRotation(point - transform.localPosition);
		float angle = Quaternion.Angle(fromRotation, toRotation);

		if (angle > 0f)
		{
			float speed = unit.rotationSpeed / angle;
			for (
				float t = Time.deltaTime * speed;
				t < 1f;
				t += Time.deltaTime * speed
			)
			{
				transform.localRotation =
					Quaternion.Slerp(fromRotation, toRotation, t);
				yield return null;
			}
		}

		transform.LookAt(point);
		unit.orientation = transform.localRotation.eulerAngles.y;
	}
}
