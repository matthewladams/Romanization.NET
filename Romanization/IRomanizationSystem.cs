﻿using System;
using System.Diagnostics.Contracts;

namespace Romanization
{
	/// <summary>
	/// A system used to romanize a language.
	/// </summary>
	public interface IRomanizationSystem
	{
		/// <summary>
		/// The system-specific function that romanizes text according to the system's rules.
		/// </summary>
		/// <param name="text">The text to romanize.</param>
		/// <returns>A romanized version of the text, leaving unrecognized characters untouched.</returns>
		[Pure]
		public string Process(string text);
	}

	/// <summary>
	/// A system used to romanize a language with multiple readings (pronunciations) per character.
	/// </summary>
	/// <typeparam name="TType">The types of reading the system uses.</typeparam>
	public interface IReadingsRomanizationSystem<TType> : IRomanizationSystem
		where TType : Enum
	{
		/// <summary>
		/// The system-specific function that romanizes text in a language with multiple readings (pronunciations) per character.
		/// </summary>
		/// <param name="text">The text to romanize.</param>
		/// <returns>A <see cref="LanguageAgnostic.ReadingsString{ReadingTypes}"/> with all readings for each character in <paramref name="text"/>.</returns>
		[Pure]
		public LanguageAgnostic.ReadingsString<TType> ProcessWithReadings(string text);
	}
}