# FormsGusture
a tap pan gesture for xamarin.forms.support ios android and uap

##useage:
it's so easy
  xmlns:gesture="clr-namespace:Plugin.FormsGesture.Shared;assembly=Plugin.FormsGesture"
  
  gesture:Gesture.TapCommand="{Binding TapCommand1}"
  
  gesture:Gesture.PanCommand="{Binding PanCommand1}"
  
TapCommand has a Point and PanCommand has a Gesture.PanEventArgs.
  
you can see the demo in [TouchTest](https://github.com/j4587698/FormsGusture/tree/master/TouchTest).

BUG:
In Demo, IOS can not show every parameter on screen.I don't know why.But it's fine in Debug.WriteLine.

PS:
Is reference from [XamarinFormsGesture](https://github.com/softlion/XamarinFormsGesture).
 
 
