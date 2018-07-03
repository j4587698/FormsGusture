﻿using System;
using System.ComponentModel;
using System.Windows.Input;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Plugin.FormsGesture.Android;
using Plugin.FormsGesture.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ResolutionGroupName("JX")]
[assembly: ExportEffect(typeof(PlatformGestureEffect), nameof(PlatformGestureEffect))]
namespace Plugin.FormsGesture.Android
{
    public class PlatformGestureEffect : PlatformEffect
    {
        private GestureDetectorCompat gestureRecognizer;

        private ICommand tapCommand;
        private ICommand panCommand;

        private readonly InternalGestureDetector tapDetector;

        private DisplayMetrics displayMetrics;
        private Gesture.PanEventArgs eventArgs = new Gesture.PanEventArgs();

        public PlatformGestureEffect()
        {
            tapDetector = new InternalGestureDetector
            {
                TapAction = motionEvent =>
                {
                    var command = tapCommand;
                    if (command != null)
                    {
                        var x = motionEvent.GetX();
                        var y = motionEvent.GetY();
                        var point = PxToDp(new Point(x, y));
                        if (command.CanExecute(point))
                            command.Execute(point);
                    }
                },
                PanAction = (e1, e2, distanceX, distanceY) =>
                {
                    var command = panCommand;
                    if (command != null)
                    {
                        eventArgs.StartPosition = new Point(e1.GetX(), e1.GetY());
                        eventArgs.CurrentPosition = new Point(e2.GetX(), e2.GetY());
                        eventArgs.StatusType = GestureStatus.Running;
                        eventArgs.TotalMove = new Point(distanceX, distanceY);
                        if (command.CanExecute(eventArgs))
                        {
                            command.Execute(eventArgs);
                        }
                    }
                },
                PanEndAction = (motionEvent) =>
                {
                    var command = panCommand;
                    if (command != null)
                    {
                        eventArgs.CurrentPosition = new Point(motionEvent.GetX(), motionEvent.GetY());
                        eventArgs.StatusType = GestureStatus.Completed;
                        eventArgs.TotalMove = new Point(motionEvent.GetX() - eventArgs.StartPosition.X, motionEvent.GetY() - eventArgs.StartPosition.Y);
                        if (command.CanExecute(eventArgs))
                        {
                            command.Execute(eventArgs);
                        }
                    }
                }
            };
        }

        private Point PxToDp(Point point)
        {
            point.X = point.X / displayMetrics.Density;
            point.Y = point.Y / displayMetrics.Density;
            return point;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            tapCommand = Gesture.GetTapCommand(Element);
            panCommand = Gesture.GetPanCommand(Element);
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;

            var context = control.Context;
            displayMetrics = context.Resources.DisplayMetrics;
            tapDetector.Density = displayMetrics.Density;

            if (gestureRecognizer == null)
                gestureRecognizer = new GestureDetectorCompat(context, tapDetector);
            control.Touch += ControlOnTouch;

            OnElementPropertyChanged(new PropertyChangedEventArgs(String.Empty));
        }

        private void ControlOnTouch(object sender, View.TouchEventArgs touchEventArgs)
        {
            bool detectedUp = touchEventArgs.Event.Action == MotionEventActions.Up;
            if (gestureRecognizer?.OnTouchEvent(touchEventArgs.Event) == false && detectedUp)
            {
                tapDetector.PanEndAction?.Invoke(touchEventArgs.Event);
            }

        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;
            control.Touch -= ControlOnTouch;
        }


        sealed class InternalGestureDetector : GestureDetector.SimpleOnGestureListener
        {

            public float Density { get; set; }

            public Action<MotionEvent> TapAction { get; set; }
            public Action<MotionEvent, MotionEvent, float, float> PanAction { get; set; }
            public Action<MotionEvent> PanEndAction { get; set; }

            public override bool OnSingleTapUp(MotionEvent e)
            {
                TapAction?.Invoke(e);
                return true;
            }

            public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                PanAction?.Invoke(e1, e2, distanceX, distanceY);
                return true;
            }

            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                PanEndAction.Invoke(e2);
                return true;
            }
        }
    }
}