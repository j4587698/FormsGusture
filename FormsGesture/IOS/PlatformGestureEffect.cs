using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Foundation;
using Plugin.FormsGesture.IOS;
using Plugin.FormsGesture.Shared;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("JX")]
[assembly: ExportEffect(typeof(PlatformGestureEffect), nameof(PlatformGestureEffect))]
namespace Plugin.FormsGesture.IOS
{
    public class PlatformGestureEffect : PlatformEffect
    {
        private readonly UITapGestureRecognizer tapDetector;
        private readonly UIPanGestureRecognizer panDetector;
        private readonly List<UIGestureRecognizer> recognizers;

        private ICommand tapCommand;
        private ICommand panCommand;

        private Gesture.PanEventArgs eventArgs = new Gesture.PanEventArgs();

        public PlatformGestureEffect()
        {
            tapDetector = CreateTapRecognizer(() => tapCommand);

            panDetector = CreatePanRecognizer(() => panCommand);

            recognizers = new List<UIGestureRecognizer>
            {
                tapDetector, panDetector
            };
        }

        private UITapGestureRecognizer CreateTapRecognizer(Func<ICommand> getCommand)
        {
            return new UITapGestureRecognizer(recognizer =>
            {
                var handler = getCommand();
                if (handler != null)
                {
                    var control = Control ?? Container;
                    var point = recognizer.LocationInView(control);
                    var pt = GetScaledCoord(point.X, point.Y);
                    if (handler.CanExecute(pt))
                    {
                        handler.Execute(pt);
                    }
                }
            })
            {
                Enabled = false,
                ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true,
                //ShouldReceiveTouch = (recognizer, touch) => true,
            };
        }

        private UIPanGestureRecognizer CreatePanRecognizer(Func<ICommand> getCommand)
        {
            return new UIPanGestureRecognizer(recognizer =>
            {
                var handler = getCommand();
                if (handler != null)
                {
                    var control = Control ?? Container;
                    var point = recognizer.LocationInView(control);
                    var pt = GetScaledCoord(point.X, point.Y);
                    switch (recognizer.State)
                    {
                        case UIGestureRecognizerState.Began:
                            eventArgs.StartPosition = pt;
                            eventArgs.StatusType = GestureStatus.Running;
                            break;
                        case UIGestureRecognizerState.Changed:
                            eventArgs.StatusType = GestureStatus.Running;
                            break;
                        case UIGestureRecognizerState.Ended:
                            eventArgs.StatusType = GestureStatus.Completed;

                            break;
                        case UIGestureRecognizerState.Cancelled:
                            eventArgs.StatusType = GestureStatus.Completed;
                            break;
                    }
                    eventArgs.CurrentPosition = pt;
                    eventArgs.TotalMove = GetScaledCoord(pt.X - eventArgs.StartPosition.X, pt.Y - eventArgs.StartPosition.Y);
                    if (handler.CanExecute(eventArgs))
                    {
                        handler.Execute(eventArgs);
                    }
                }
            })
            {
                Enabled = false,
                ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true,
                MaximumNumberOfTouches = 1,
            };
        }

        private Point GetScaledCoord(double x, double y)
        {
            if (Gesture.GetIgnoreDensity(Element))
            {
                x = x * UIScreen.MainScreen.Scale;
                y = y * UIScreen.MainScreen.Scale;
            }

            return new Point(x, y);
        }


        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            tapCommand = Gesture.GetTapCommand(Element);
            panCommand = Gesture.GetPanCommand(Element);
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;
            control.UserInteractionEnabled = true;

            foreach (var recognizer in recognizers)
            {
                recognizer.Enabled = false;
                control.RemoveGestureRecognizer(recognizer);
            }

            foreach (var recognizer in recognizers)
            {
                control.AddGestureRecognizer(recognizer);
                recognizer.Enabled = true;
            }

            OnElementPropertyChanged(new PropertyChangedEventArgs(String.Empty));
        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;
            foreach (var recognizer in recognizers)
            {
                recognizer.Enabled = false;
                control.RemoveGestureRecognizer(recognizer);
            }
        }
    }
}