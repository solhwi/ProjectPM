using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUpDownComponent : MonoBehaviour
{
	private Coroutine scaleCoroutine = null;

	[SerializeField][Range(0.0f, 1.0f)] private float minScale = 1.0f;
	[SerializeField][Range(1.0f, 1.1f)] private float maxScale = 1.085f;
	[SerializeField][Range(0.0f, 2.0f)] private float animSpeed = 1.0f;

	private void OnEnable()
	{
		scaleCoroutine = StartCoroutine(ScaleUpDownRoutine(transform));
	}

	private void OnDisable()
	{
		StopCoroutine(scaleCoroutine);
	}

	private IEnumerator ScaleUpDownRoutine(Transform tr)
	{
		while (true)
		{
			yield return ScaleUpRoutine(tr);
			yield return ScaleDownRoutine(tr);
		}
	}

	private IEnumerator ScaleUpRoutine(Transform tr)
	{
		float t = 0.0f;

		while (t < 1.0f)
		{
			yield return null;
			t += Time.deltaTime * animSpeed;

			float interpolation = Mathf.Lerp(minScale, maxScale, t);
			tr.localScale = new Vector3(interpolation, interpolation, interpolation);
		}

		tr.localScale = new Vector3(maxScale, maxScale, maxScale);
	}

	private IEnumerator ScaleDownRoutine(Transform tr)
	{
		float t = 0.0f;

		while (t < 1.0f)
		{
			yield return null;
			t += Time.deltaTime;

			float interpolation = Mathf.Lerp(maxScale, minScale, t);
			tr.localScale = new Vector3(interpolation, interpolation, interpolation);
		}

		tr.localScale = new Vector3(minScale, minScale, minScale);
	}
}
