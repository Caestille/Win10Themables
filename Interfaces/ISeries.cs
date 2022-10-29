﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ModernThemables.Interfaces
{
	/// <summary>
	/// Interface for a generic chart series.
	/// </summary>
	public interface ISeries
	{
		/// <summary>
		/// Event fired when a property of the series changes.
		/// </summary>
		event EventHandler<PropertyChangedEventArgs> PropertyChanged;

		/// <summary>
		/// Event fired when the series points change.
		/// </summary>
		event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanged;

		/// <summary>
		/// A <see cref="Func{T1, T2, TResult}"/> Which given a list of series points and the current highlighted
		/// point, returns a string which is the formatted tooltip.
		/// </summary>
		Func<IEnumerable<IChartPoint>, IChartPoint, string> TooltipLabelFormatter { get; set; }

		/// <summary>
		/// The series line stroke.
		/// </summary>
		IChartBrush Stroke { get; set; }

		/// <summary>
		/// The series line fill.
		/// </summary>
		IChartBrush Fill { get; set; }

		/// <summary>
		/// The series values.
		/// </summary>
		ObservableCollection<IChartPoint> Values { get; set; }

		/// <summary>
		/// A unique identifier for the series.
		/// </summary>
		Guid Identifier { get; }

		/// <summary>
		/// The series name.
		/// </summary>
		string Name { get; set; }
	}
}
