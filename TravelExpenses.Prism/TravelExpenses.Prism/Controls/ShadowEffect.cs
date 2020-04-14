using System;
using System.Linq;
using Xamarin.Forms;

namespace TravelExpenses.Prism.Controls
{
	public static class ViewEffects
	{
		public static readonly BindableProperty HasShadowProperty =
			BindableProperty.CreateAttached("HasShadow", typeof(bool), typeof(ViewEffects), false, propertyChanged: OnHasShadowChanged);

		private static void OnHasShadowChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var view = bindable as View;
			if (view == null)
				return;

			var hasShadow = (bool)newValue;
			if (hasShadow)
			{
				view.Effects.Add(new ShadowEffect());
			}
			else
			{
				var toRemove = view.Effects.FirstOrDefault(e => e is ShadowEffect);
				if (toRemove != null)
					view.Effects.Remove(toRemove);
			}
		}

		public static readonly BindableProperty ShadowSizeProperty =
			BindableProperty.CreateAttached("ShadowSize", typeof(double), typeof(ViewEffects), 0d);

		public static readonly BindableProperty ShadowColorProperty =
			BindableProperty.CreateAttached("ShadowColor", typeof(Color), typeof(ViewEffects), Color.Default);

		public static void SetHasShadow(BindableObject view, bool hasShadow)
		{
			view.SetValue(HasShadowProperty, hasShadow);
		}

		public static bool GetHasShadow(BindableObject view)
		{
			return (bool)view.GetValue(HasShadowProperty);
		}

		public static void SetShadowSize(BindableObject view, double size)
		{
			view.SetValue(ShadowSizeProperty, size);
		}

		public static double GetShadowSize(BindableObject view)
		{
			return (double)view.GetValue(ShadowSizeProperty);
		}

		public static void SetShadowColor(BindableObject view, Color color)
		{
			view.SetValue(ShadowColorProperty, color);
		}

		public static Color GetShadowColor(BindableObject view)
		{
			return (Color)view.GetValue(ShadowColorProperty);
		}

		class ShadowEffect : RoutingEffect
		{
			public ShadowEffect() : base("Xamarin.ShadowEffect")
			{

			}
		}
	}
}
