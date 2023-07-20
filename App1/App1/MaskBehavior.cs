using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnoBehavior;
#nullable enable
namespace App1;
sealed class MaskBehavior : Behavior<TextBox>
{
	TextBox view = default!;

	// Using a DependencyProperty as the backing store for Mask.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty MaskProperty =
		DependencyProperty.Register("Mask", typeof(string), typeof(MaskBehavior), new PropertyMetadata(string.Empty, OnMaskPropertyChanged));

	static void OnMaskPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
	{
		var behavior = (MaskBehavior)dependencyObject;
		var mask = (string)args.NewValue;
		behavior.SetMaskPositions(mask);
	}

	public string Mask
	{
		get => (string)GetValue(MaskProperty);
		set => SetValue(MaskProperty, value);
	}



	// Using a DependencyProperty as the backing store for UnmaskedCharacter.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty UnmaskedCharacterProperty =
		DependencyProperty.Register("UnmaskedCharacter", typeof(char), typeof(MaskBehavior), new PropertyMetadata('X', OnUnmaskedCharacterChanged));

	static async void OnUnmaskedCharacterChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
	{
		var behavior = (MaskBehavior)dependencyObject;
		await behavior.OnMaskChanged(behavior.Mask).ConfigureAwait(false);
	}

	public char UnmaskedCharacter
	{
		get { return (char)GetValue(UnmaskedCharacterProperty); }
		set { SetValue(UnmaskedCharacterProperty, value); }
	}

	readonly SemaphoreSlim applyMaskSemaphoreSlim = new(1, 1);

	IReadOnlyDictionary<int, char>? maskPositions;

	async ValueTask OnMaskChanged(string? mask, CancellationToken token = default)
	{
		if (string.IsNullOrEmpty(mask))
		{
			maskPositions = null;
			return;
		}

		var originalText = RemoveMask(view.Text);

		SetMaskPositions(mask);

		await ApplyMask(originalText, token);
	}

	void SetMaskPositions(in string? mask)
	{
		if (string.IsNullOrEmpty(mask))
		{
			maskPositions = null;
			return;
		}

		var dic = new Dictionary<int, char>();

		for (int i = 0; i < mask.Length; i++)
		{
			if (mask[i] != UnmaskedCharacter)
				dic.Add(i, mask[i]);
		}

		maskPositions = dic;
	}


	Task OnTextPropertyChanged(CancellationToken cancellationToken = default) => ApplyMask(view.Text, cancellationToken);


	[return: NotNullIfNotNull(nameof(text))]
	string? RemoveMask(string? text)
	{
		if (string.IsNullOrEmpty(text))
			return text;

		var maskChars = maskPositions?
			.Select(c => c.Value)
			.Distinct()
			.ToArray();

		return string.Join(string.Empty, text.Split(maskChars));
	}

	async Task ApplyMask(string? text, CancellationToken cancellationToken = default)
	{
		await applyMaskSemaphoreSlim.WaitAsync(cancellationToken);

		try
		{

			if (!string.IsNullOrWhiteSpace(text) && maskPositions is not null)
			{
				if (Mask is not null && text.Length > Mask.Length)
					text = text.Remove(text.Length - 1);

				text = RemoveMask(text);

				foreach (var position in maskPositions)
				{
					if (text.Length < position.Key + 1)
						continue;

					var value = position.Value.ToString();

					if (text.Substring(position.Key, 1) != value)
						text = text.Insert(position.Key, value);
				}
			}

			if (view is not null)
				view.Text = text;
		}
		finally
		{
			applyMaskSemaphoreSlim.Release();
		}
	}

	protected override void OnAttachedTo(TextBox bindable)
	{
		base.OnAttachedTo(bindable);
		view = bindable;
		bindable.TextChanged += OnTextChanged;
	}

	protected override void OnDetachingFrom(TextBox bindable)
	{
		base.OnDetachingFrom(bindable);
		bindable.TextChanged -= OnTextChanged;
	}

	async void OnTextChanged(object sender, TextChangedEventArgs e)
	{
		await OnTextPropertyChanged().ConfigureAwait(false);
	}
}
