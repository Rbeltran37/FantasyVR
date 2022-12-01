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
	public class RenderTextureUtility
	{

		/// <summary>
		/// Converts a Texture2D into a RenderTexture, creating a new one and returning it.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static RenderTexture Texture2D2RenderTexture(Texture2D source)
		{
			//for some reason, ARGBFloat produces artifacts. Switched back to ARGB64.
			RenderTexture rt = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGB64);
			rt.Create();
			Graphics.Blit(source, rt);
			return rt;
		}

		/// <summary>
		/// Converts a RenderTexture into a Texture2D, creating a new one and returning it.
		/// </summary>
		public static Texture2D RenderTexture2Texture2D(RenderTexture rt)
		{
			RenderTexture currentRT = RenderTexture.active;
			RenderTexture.active = rt;
			Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBAFloat, false);
			tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
			tex.Apply();
			RenderTexture.active = currentRT;
			return tex;
		}

		/// <summary>
		/// Converts a RenderTexture into a Texture2D which already exists.
		/// </summary>
		public static void RenderTexture2Dest(RenderTexture rt, Texture2D dest)
		{
			RenderTexture currentRT = RenderTexture.active;
			RenderTexture.active = rt;
			dest.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
			dest.Apply();
			RenderTexture.active = currentRT;
		}
	}
}

#endif