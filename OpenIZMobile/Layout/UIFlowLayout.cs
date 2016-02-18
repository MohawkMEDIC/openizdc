using System;
using Android.Views;
using Android.Graphics;
using Android.Content;
using Android.Util;
using Android.Content.Res;

namespace OpenIZMobile.Layout
{
	/// <summary>
	/// Represents a flow layout used for rendering tiles
	/// </summary>
	public class UIFlowLayout : ViewGroup
	{

		// Padding
		private int m_horizontalPadding ;
		private int m_verticalPadding;

		// Paint
		private Paint m_paint;


		public UIFlowLayout (Context context, IAttributeSet attrs) : base(context, attrs)
		{

			// Get styled attributes
			using(TypedArray ta = context.ObtainStyledAttributes (attrs, Resource.Styleable.FlowLayout))
			{
				this.m_horizontalPadding = ta.GetDimensionPixelSize(Resource.Styleable.FlowLayout_horizontalSpacing, 0);
				this.m_verticalPadding = ta.GetDimensionPixelSize(Resource.Styleable.FlowLayout_verticalSpacing, 0);
			}

			this.m_paint = new Paint ();
			this.m_paint.AntiAlias = true;
			this.m_paint.Color = Color.White;
			this.m_paint.StrokeWidth = 2.0f;
		}

		/// <summary>
		/// On measuring
		/// </summary>
		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			int wSize = MeasureSpec.GetSize (widthMeasureSpec) - this.PaddingRight;
			var wMode = MeasureSpec.GetMode (widthMeasureSpec);

			bool growHeight = wMode != MeasureSpecMode.Unspecified;
			int width = 0;
			int height = this.PaddingTop;

			int cWidth = this.PaddingLeft;
			int cHeight = 0;

			bool breakLine = false;
			bool newLine = false;
			int spacing = 0;

			for (int i = 0; i < this.ChildCount; i++) {

				// Get & measure the child
				View child = this.GetChildAt (i);
				this.MeasureChild (child, widthMeasureSpec, heightMeasureSpec);
				LayoutParams lParms = (LayoutParams)child.LayoutParameters;

				spacing = this.m_horizontalPadding;
				if (lParms.HorizontalSpacing >= 0)
					spacing = lParms.HorizontalSpacing;
				
				// Determine if a new line needs to be creted
				if (growHeight && (breakLine || cWidth + child.MeasuredWidth > wSize)) {
					height += cHeight + this.m_verticalPadding;
					cHeight = 0;
					width = Math.Max (width, cWidth - spacing);
					cWidth = this.PaddingLeft;
					newLine = true;
				} else
					newLine = false;

				// Calculate X/Y
				lParms.X = cWidth;
				lParms.Y = height;

				cWidth += child.MeasuredWidth + spacing;
				cHeight = Math.Max (cHeight, child.MeasuredHeight);
				breakLine = lParms.BreakLine;
			}

			// No new line re-calc
			if (!newLine) {
				height += cHeight;
				width = Math.Max (width, cWidth - spacing);
			}

			width += PaddingRight;
			height += PaddingBottom;
			this.SetMeasuredDimension (ResolveSize (width, widthMeasureSpec), ResolveSize (height, heightMeasureSpec));

		}

		/// <param name="changed">This is a new size or position for this view</param>
		/// <param name="left">Left position, relative to parent</param>
		/// <param name="top">Top position, relative to parent</param>
		/// <param name="right">Right position, relative to parent</param>
		/// <param name="bottom">Bottom position, relative to parent</param>
		/// <summary>
		/// Perform a layout
		/// </summary>
		protected override void OnLayout (bool changed, int left, int top, int right, int bottom)
		{
			for (int i = 0; i < this.ChildCount; i++) {
				View child = this.GetChildAt (i);
				var lParms = (LayoutParams)child.LayoutParameters;
				child.Layout (lParms.X + this.m_horizontalPadding, lParms.Y, lParms.X + child.MeasuredWidth, lParms.Y + child.MeasuredHeight);
			}
		} 

