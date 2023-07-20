// Copyright (c) Microsoft. All rights reserved. 
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using Microsoft.UI.Xaml;
using System;

namespace UnoBehavior;

/// <summary>
/// Encapsulates state information and zero or more ICommands into an attachable object.
/// </summary>
/// <typeparam name="T">The type the <see cref="Behavior&lt;T&gt;"/> can be attached to.</typeparam>
/// <remarks>
///		Behavior is the base class for providing attachable state and commands to an object.
///		The types the Behavior can be attached to can be controlled by the generic parameter.
///		Override OnAttached() and OnDetaching() methods to hook and unhook any necessary handlers
///		from the AssociatedObject.
///	</remarks>
public abstract partial class Behavior<T> : Behavior where T : DependencyObject
{
	/// <inheritdoc/>
	protected Behavior() : base(typeof(T))
	{
	}

	/// <inheritdoc/>
	protected override void OnAttachedTo(DependencyObject bindable)
	{
		base.OnAttachedTo(bindable);
		OnAttachedTo((T)bindable);
	}

	/// <summary>
	/// Application developers override this method to implement the behaviors that will be associated with <paramref name="bindable" />.
	/// </summary>
	/// <param name="bindable">The bindable object to which the behavior was attached.</param>
	protected virtual void OnAttachedTo(T bindable)
	{
	}

	/// <inheritdoc/>
	protected override void OnDetachingFrom(DependencyObject bindable)
	{
		OnDetachingFrom((T)bindable);
		base.OnDetachingFrom(bindable);
	}

	/// <summary>
	/// Application developers override this method to remove the behaviors from <paramref name="bindable" />
	/// that were implemented in a previous call to the <see cref="OnAttachedTo(T)"/> method.
	/// </summary>
	/// <param name="bindable">The bindable object from which the behavior was detached.</param>
	protected virtual void OnDetachingFrom(T bindable)
	{
	}
}

/// <summary>
/// Encapsulates state information and zero or more ICommands into an attachable object.
/// </summary>
/// <remarks>This is an infrastructure class. Behavior authors should derive from Behavior&lt;T&gt; instead of from this class.</remarks>
public abstract partial class Behavior : DependencyObject, IAttachedObject
{
	/// <summary>
	/// Creates a new <see cref="Behavior" /> with default values.
	/// </summary>
	protected Behavior() : this(typeof(DependencyObject))
	{
	}

	internal Behavior(Type associatedType) => AssociatedType = associatedType ?? throw new ArgumentNullException(nameof(associatedType));

	/// <summary>
	/// Gets the type of the objects with which this <see cref="Behavior" /> can be associated.
	/// </summary>
	protected Type AssociatedType { get; }

	void IAttachedObject.AttachTo(DependencyObject bindable)
	{
		if (bindable is null)
			throw new ArgumentNullException(nameof(bindable));
		if (!AssociatedType.IsInstanceOfType(bindable))
			throw new InvalidOperationException("bindable not an instance of AssociatedType");
		OnAttachedTo(bindable);
	}

	void IAttachedObject.DetachFrom(DependencyObject bindable) => OnDetachingFrom(bindable);

	/// <summary>
	/// Application developers override this method to implement the behaviors that will be associated with <paramref name="bindable" />.
	/// </summary>
	/// <param name="bindable">The bindable object to which the behavior was attached.</param>
	protected virtual void OnAttachedTo(DependencyObject bindable)
	{
	}

	/// <summary>
	/// Application developers override this method to remove the behaviors from <paramref name="bindable" />
	/// that were implemented in a previous call to the <see cref="OnAttachedTo(BindableObject)"/> method.
	/// </summary>
	/// <param name="bindable">The bindable object from which the behavior was detached.</param>
	protected virtual void OnDetachingFrom(DependencyObject bindable)
	{
	}
}