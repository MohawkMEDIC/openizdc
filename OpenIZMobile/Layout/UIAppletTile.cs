
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
		private const int SZ_SMALL_TILE = 145;
		private const int SZ_LARGE_TILE = 290;


		private float m_touchPointStartX = 0;
		private float m_touchPointStartY = 0;

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
			this.m_applet = applet;

			this.m_outlinePaint = new Paint ();
			this.m_outlinePaint.StrokeWidth = 2.0f;
			this.m_outlinePaint.Color = Color.ParseColor("#2c3e50");
			this.m_outlinePaint.AntiAlias = true;

			this.m_textPaint = new TextPaint ();
			this.m_textPaint.Color = Color.White;
			this.m_textPaint.TextSize = (int)this.ConvertDpToPx(14);
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
				var id = Resources.GetIdentifier (m_currentTile.Icon.Replace ("@drawable/" , ""), "drawable", this.Context.PackageName);
				BitmapDrawable d = Resources.GetDrawable (id) as BitmapDrawable;

				if (this.m_currentTile.Size == AppletTileSize.Small) {
					int szIcon = (int)this.ConvertDpToPx (SZ_SMALL_TILE * 0.75);
					int lfIcon = (int)((canvas.Width - szIcon) / 2);

					var bitmap = Bitmap.CreateScaledBitmap(d.Bitmap, szIcon, szIcon, false);
					canvas.DrawBitmap (bitmap, lfIcon, lfIcon, this.m_outlinePaint);
				}
				else {
					int szIcon = (int)this.ConvertDpToPx (SZ_SMALL_TILE * 0.5);
					var bitmap = Bitmap.CreateScaledBitmap(d.Bitmap, szIcon, szIcon, false);
					canvas.DrawBitmap (bitmap, 10, 10, this.m_outlinePaint);
				}
			}

			// Draw the text for the tile
			using (StaticLayout sl = new StaticLayout (this.m_currentTile.GetText(this.Resources.Configuration.Locale.DisplayLanguage), this.m_textPaint,
				canvas.Width, this.m_currentTile.Size == AppletTileSize.Small ? Android.Text.Layout.Alignment.AlignCenter : Android.Text.Layout.Alignment.AlignNormal, 1, 0, true)) {
				canvas.Translate (this.m_currentTile.Size == AppletTileSize.Small ? 0 : (int)this.ConvertDpToPx(10), canvas.Height - sl.Height - 30);
				sl.Draw (canvas);
				canvas.Restore ();
			}

			base.Draw (canvas);
		}

		/// <param name="e">The motion event.</param>
		/// <summary>
		/// Implement this method to handle touch screen motion events.
		/// </summary>
		/// <returns>To be added.</returns>
		public override bool OnTouchEvent (MotionEvent e)
		{
			// Determine the action
			switch (e.ActionMasked) {
			case MotionEventActions.Down:
				{
					this.m_touchPointStartX = e.GetX();
					this.m_touchPointStartY = e.GetY ();
					return true;
				}
			case MotionEventActions.Up:
				{

					float finalX = e.GetX (),
						finalY = e.GetY ();

					if (Math.Abs (finalX - this.m_touchPointStartX) < 20 &&
					   Math.Abs (finalY - this.m_touchPointStartY) < 20) { // tap
						Intent viewIntent = new Intent (this.Context, typeof(AppletActivity));
						viewIntent.PutExtra ("appletId", this.m_applet.Info.Id.ToString ());
						this.Context.StartActivity (viewIntent);
						return false;
					}
					return true;
				}
			}
			return base.OnTouchEvent (e);
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

