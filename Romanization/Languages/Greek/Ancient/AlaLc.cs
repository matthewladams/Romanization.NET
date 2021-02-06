﻿using Romanization.LanguageAgnostic;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable InconsistentNaming

namespace Romanization
{
	public static partial class Greek
	{
		public static partial class Ancient
		{
			/// <summary>
			/// The ALA-LC (American Library Association and Library of Congress) Greek romanization system.<br />
			/// For more information, visit:<br />
			/// <a href='https://en.wikipedia.org/wiki/Romanization_of_Greek'>https://en.wikipedia.org/wiki/Romanization_of_Greek</a><br />
			/// and<br />
			/// <a href='https://www.loc.gov/catdir/cpso/romanization/greek.pdf'>https://www.loc.gov/catdir/cpso/romanization/greek.pdf</a>
			/// </summary>
			public sealed class AlaLc : IExtendedMultiCulturalRomanizationSystem
			{
				/// <inheritdoc />
				public SystemType Type => SystemType.Transliteration;

				/// <inheritdoc />
				public CultureInfo DefaultCulture => CultureInfo.GetCultureInfo("el-GR");

				/// <inheritdoc cref="OutputNumeralType"/>
				public readonly OutputNumeralType OutputNumeralType;

				/// <summary>
				/// Whether or not this system should parse as if the text is very old. In very old Greek, a different
				/// punctuation system was used and Attic numerals were used instead of newer Greek numerals.<br />
				/// While the use of this should largely depend on the text you intend to use it with, a decent
				/// rule-of-thumb is that if Attic numerals are a possibility, this should be true.
				/// </summary>
				public readonly bool VeryOld;

				// Sub-systems
				private readonly INumeralParsingSystem NumeralsSystem;

				// System-Specific Constants
				private readonly Dictionary<string, string> RomanizationTable = new Dictionary<string, string>();
				private readonly Dictionary<string, string> DiphthongTable = new Dictionary<string, string>();
				private readonly Dictionary<string, string> SpecificCombinationTable = new Dictionary<string, string>();
				private readonly Dictionary<string, string> PunctuationTable = new Dictionary<string, string>();

				private readonly CaseAwareSub RhoAspiratedSub = new CaseAwareSub("(?:\\bρ|(?<=ρ)ρ(?!\\b|ρ))", "rh");

				private readonly CaseAwareSub RoughBreathingVowelSub =
					new CaseAwareSub($"(?<=[{GreekAllVowels}]|{GreekVowelDiphthongs})\u0314", "h", true);

				/// <summary>
				/// Instantiates a copy of the system to process romanizations.
				/// </summary>
				/// <param name="outputNumeralType">What kind of numeral to romanize to.<br />
				/// Greek numerals are traditionally romanized to Roman numerals, except for when in
				/// official/government documents.</param>
				/// <param name="veryOld">Whether or not this system should parse as if the text is very old. In very old
				/// Greek, a different punctuation system was used and Attic numerals were used instead.</param>
				public AlaLc(OutputNumeralType outputNumeralType = OutputNumeralType.Roman, bool veryOld = false)
				{
					OutputNumeralType = outputNumeralType;
					VeryOld = veryOld;
					NumeralsSystem = !veryOld ? (INumeralParsingSystem) new GreekNumerals() : new AtticNumerals();

					#region Romanization Chart

					// Sourced from https://en.wikipedia.org/wiki/Romanization_of_Greek

					// Main characters (2021)
					RomanizationTable["α"]       = "a";
					RomanizationTable["β"]       = "b";
					RomanizationTable["γ"]       = "g";  // has special provisions
					RomanizationTable["δ"]       = "d";
					RomanizationTable["ε"]       = "e";
					RomanizationTable["ζ"]       = "z";
					RomanizationTable["η"]       = "ē";
					RomanizationTable["θ"]       = "th";
					RomanizationTable["ι"]       = "i";
					RomanizationTable["κ"]       = "k";
					RomanizationTable["λ"]       = "l";
					RomanizationTable["μ"]       = "m";
					RomanizationTable["ν"]       = "n";
					RomanizationTable["ξ"]       = "x";
					RomanizationTable["ο"]       = "o";
					RomanizationTable["π"]       = "p";
					RomanizationTable["ρ\u0314"] = "rh";
					RomanizationTable["ρ"]       = "r";  // has special provisions
					RomanizationTable["σ"]       = "s";
					RomanizationTable["ς"]       = "s";
					RomanizationTable["τ"]       = "t";
					RomanizationTable["υ"]       = "y";  // has special provisions
					RomanizationTable["φ"]       = "ph";
					RomanizationTable["χ"]       = "ch";
					RomanizationTable["ψ"]       = "ps";
					RomanizationTable["ω"]       = "ō";

					DiphthongTable["αι"] = "ae";
					DiphthongTable["ει"] = "ei";
					DiphthongTable["οι"] = "oi";
					DiphthongTable["ου"] = "ou";
					DiphthongTable["υι"] = "ui";
					DiphthongTable["αυ"] = "au";
					DiphthongTable["ευ"] = "eu";
					DiphthongTable["ηυ"] = "eu";
					DiphthongTable["υι"] = "ōu";

					// Gamma velar stop combinations
					SpecificCombinationTable["γγ"] = "ng";
					SpecificCombinationTable["γκ"] = "nk";
					SpecificCombinationTable["γξ"] = "nx";
					SpecificCombinationTable["γχ"] = "nch";

					// Uncommon letters
					RomanizationTable["ϝ"] = "w"; // Digamma
					RomanizationTable["ͷ"] = "w"; // Digamma
					RomanizationTable["ϙ"] = "ḳ"; // Koppa
					RomanizationTable["ϟ"] = "ḳ"; // Koppa
					RomanizationTable["ϡ"] = "";  // Sampi
					RomanizationTable["ͳ"] = "";  // Sampi
					RomanizationTable["ϻ"] = "";  // San
					RomanizationTable["ϲ"] = "s"; // Lunate sigma
					RomanizationTable["ϳ"] = "";  // Yot

					// Punctuation
					if (veryOld)
					{
						PunctuationTable["."]      = ","; // Low dot (in ancient Greek this acted as a short breath, or comma)
						PunctuationTable["·"]      = ";"; // Mid dot (in ancient Greek this acted as a long breath, or semicolon)
						PunctuationTable["\u0387"] = ";"; // Distinct from above but visually the same
						PunctuationTable["\u02D9"] = "."; // High dot (in ancient Greek this acted as a full stop)
						PunctuationTable["\u205A"] = "."; // In ancient texts the Greek two-dot punctuation mark (looks like a colon) served as the full stop
						PunctuationTable["\u203F"] = "-"; // Papyrological hyphen
						PunctuationTable["\u035C"] = "-"; // Papyrological hyphen
					}
					PunctuationTable[";"]      = "?";
					PunctuationTable["\u037E"] = "?"; // Distinct from above but visually the same
					PunctuationTable["’"]      = "h"; // Sometimes used as an aspiration mark

					#endregion
				}

