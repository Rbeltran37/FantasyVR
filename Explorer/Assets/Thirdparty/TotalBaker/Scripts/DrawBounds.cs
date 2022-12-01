/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace TB
{

    public class DrawBounds : MonoBehaviour
    {
        public bool showCenter;
        public bool showLabel;
        public Color color = Color.white;

        /// <summary>
        /// When the game object is selected this will draw the gizmos
        /// </summary>
        /// <remarks>Only called when in the Unity editor.</remarks>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = this.color;

            // get renderer bonding box
            var bounds = new Bounds();
            var initBound = false;
            if (GetBoundWithChildren(this.transform, ref bounds, ref initBound))
            {
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }

            if (this.showCenter)
            {
                Gizmos.DrawLine(new Vector3(bounds.min.x, bounds.center.y, bounds.center.z), new Vector3(bounds.max.x, bounds.center.y, bounds.center.z));
                Gizmos.DrawLine(new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), new Vector3(bounds.center.x, bounds.max.y, bounds.center.z));
                Gizmos.DrawLine(new Vector3(bounds.center.x, bounds.center.y, bounds.min.z), new Vector3(bounds.center.x, bounds.center.y, bounds.max.z));
            }

            if (showLabel)
            {
                Handles.BeginGUI();
                SceneView view = SceneView.currentDrawingSceneView;
                Vector3 pos = view.camera.WorldToScreenPoint(bounds.center);
                Vector2 size = GUI.skin.label.CalcSize(new GUIContent(bounds.ToString("F3")));
                GUI.Label(new Rect(pos.x - (size.x / 2), -pos.y + view.position.height + 4, size.x, size.y), bounds.ToString("F3"));
                Handles.EndGUI();
            }
        }

        /// <summary>
        /// Gets the rendering bounds of the transform.
        /// </summary>
        /// <param name="transform">The game object to get the bounding box for.</param>
        /// <param name="pBound">The bounding box reference that will </param>
        /// <param name="encapsulate">Used to determine if the first bounding box to be calculated should be encapsulated into the <see cref="pBound"/> argument.</param>
        /// <returns>Returns true if at least one bounding box was calculated.</returns>
        public static bool GetBoundWithChildren(Transform transform, ref Bounds pBound, ref bool encapsulate)
        {
            Bounds bound;
            var didOne = false;

            // get 'this' bound
            if (transform.gameObject.GetComponent<Renderer>() != null)
            {
                bound = transform.gameObject.GetComponent<Renderer>().bounds;
                if (encapsulate)
                {
                    pBound.Encapsulate(bound.min);
                    pBound.Encapsulate(bound.max);
                }
                else
                {
                    pBound.min = bound.min;
                    pBound.max = bound.max;
                    encapsulate = true;
                }

                didOne = true;
            }

            // union with bound(s) of any/all children
            foreach (Transform child in transform)
            {
                if (GetBoundWithChildren(child, ref pBound, ref encapsulate))
                {
                    didOne = true;
                }
            }

            return didOne;
        }
    }

}

#endif