/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace TB
{

  public class HorizontalPaneState
  {
    public const int SPLITTER_WIDTH = 9;
    public int id = 0;

    public bool isDraggingSplitter = false,
      isPaneWidthChanged = false;

    public float leftPaneWidth = -1,
      initialLeftPaneWidth = -1,
      lastAvailableWidth = -1,
      availableWidth = 0,
      minPaneWidthLeft = 75,
      minPaneWidthRight = 75;

    /*
    * Unity can, apparently, recycle state objects.  In that event we want to
    * wipe the slate clean and just start over to avoid wackiness.
    */
    protected virtual void Reset(int newId)
    {
      id = newId;
      isDraggingSplitter = false;
      isPaneWidthChanged = false;
      leftPaneWidth = -1;
      initialLeftPaneWidth = -1;
      lastAvailableWidth = -1;
      availableWidth = 0;
      minPaneWidthLeft = 75;
      minPaneWidthRight = 75;
    }

    /*
    * Some aspects of our state are really just static configuration that
    * shouldn't be modified by the control, so we blindly set them if we have a
    * prototype from which to do so.
    */
    protected virtual void InitFromPrototype(int newId, HorizontalPaneState prototype)
    {
      id = newId;
      initialLeftPaneWidth = prototype.initialLeftPaneWidth;
      minPaneWidthLeft = prototype.minPaneWidthLeft;
      minPaneWidthRight = prototype.minPaneWidthRight;
    }

    /*
    * This method takes care of guarding against state object recycling, and
    * ensures we pick up what we need, when we need to, from the prototype state
    * object.
    */
    public void ResolveStateToCurrentContext(int currentId, HorizontalPaneState prototype)
    {
      if (id != currentId)
        Reset(currentId);
      else if (prototype != null)
        InitFromPrototype(currentId, prototype);
    }
  }

  public static class EditorGUILayoutHorizontalPanes
  {
    // TODO: This makes it impossible to nest pane sets!
    private static HorizontalPaneState hState;

    public static void Begin()
    {
      Begin(null);
    }

    public static void Begin(HorizontalPaneState prototype)
    {
      int id = GUIUtility.GetControlID(FocusType.Passive);
      hState = (HorizontalPaneState) GUIUtility.GetStateObject(typeof(HorizontalPaneState), id);
      hState.ResolveStateToCurrentContext(id, prototype);

      // *INDENT-OFF*
      Rect totalArea = EditorGUILayout.BeginHorizontal();
      hState.availableWidth = totalArea.width - HorizontalPaneState.SPLITTER_WIDTH;
      hState.isPaneWidthChanged = false;
      if (totalArea.width > 0)
      {
        if (hState.leftPaneWidth < 0)
        {
          if (hState.initialLeftPaneWidth < 0)
            hState.leftPaneWidth = hState.availableWidth * 0.5f;
          else
            hState.leftPaneWidth = hState.initialLeftPaneWidth;
          hState.isPaneWidthChanged = true;
        }

        if (hState.lastAvailableWidth < 0)
          hState.lastAvailableWidth = hState.availableWidth;
        if (Mathf.Approximately(hState.lastAvailableWidth, hState.availableWidth))
        {
          hState.leftPaneWidth = hState.availableWidth * (hState.leftPaneWidth / hState.lastAvailableWidth);
          hState.isPaneWidthChanged = true;
        }

        hState.lastAvailableWidth = hState.availableWidth;
      }

      GUILayout.BeginHorizontal(GUILayout.Width(hState.leftPaneWidth));
      // *INDENT-ON*
    }

    public static void Splitter()
    {
      GUILayout.EndHorizontal();

      float availableWidthForOnePanel = hState.availableWidth - (1 + hState.minPaneWidthRight);
      Rect drawableSplitterArea = GUILayoutUtility.GetRect(GUIHelper.NoContent, HorizontalPaneStyles.Splitter, GUILayout.Width(1f), GUIHelper.ExpandHeight);
      Rect splitterArea = new Rect(drawableSplitterArea.xMin - (int) (HorizontalPaneState.SPLITTER_WIDTH * 0.5f), drawableSplitterArea.yMin, HorizontalPaneState.SPLITTER_WIDTH, drawableSplitterArea.height);
      switch (Event.current.type)
      {
        case EventType.MouseDown:
          if (splitterArea.Contains(Event.current.mousePosition))
          {
            hState.isDraggingSplitter = true;
            GUIUtility.hotControl = hState.id;
            Event.current.Use();
          }

          break;
        case EventType.MouseDrag:
          if (hState.isDraggingSplitter && hState.id == GUIUtility.hotControl)
          {
            hState.leftPaneWidth += Event.current.delta.x;
            hState.leftPaneWidth = Mathf.Round(hState.leftPaneWidth);
            hState.isPaneWidthChanged = true;
            Event.current.Use();
          }

          break;
        case EventType.MouseUp:
          hState.isDraggingSplitter = false;
          if (hState.id == GUIUtility.hotControl)
          {
            GUIUtility.hotControl = 0;
            Event.current.Use();
          }

          break;
      }

      if (hState.isPaneWidthChanged)
      {
        if (hState.leftPaneWidth < hState.minPaneWidthLeft)
          hState.leftPaneWidth = hState.minPaneWidthLeft;
        if (hState.leftPaneWidth >= availableWidthForOnePanel)
          hState.leftPaneWidth = availableWidthForOnePanel;
        if (EditorWindow.focusedWindow != null)
          EditorWindow.focusedWindow.Repaint();
      }

      GUI.Label(drawableSplitterArea, GUIHelper.NoContent, HorizontalPaneStyles.Splitter);
      EditorGUIUtility.AddCursorRect(splitterArea, MouseCursor.ResizeHorizontal);
    }

    public static void End()
    {
      EditorGUILayout.EndHorizontal();
    }
  }

  public static class HorizontalPaneStyles
  {
    private static Texture2D SplitterImage;

    static HorizontalPaneStyles()
    {
      // TODO: Change the image color based on chosen editor skin.
      SplitterImage = new Texture2D(1, 1, TextureFormat.ARGB32, false)
      {
        hideFlags = HideFlags.HideAndDontSave,
        anisoLevel = 0,
        filterMode = FilterMode.Point,
        wrapMode = TextureWrapMode.Clamp
      };
      SplitterImage.SetPixels(new[] {Color.gray});
      SplitterImage.Apply();
    }

    private static GUIStyle _Splitter = null;

    public static GUIStyle Splitter
    {
      get
      {
        if (_Splitter == null)
        {
          // *INDENT-OFF*
          _Splitter = new GUIStyle()
            {
              normal = new GUIStyleState() {background = SplitterImage},
              imagePosition = ImagePosition.ImageOnly,
              wordWrap = false,
              alignment = TextAnchor.MiddleCenter,
            }
            .Named("HSplitter")
            .Size(1, 0, false, true)
            .ResetBoxModel()
            .Margin(3, 3, 0, 0)
            .ClipText();
          // *INDENT-ON*
        }

        return _Splitter;
      }
    }
  }

  public static class GUIHelper
  {
    // Don't create new instances every time we need an option -- just pre-create the permutations:
    public static GUILayoutOption ExpandWidth = GUILayout.ExpandWidth(true),
      NoExpandWidth = GUILayout.ExpandWidth(false),
      ExpandHeight = GUILayout.ExpandHeight(true),
      NoExpandHeight = GUILayout.ExpandHeight(false);

    // Provided for consistency of interface, but not actually a savings/win:
    public static GUILayoutOption Width(float w)
    {
      return GUILayout.Width(w);
    }

    // Again, don't create instances when we don't need to:
    public static GUIStyle NoStyle = GUIStyle.none;
    public static GUIContent NoContent = GUIContent.none;
  }

  public static class GUIStyleExtensions
  {
    public static GUIStyle NoBackgroundImages(this GUIStyle style)
    {
      style.normal.background =
        style.active.background =
          style.hover.background =
            style.focused.background =
              style.onNormal.background =
                style.onActive.background =
                  style.onHover.background =
                    style.onFocused.background =
                      null;
      return style;
    }

    public static GUIStyle BaseTextColor(this GUIStyle style, Color c)
    {
      // *INDENT-OFF*
      style.normal.textColor =
        style.active.textColor =
          style.hover.textColor =
            style.focused.textColor =
              style.onNormal.textColor =
                style.onActive.textColor =
                  style.onHover.textColor =
                    style.onFocused.textColor =
                      c;
      // *INDENT-ON*
      return style;
    }

    public static GUIStyle ResetBoxModel(this GUIStyle style)
    {
      style.border = new RectOffset();
      style.margin = new RectOffset();
      style.padding = new RectOffset();
      style.overflow = new RectOffset();
      style.contentOffset = Vector2.zero;

      return style;
    }

    public static GUIStyle Padding(this GUIStyle style, int left, int right, int top, int bottom)
    {
      style.padding = new RectOffset(left, right, top, bottom);

      return style;
    }

    public static GUIStyle Margin(this GUIStyle style, int left, int right, int top, int bottom)
    {
      style.margin = new RectOffset(left, right, top, bottom);

      return style;
    }

    public static GUIStyle Border(this GUIStyle style, int left, int right, int top, int bottom)
    {
      style.border = new RectOffset(left, right, top, bottom);

      return style;
    }

    public static GUIStyle Named(this GUIStyle style, string name)
    {
      style.name = name;

      return style;
    }

    public static GUIStyle ClipText(this GUIStyle style)
    {
      style.clipping = TextClipping.Clip;

      return style;
    }

    public static GUIStyle Size(this GUIStyle style, int width, int height, bool stretchWidth, bool stretchHeight)
    {
      style.fixedWidth = width;
      style.fixedHeight = height;
      style.stretchWidth = stretchWidth;
      style.stretchHeight = stretchHeight;

      return style;
    }
  }

}

#endif