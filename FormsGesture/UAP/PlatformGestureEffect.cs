using System;
using System.ComponentModel;
using System.Windows.Input;
using Windows.Graphics.Display;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;
using Plugin.FormsGesture.Shared;
using Plugin.FormsGesture.UAP;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Xamarin.Forms.PlatformConfiguration;

[assembly: ResolutionGroupName("JX")]
[assembly: ExportEffect(typeof(PlatformGestureEffect), nameof(PlatformGestureEffect))]
namespace Plugin.FormsGesture.UAP
{
    public class PlatformGestureEffect : PlatformEffect
    {
        private Windows.UI.Input.GestureRecognizer detector;

        private ICommand tapCommand;
        private ICommand panCommand;

        private float density;

        private Gesture.PanEventArgs eventArgs = new Gesture.PanEventArgs();

        public PlatformGestureEffect()
        {
            detector = new Windows.UI.Input.GestureRecognizer
            {
                GestureSettings = GestureSettings.Tap | GestureSettings.Drag | GestureSettings.ManipulationTranslateInertia,
                ShowGestureFeedback = false,
            };
            detector.Tapped += (sender, args) =>
            {
                TriggerCommand(tapCommand, GetScaledCoord(args.Position.X, args.Position.Y));
            };
            detector.Dragging += (sender, args) =>
            {
                switch (args.DraggingState)
                {
                    case DraggingState.Started:
                        eventArgs.StartPosition = GetScaledCoord(args.Position.X, args.Position.Y);
                        eventArgs.StatusType = GestureStatus.Running;
                        break;
                    case DraggingState.Continuing:
                        eventArgs.StatusType = GestureStatus.Running;
                        break;
                    case DraggingState.Completed:
                        eventArgs.StatusType = GestureStatus.Completed;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                eventArgs.CurrentPosition = GetScaledCoord(args.Position.X, args.Position.Y);
                eventArgs.TotalMove = GetScaledCoord(eventArgs.CurrentPosition.X - eventArgs.StartPosition.X, eventArgs.CurrentPosition.Y - eventArgs.StartPosition.Y);
                TriggerCommand(panCommand, eventArgs);
            };
        }

        private Point GetScaledCoord(double x, double y)
        {
            if (Gesture.GetIgnoreDensity(Element))
            {
                x = x * density;
                y = y * density;
            }

            return new Point(x, y);
        }

        private void TriggerCommand(ICommand command, object parameter)
        {
            if (command?.CanExecute(parameter) == true)
                command.Execute(parameter);
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            tapCommand = Gesture.GetTapCommand(Element);
            panCommand = Gesture.GetPanCommand(Element);
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;

            control.PointerMoved -= ControlOnPointerMoved;
            control.PointerPressed -= ControlOnPointerPressed;
            control.PointerReleased -= ControlOnPointerReleased;
            control.PointerCanceled -= ControlOnPointerCanceled;
            control.PointerCaptureLost -= ControlOnPointerCanceled;

            control.PointerMoved += ControlOnPointerMoved;
            control.PointerPressed += ControlOnPointerPressed;
            control.PointerReleased += ControlOnPointerReleased;
            control.PointerCanceled += ControlOnPointerCanceled;
            control.PointerCaptureLost += ControlOnPointerCanceled;
            //control.PointerExited += ControlOnPointerCanceled;

            density = DisplayInformation.GetForCurrentView().LogicalDpi / 96;

            OnElementPropertyChanged(new PropertyChangedEventArgs(String.Empty));
        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;
            control.PointerMoved -= ControlOnPointerMoved;
            control.PointerPressed -= ControlOnPointerPressed;
            control.PointerReleased -= ControlOnPointerReleased;
            control.PointerCanceled -= ControlOnPointerCanceled;
            control.PointerCaptureLost -= ControlOnPointerCanceled;
            //control.PointerExited -= ControlOnPointerCanceled;
        }

        private void ControlOnPointerPressed(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            detector.CompleteGesture();
            detector.ProcessDownEvent(pointerRoutedEventArgs.GetCurrentPoint(Control ?? Container));
            pointerRoutedEventArgs.Handled = true;
        }

        private void ControlOnPointerMoved(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            detector.ProcessMoveEvents(pointerRoutedEventArgs.GetIntermediatePoints(Control ?? Container));
            pointerRoutedEventArgs.Handled = true;
        }

        private void ControlOnPointerCanceled(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            detector.CompleteGesture();
            pointerRoutedEventArgs.Handled = true;
        }

        private void ControlOnPointerReleased(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            detector.ProcessUpEvent(pointerRoutedEventArgs.GetCurrentPoint(Control ?? Container));
            pointerRoutedEventArgs.Handled = true;
        }
    }
}