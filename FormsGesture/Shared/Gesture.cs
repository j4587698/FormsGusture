using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Plugin.FormsGesture.Shared
{
    /// <summary>
    /// Gesture class
    /// </summary>
    public static class Gesture
    {
        public static readonly BindableProperty PanCommandProperty = BindableProperty.CreateAttached("PanCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);

        public static readonly BindableProperty TapCommandProperty = BindableProperty.CreateAttached("TapCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static ICommand GetPanCommand(BindableObject view)
        {
            return (ICommand)view.GetValue(PanCommandProperty);
        }

        public static void SetPanCommand(BindableObject view, ICommand value)
        {
            view.SetValue(PanCommandProperty, value);
        }      

        public static ICommand GetTapCommand(BindableObject view)
        {
            return (ICommand)view.GetValue(TapCommandProperty);
        }

        public static void SetTapCommand(BindableObject view, ICommand value)
        {
            view.SetValue(TapCommandProperty, value);
        }

        private static GestureEffect GetOrCreateEffect(View view)
        {
            var effect = (GestureEffect)view.Effects.FirstOrDefault(e => e is GestureEffect);
            if (effect == null)
            {
                effect = new GestureEffect();
                view.Effects.Add(effect);
            }
            return effect;
        }

        private static void CommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is View view)
            {
                var effect = GetOrCreateEffect(view);
            }
        }

        class GestureEffect : RoutingEffect
        {
            public GestureEffect() : base("JX.PlatformGestureEffect")
            {
            }
        }

        public class PanEventArgs : EventArgs
        {
            public Point StartPosition { get; set; }

            public Point CurrentPosition { get; set; }

            public Point TotalMove { get; set; }

            public GestureStatus StatusType { get; set; }
        }
    }
}