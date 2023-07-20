// Copyright (c) Microsoft. All rights reserved. 
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using Microsoft.UI.Xaml;
using System.Windows;

namespace UnoBehavior;

/// <summary>
/// An interface for an object that can be attached to another object.
/// </summary>
public interface IAttachedObject
{
	void AttachTo(DependencyObject bindable);
	void DetachFrom(DependencyObject bindable);
}