				/// <summary>
				/// Performs ALA-LC Greek romanization on the given text.<br />
				/// Supports providing a specific <paramref name="nativeCulture"/> (as long as the
				/// country code is <c>el</c>) to romanize from, and a
				/// <paramref name="romanizedCulture"/> to romanize to.
				/// </summary>
				/// <param name="text">The text to romanize.</param>
				/// <param name="nativeCulture">The culture to romanize from.</param>
				/// <param name="romanizedCulture">The culture to romanize to.</param>
				/// <returns>A romanized version of the text, leaving unrecognized characters untouched.</returns>
				/// <exception cref="IrrelevantCultureException"><paramref name="nativeCulture"/> is irrelevant to the
				/// language/region.</exception>
				[Pure]
				public string Process(string text, CultureInfo nativeCulture, CultureInfo romanizedCulture)
				{
					if (nativeCulture.TwoLetterISOLanguageName.ToLowerInvariant() != "el")
						throw new IrrelevantCultureException(nativeCulture.DisplayName, nameof(nativeCulture));
					return Utilities.RunWithCulture(nativeCulture, () =>
					{
						text = text
							// General preparation, normalization
							.LanguageWidePreparation()
							// Remove diacritics that this system ignores
							.WithoutChars("\u0300\u0340" + // Grave accent
							              "\u0301\u0341" + // Acute accent
							              "\u0313\u0343" + // Smooth breathing/koronis
							              "\u0303\u0342" + // Tilde
							              "\u0311" +       // Inverted breve
							              "\u0345")        // Iota subscript
							.ReplaceFromChart(PunctuationTable);

						// Convert numerals
						text = OutputNumeralType == OutputNumeralType.Roman
							? NumeralsSystem.ProcessNumeralsInText(text, value => value.Value.ToRomanNumerals())
							: NumeralsSystem.ProcessNumeralsInText(text, value => value.Value.ToArabicNumerals(romanizedCulture));

						// Actual romanization of text
						return text
							// Do special provisions
							.ReplaceMany(RhoAspiratedSub, RoughBreathingVowelSub)
							.ReplaceFromChart(DiphthongTable)
							.ReplaceFromChart(SpecificCombinationTable)
							// Do regular character replacement
							.ReplaceFromChart(RomanizationTable);
					});
				}

				/// <summary>
				/// Performs ALA-LC Greek romanization on the given text.<br />
				/// Supports providing a specific <paramref name="nativeCulture"/> to process with, as long as the
				/// country code is <c>el</c>.
				/// </summary>
				/// <param name="text">The text to romanize.</param>
				/// <param name="nativeCulture">The culture to romanize from.</param>
				/// <returns>A romanized version of the text, leaving unrecognized characters untouched.</returns>
				/// <exception cref="IrrelevantCultureException"><paramref name="nativeCulture"/> is irrelevant to the
				/// language/region.</exception>
				[Pure]
				public string Process(string text, CultureInfo nativeCulture)
					=> Process(text, nativeCulture, CultureInfo.CurrentCulture);

				/// <summary>
				/// Performs ALA-LC Greek romanization on the given text.
				/// </summary>
				/// <param name="text">The text to romanize.</param>
				/// <returns>A romanized version of the text, leaving unrecognized characters untouched.</returns>
				[Pure]
				public string Process(string text)
					=> Process(text, DefaultCulture);
			}
		}
	}
}