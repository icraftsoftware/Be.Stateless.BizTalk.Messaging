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
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Be.Stateless.Extensions;
using log4net;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Schema.Annotation
{
	public class PropertyExtractor : IEquatable<PropertyExtractor>
	{
		#region Operators

		public static bool operator ==(PropertyExtractor left, PropertyExtractor right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(PropertyExtractor left, PropertyExtractor right)
		{
			return !Equals(left, right);
		}

		#endregion

		public PropertyExtractor(XmlQualifiedName propertyName, ExtractionMode extractionMode)
		{
			if (GetType() == typeof(PropertyExtractor) && !(extractionMode == ExtractionMode.Clear || extractionMode == ExtractionMode.Ignore))
				throw new ArgumentException(
					"Invalid ExtractionMode, only Clear and Ignore are supported for PropertyExtractor without a Value or an XPath.",
					nameof(extractionMode));
			PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
			ExtractionMode = extractionMode;
		}

		#region IEquatable<PropertyExtractor> Members

		public bool Equals(PropertyExtractor other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.GetType() == GetType() && ExtractionMode == other.ExtractionMode && PropertyName.Equals(other.PropertyName);
		}

		#endregion

		#region Base Class Member Overrides

		public override bool Equals(object other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.GetType() == GetType() && Equals((PropertyExtractor) other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((int) ExtractionMode * 397) ^ PropertyName.GetHashCode();
			}
		}

		public override string ToString()
		{
			return $"{GetType()}[ExtractionMode:{ExtractionMode}][PropertyName:{PropertyName}]";
		}

		#endregion

		public ExtractionMode ExtractionMode { get; }

		public XmlQualifiedName PropertyName { get; }

		protected internal virtual void WriteXmlCore(XmlWriter writer)
		{
			if (writer == null) throw new ArgumentNullException(nameof(writer));
			if (ExtractionMode != default) writer.WriteAttributeString("mode", ExtractionMode.ToString().ToCamelCase());
		}

		[SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Public API.")]
		[SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
		public virtual void Execute(IBaseMessageContext messageContext)
		{
			if (messageContext == null) throw new ArgumentNullException(nameof(messageContext));
			if (ExtractionMode == ExtractionMode.Clear)
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Clearing property {0} from context.", PropertyName);
				if (messageContext.IsPromoted(PropertyName.Name, PropertyName.Namespace))
				{
					messageContext.Promote(PropertyName.Name, PropertyName.Namespace, null);
				}
				messageContext.Write(PropertyName.Name, PropertyName.Namespace, null);
			}
			else if (ExtractionMode == ExtractionMode.Ignore)
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Ignoring property {0} from context.", PropertyName);
			}
			else
			{
				throw new InvalidOperationException($"Unexpected ExtractionMode '{ExtractionMode}'.");
			}
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(PropertyExtractor));
	}
}
