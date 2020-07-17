﻿#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Schema.Annotation
{
	/// <summary>
	/// Converts a <see cref="PropertyExtractorCollection"/> back and forth to a <see cref="string"/>.
	/// </summary>
	/// <remarks>
	/// Notice that <see cref="PropertyExtractorCollectionConverter"/> delegates the XML serialization and deserialization to <see
	/// cref="PropertyExtractorCollection"/>.
	/// </remarks>
	/// <seealso cref="PropertyExtractorCollection"/>
	public class PropertyExtractorCollectionConverter : ExpandableObjectConverter
	{
		/// <summary>
		/// Deserializes a <see cref="PropertyExtractorCollection"/> from its XML serialization <see cref="string"/>.
		/// </summary>
		/// <param name="xml">
		/// A <see cref="string"/> denoting the XML serialization of a <see cref="PropertyExtractorCollection"/>.
		/// </param>
		/// <returns>
		/// The deserialized <see cref="PropertyExtractorCollection"/>.
		/// </returns>
		/// <seealso cref="Serialize"/>
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public static PropertyExtractorCollection Deserialize(string xml)
		{
			if (xml.IsNullOrEmpty()) return PropertyExtractorCollection.Empty;
			using (var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { IgnoreWhitespace = true, IgnoreComments = true }))
			{
				var collection = new PropertyExtractorCollection();
				collection.ReadXml(reader);
				return collection;
			}
		}

		/// <summary>
		/// Serializes a <see cref="PropertyExtractorCollection"/> to its XML <see cref="string"/> representation.
		/// </summary>
		/// <param name="extractors">
		/// The <see cref="PropertyExtractorCollection"/> to serialize.
		/// </param>
		/// <returns>
		/// A <see cref="string"/> that represents the <see cref="PropertyExtractorCollection"/>.
		/// </returns>
		/// <seealso cref="Deserialize"/>
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public static string Serialize(PropertyExtractorCollection extractors)
		{
			if (extractors == null || !extractors.Any()) return null;
			using (var stringWriter = new StringWriter())
			using (var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				extractors.WriteXml(writer);
				writer.Flush();
				return stringWriter.ToString();
			}
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
		/// </summary>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		/// <param name="context">
		/// An <see cref="ITypeDescriptorContext"/> that provides a format context.
		/// </param>
		/// <param name="sourceType">
		/// A <see cref="Type"/> that represents the type you want to convert from.
		/// </param>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Returns whether this converter can convert the object to the specified type, using the specified context.
		/// </summary>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		/// <param name="context">
		/// An <see cref="ITypeDescriptorContext"/> that provides a format context.
		/// </param>
		/// <param name="destinationType">
		/// A <see cref="Type"/> that represents the type you want to convert to.
		/// </param>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// Converts the given object to the type of this converter, using the specified context and culture information.
		/// </summary>
		/// <returns>
		/// An <see cref="object"/> that represents the converted value.
		/// </returns>
		/// <param name="context">
		/// An <see cref="ITypeDescriptorContext"/> that provides a format context.
		/// </param>
		/// <param name="culture">
		/// The <see cref="CultureInfo"/> to use as the current culture.
		/// </param>
		/// <param name="value">
		/// The <see cref="object"/> to convert.
		/// </param>
		/// <exception cref="NotSupportedException">
		/// The conversion cannot be performed.
		/// </exception>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var xml = value as string;
			return value == null || xml != null ? Deserialize(xml) : base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <returns>
		/// An <see cref="object"/> that represents the converted value.
		/// </returns>
		/// <param name="context">
		/// An <see cref="ITypeDescriptorContext"/> that provides a format context.
		/// </param>
		/// <param name="culture">
		/// A <see cref="CultureInfo"/>. If null is passed, the current culture is assumed.
		/// </param>
		/// <param name="value">
		/// The <see cref="object"/> to convert.
		/// </param>
		/// <param name="destinationType">
		/// The <see cref="Type"/> to convert the <paramref name="value"/> parameter to.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// The <paramref name="destinationType"/> parameter is null.
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// The conversion cannot be performed.
		/// </exception>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return value is PropertyExtractorCollection collection && destinationType == typeof(string)
				? Serialize(collection)
				: base.ConvertTo(context, culture, value, destinationType);
		}

		#endregion
	}
}