		/// <param name="canvas">The canvas on which to draw the child</param>
		/// <param name="child">Who to draw</param>
		/// <param name="drawingTime">The time at which draw is occurring</param>
		/// <summary>
		/// Draw the child
		/// </summary>
		protected override bool DrawChild (Canvas canvas, View child, long drawingTime)
		{
			bool more = base.DrawChild (canvas, child, drawingTime);
			var lParms = (LayoutParams)child.LayoutParameters;
			if (lParms.HorizontalSpacing > 0) {
				float x = child.Right;
				float y = child.Top + child.Height / 2.0f;
				//canvas.DrawLine (x, y - 4.0f, x, y + 4.0f, this.m_paint);
				//canvas.DrawLine (x, y, x + lParms.HorizontalSpacing, y, this.m_paint);
				//canvas.DrawLine (x + lParms.HorizontalSpacing, y - 4.0f, x + lParms.HorizontalSpacing, y + 4.0f, this.m_paint);
			}
			if (lParms.BreakLine) {
				float x = child.Right;
				float y = child.Top + child.Height / 2.0f;
				//canvas.DrawLine (x, y, x, y + 6.0f, this.m_paint);
				//canvas.DrawLine (x, y + 6.0f, x + 6.0f, y + 6.0f, this.m_paint);
			}
			return more;
		}


		/// <summary>
		/// Check layout parameters
		/// </summary>
		/// <returns><c>true</c>, if layout parameters was checked, <c>false</c> otherwise.</returns>
		protected override bool CheckLayoutParams (ViewGroup.LayoutParams p)
		{
			return p is UIFlowLayout.LayoutParams;
		}

		/// <summary>
		/// Generate default layour params
		/// </summary>
		protected override ViewGroup.LayoutParams GenerateDefaultLayoutParams ()
		{
			return new LayoutParams (LayoutParams.WrapContent, LayoutParams.WrapContent);
		}

		/// <param name="attrs">the attributes to build the layout parameters from</param>
		/// <summary>
		/// Returns a new set of layout parameters based on the supplied attributes set.
		/// </summary>
		public override ViewGroup.LayoutParams GenerateLayoutParams (IAttributeSet attrs)
		{
			return new LayoutParams (this.Context, attrs);
		}

		/// <summary>
		/// Generates the layout parameters.
		/// </summary>
		/// <returns>The layout parameters.</returns>
		/// <param name="p">P.</param>
		protected override ViewGroup.LayoutParams GenerateLayoutParams (ViewGroup.LayoutParams p)
		{
			return new LayoutParams (p.Width, p.Height);
		}

		/// <summary>
		/// Layout parameters extension
		/// </summary>
		public new class LayoutParams : ViewGroup.LayoutParams {

			/// <summary>
			/// Gets or sets the X coordinate
			/// </summary>
			public int X {
				get;
				set;
			}

			/// <summary>
			/// Gets or sets the Y coordinate
			/// </summary>
			public int Y {
				get;
				set;
			}

			/// <summary>
			/// Gets the horizontal spacking
			/// </summary>
			public int HorizontalSpacing {
				get;
				set;
			}

			/// <summary>
			/// Break line
			/// </summary>
			public bool BreakLine {
				get;
				set;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="OpenIZMobile.Fragment.LayoutParams"/> class.
			/// </summary>
			public LayoutParams(Context context, IAttributeSet attrs) : base(context, attrs) {
				
				using(TypedArray ta = context.ObtainStyledAttributes(attrs, Resource.Styleable.FlowLayout_LayoutParams))
				{
					this.HorizontalSpacing = ta.GetDimensionPixelSize(Resource.Styleable.FlowLayout_LayoutParams_layout_horizontalSpacing, -1);
					this.BreakLine = ta.GetBoolean(Resource.Styleable.FlowLayout_LayoutParams_layout_breakLine, false);
				} 
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="OpenIZMobile.Fragment.LayoutParams"/> class.
			/// </summary>
			public LayoutParams(int w, int h) : base(w, h) {
			}
		}
		
	}
}

