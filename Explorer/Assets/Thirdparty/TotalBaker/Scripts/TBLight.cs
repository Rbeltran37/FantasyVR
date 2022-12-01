/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

#if UNITY_EDITOR

using UnityEngine;

namespace TB
{
	[RequireComponent(typeof(Light))]
	[ExecuteInEditMode]
	public class TBLight : MonoBehaviour
	{
		//last values
		private LightType type;
		private Color color;
		private float intensity;
		private float range;
		private float spotAngle;

		private new Light light;

		private void Awake()
		{
			light = GetComponent<Light>();
		}

		/// <summary>
		/// Returns true if at least one relevant light's property has changed from the last computed values
		/// To be called from TB (polling system)
		/// </summary>
		/// <returns></returns>
		public bool HasChanged()
		{
			bool changed = false;
			
			if (type != light.type)
			{
				type = light.type;
				changed = true;
			}
			if (color != light.color)
			{
				color = light.color;
				changed = true;
			}
			if (!Mathf.Approximately(intensity, light.intensity))
			{
				intensity = light.intensity;
				changed = true;
			}
			if (!Mathf.Approximately(range, light.range))
			{
				range = light.range;
				changed = true;
			}
			if (!Mathf.Approximately(spotAngle, light.spotAngle))
			{
				spotAngle = light.spotAngle;
				changed = true;
			}

			if (!changed && transform.hasChanged) {
				changed = true;
				transform.hasChanged = false;
			}

			return changed;
		}
	}
}

#endif
