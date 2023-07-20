using Microsoft.UI.Xaml;
using System.Collections.Generic;

namespace UnoBehavior;
public static class Interaction
{
	public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached("Behaviors", typeof(ICollection<Behavior>), typeof(Interaction), new PropertyMetadata(null, OnBehaviorsChanged));

	public static AttachedCollection<Behavior> GetBehaviors(DependencyObject obj)
	{
		var behaviors = (AttachedCollection<Behavior>)obj.GetValue(BehaviorsProperty);
		if (behaviors is null)
		{
			behaviors = new AttachedCollection<Behavior>();
			obj.SetValue(BehaviorsProperty, behaviors);
		}
		return behaviors;
	}

	public static void SetBehaviors(DependencyObject obj, AttachedCollection<Behavior> value)
	{
		obj.SetValue(BehaviorsProperty, value);
	}

	static void OnBehaviorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var oldBehaviors = (AttachedCollection<Behavior>)e.OldValue;
		var newBehaviors = (AttachedCollection<Behavior>)e.NewValue;

		oldBehaviors?.DetachFrom(d);
		newBehaviors?.AttachTo(d);
	}
}
