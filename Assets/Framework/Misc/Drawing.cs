namespace Andoco.Unity.Framework.Misc
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class Drawing
	{
	    //****************************************************************************************************
	    //  static function DrawLine(rect : Rect) : void
	    //  static function DrawLine(rect : Rect, color : Color) : void
	    //  static function DrawLine(rect : Rect, width : float) : void
	    //  static function DrawLine(rect : Rect, color : Color, width : float) : void
	    //  static function DrawLine(Vector2 pointA, Vector2 pointB) : void
	    //  static function DrawLine(Vector2 pointA, Vector2 pointB, color : Color) : void
	    //  static function DrawLine(Vector2 pointA, Vector2 pointB, width : float) : void
	    //  static function DrawLine(Vector2 pointA, Vector2 pointB, color : Color, width : float) : void
	    //  
	    //  Draws a GUI line on the screen.
	    //  
	    //  DrawLine makes up for the severe lack of 2D line rendering in the Unity runtime GUI system.
	    //  This function works by drawing a 1x1 texture filled with a color, which is then scaled
	    //   and rotated by altering the GUI matrix.  The matrix is restored afterwards.
	    //****************************************************************************************************
	    
	    public static Texture2D lineTex;
	    
	    public static void DrawLine(Rect rect) { DrawLine(rect, GUI.contentColor, 1.0f); }
	    public static void DrawLine(Rect rect, Color color) { DrawLine(rect, color, 1.0f); }
	    public static void DrawLine(Rect rect, float width) { DrawLine(rect, GUI.contentColor, width); }
	    public static void DrawLine(Rect rect, Color color, float width) { DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
	    public static void DrawLine(Vector2 pointA, Vector2 pointB) { DrawLine(pointA, pointB, GUI.contentColor, 1.0f); }
	    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color) { DrawLine(pointA, pointB, color, 1.0f); }
	    public static void DrawLine(Vector2 pointA, Vector2 pointB, float width) { DrawLine(pointA, pointB, GUI.contentColor, width); }
	    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
	    {
	        // Save the current GUI matrix, since we're going to make changes to it.
	        Matrix4x4 matrix = GUI.matrix;
	        
	        // Generate a single pixel texture if it doesn't exist
	        if (!lineTex) { lineTex = new Texture2D(1, 1); }
	        
	        // Store current GUI color, so we can switch it back later,
	        // and set the GUI color to the color parameter
	        Color savedColor = GUI.color;
	        GUI.color = color;
	        
	        // Determine the angle of the line.
	        float angle = Vector3.Angle(pointB - pointA, Vector2.right);
	        
	        // Vector3.Angle always returns a positive number.
	        // If pointB is above pointA, then angle needs to be negative.
	        if (pointA.y > pointB.y) { angle = -angle; }
	        
	        // Use ScaleAroundPivot to adjust the size of the line.
	        // We could do this when we draw the texture, but by scaling it here we can use
	        //  non-integer values for the width and length (such as sub 1 pixel widths).
	        // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
	        //  is centered on the origin at pointA.
	        GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
	        
	        // Set the rotation for the line.
	        //  The angle was calculated with pointA as the origin.
	        GUIUtility.RotateAroundPivot(angle, pointA);
	        
	        // Finally, draw the actual line.
	        // We're really only drawing a 1x1 texture from pointA.
	        // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
	        //  render with the proper width, length, and angle.
	        GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), lineTex);
	        
	        // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
	        GUI.matrix = matrix;
	        GUI.color = savedColor;
	    }
	    
	    public static void DrawChart(IEnumerable<float> values, ChartConfig config, Color? color = null)
	    {
	        color = color ?? Color.yellow;
	        var baseX = config.Rect.x;
	        var baseY = config.Rect.y;
	        var w = config.Rect.width;
	        var h = config.Rect.height;

	        const float lineWidth = 1f;
	        var previousPos = new Vector2(baseX, baseY + h);
	        var i = 1;
	        
	        foreach (var val in values.Skip(1))
	        {
	            var x = baseX + i * (w / config.HorizontalScale);
	            var y = (baseY + h) - val * (h / config.VerticalScale);
	            var pos = new Vector2(x, y);
	            
	            Drawing.DrawLine(previousPos, pos, color.Value, lineWidth);
	            
	            previousPos = pos;
	            i++;
	        }
	    }
	    
	    public static void DrawChartGrid(ChartConfig config)
	    {
	        var baseX = config.Rect.x;
	        var baseY = config.Rect.y;
	        var w = config.Rect.width;
	        var h = config.Rect.height;
	        const float lineWidth = 1f;

	        var yStep = h / config.VerticalScale * config.VerticalMarkers;
	        var y = baseY;
	        var i = 0;

	        var lblStyle = new GUIStyle();
	        lblStyle.normal.textColor = Color.white;

	        while (y <= baseY + h)
	        {
	            var p1 = new Vector2(baseX, y);
	            var p2 = new Vector2(baseX + w, y);
	            Drawing.DrawLine(p1, p2, config.MarkerColor, lineWidth);
	            y += yStep;

	            if (i % 2 == 0)
	            {
	                var lbl = (config.VerticalScale - i * config.VerticalMarkers).ToString();
	                var lblRect = GUILayoutUtility.GetRect(new GUIContent(lbl), lblStyle);
	                GUI.Label(new Rect(p1.x - 10, p1.y - lblRect.height / 2f, lblRect.height, lblRect.width), lbl, lblStyle);
	            }
	                
	            i++;
	        }
	        
	        var xStep = w / config.HorizontalScale * config.HorizontalMarkers;
	        var x = baseX;
	        while (x <= baseX + w)
	        {
	            var p1 = new Vector2(x, baseY);
	            var p2 = new Vector2(x, baseY + h);
	            Drawing.DrawLine(p1, p2, config.MarkerColor, lineWidth);
	            x += xStep;
	        }
	    }

	    public static void DrawColliderBoundsGizmos(Collider c)
	    {
	        Gizmos.color = Color.red;
	        Gizmos.DrawWireSphere(c.bounds.center, 2f);
	        Gizmos.color = Color.yellow;
	        Gizmos.DrawWireCube(c.bounds.center, c.bounds.extents * 2.01f);
	        Gizmos.color = Color.blue;
	        Gizmos.DrawWireCube(c.bounds.center, c.bounds.size * 1.02f);
	        
	        Gizmos.color = Color.white;
	        Gizmos.DrawWireSphere(c.bounds.min, 0.5f);
	        Gizmos.DrawWireSphere(c.bounds.max, 0.5f);
	    }

		#region Inner classes

		public sealed class ChartConfig
		{
			public ChartConfig()
			{
				Rect = new Rect(0f, 0f, 400f, 200f);
				VerticalScale = 10f;
				HorizontalScale = 100f;
				VerticalMarkers = 10f;
				HorizontalMarkers = 10f;
				MarkerColor = Color.grey;
			}
			
			public Rect Rect { get; set; }
			
			public float VerticalScale { get; set; }
			
			public float VerticalMarkers { get; set; }
			
			public float HorizontalScale { get; set; }
			
			public float HorizontalMarkers { get; set; }
			
			public Color MarkerColor { get; set; }
		}

		#endregion
	}
}