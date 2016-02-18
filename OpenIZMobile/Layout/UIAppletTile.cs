
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using OpenIZ.Mobile.Core;
using OpenIZ.Mobile.Core.Applets;
using Android.Graphics;
using Android.Text;
using Android.Graphics.Drawables;

namespace OpenIZMobile.Layout
{
	/// <summary>
	/// Represents a single tile on the main home page
	/// </summary>
	public class UIAppletTile : View
	{

		// Tile sizes in DP
		private const int SZ_SMALL_TILE = 196;
		private const int SZ_LARGE_TILE = 256;

		// Applet
		private AppletManifest m_applet;

		// Current tile
		private AppletTile m_currentTile;

		// Paint
		private Paint m_outlinePaint;
		private TextPaint m_textPaint;

		/// <summary>
		/// Creates a new instance of hte applet tile based on a manifest
		/// </summary>
		public UIAppletTile (Context context, AppletManifest applet) : base(context)
		{
			// Get the default tile
			// TODO: Load from configuration the tile layout to use, right now they will all be small
			this.m_currentTile = applet.Tiles.FirstOrDefault();


			this.m_outlinePaint = new Paint ();
			this.m_outlinePaint.StrokeWidth = 2.0f;
			this.m_outlinePaint.Color = Color.ParseColor("#2c3e50");
			this.m_outlinePaint.AntiAlias = true;

			this.m_textPaint = new TextPaint ();
			this.m_textPaint.Color = Color.White;
			this.m_textPaint.TextSize = (int)this.ConvertDpToPx(16);
			this.m_textPaint.SetTypeface(Typeface.SansSerif);

		}

		/// <summary>
		/// On measure
		/// </summary>
		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			double width = this.ConvertDpToPx(SZ_SMALL_TILE),
			height = this.ConvertDpToPx(SZ_SMALL_TILE);

			if (this.m_currentTile.Size == AppletTileSize.Large)
				width = this.ConvertDpToPx(SZ_LARGE_TILE);

			this.SetMeasuredDimension (ResolveSize ((int)width, widthMeasureSpec), ResolveSize ((int)height, heightMeasureSpec));

		}

		/// <param name="canvas">The Canvas to which the View is rendered.</param>
		/// <summary>
		/// Draw the child
		/// </summary>
		public override void Draw (Canvas canvas)
		{
			canvas.DrawRect (0, 0, canvas.Width, canvas.Height, this.m_outlinePaint);


			// Draw the image of the tile
			if (m_currentTile.Icon.StartsWith ("@drawable")) { // Builtin
				var id = Resources.GetIdentifier (m_currentTile.Icon.Replace ("@drawable/", ""), "drawable", "org.openiz.openiz_mobile");
				Drawable d = Resources.GetDrawable (id);

				if (this.m_currentTile.Size == AppletTileSize.Small) {
					int szIcon = (int)this.ConvertDpToPx (SZ_SMALL_TILE * 0.75);
					int lfIcon = (int)((canvas.Width - szIcon) / 2);
					d.SetBounds (lfIcon, lfIcon, lfIcon + szIcon, lfIcon + szIcon);
					d.Draw (canvas);
				}

			}

			// Draw the text for the tile
			using (StaticLayout sl = new StaticLayout (this.m_currentTile.Text, this.m_textPaint,
				                        canvas.Width, Android.Text.Layout.Alignment.AlignCenter, 1, 0, true)) {
				canvas.Translate (canvas.Width - sl.Width, canvas.Height - sl.Height - 30);
				sl.Draw (canvas);
				canvas.Restore ();
			}
			base.Draw (canvas);
		}
		


		/// <summary>
		/// Convert pixels to DP
		/// </summary>
		private double ConvertDpToPx(double dp){
			var resources = this.Context.Resources;
			DisplayMetrics metrics = resources.DisplayMetrics;
			double px = dp * ((int)metrics.DensityDpi / (int)DisplayMetrics.DensityDefault);
			return px;
		}
	}
}

