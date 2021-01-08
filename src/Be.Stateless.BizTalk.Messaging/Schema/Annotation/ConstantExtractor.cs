#region Copyright & License

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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.Extensions;
using log4net;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Schema.Annotation
{
	public class ConstantExtractor : PropertyExtractor, IEquatable<ConstantExtractor>
	{
		#region Operators

		public static bool operator ==(ConstantExtractor left, ConstantExtractor right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ConstantExtractor left, ConstantExtractor right)
		{
			return !Equals(left, right);
		}

		#endregion

		public ConstantExtractor(XmlQualifiedName propertyName, string value, ExtractionMode extractionMode = ExtractionMode.Write)
			: base(propertyName, extractionMode)
		{
			if (value.IsNullOrEmpty()) throw new ArgumentNullException(nameof(value));
			if (extractionMode == ExtractionMode.Demote)
				throw new ArgumentException(
					$"{nameof(Annotation.ExtractionMode)} '{ExtractionMode.Demote}' is not supported by {nameof(ConstantExtractor)}.",
					nameof(extractionMode));
			Value = value;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
		public ConstantExtractor(IMessageContextProperty property, string value, ExtractionMode extractionMode = ExtractionMode.Write)
			: this(property?.QName ?? throw new ArgumentNullException(nameof(property)), value, extractionMode) { }

		#region IEquatable<ConstantExtractor> Members

		public bool Equals(ConstantExtractor other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
		}

		#endregion

		#region Base Class Member Overrides

		public override bool Equals(object other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.GetType() == GetType() && Equals((ConstantExtractor) other);
		}

		[SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
		public override void Execute(IBaseMessageContext messageContext)
		{
			if (messageContext == null) throw new ArgumentNullException(nameof(messageContext));
			if (ExtractionMode == ExtractionMode.Write)
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Writing property {0} with value {1} to context.", PropertyName, Value);
				messageContext.Write(PropertyName.Name, PropertyName.Namespace, Value);
			}
			else if (ExtractionMode == ExtractionMode.Promote)
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Promoting property {0} with value {1} to context.", PropertyName, Value);
				messageContext.Promote(PropertyName.Name, PropertyName.Namespace, Value);
			}
			else
			{
				base.Execute(messageContext);
			}
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode() * 397) ^ Value.GetHashCode();
			}
		}

		public override string ToString()
		{
			return $"{base.ToString()}[Value:{Value}]";
		}

		protected internal override void WriteXmlCore(XmlWriter writer)
		{
			if (writer == null) throw new ArgumentNullException(nameof(writer));
			base.WriteXmlCore(writer);
			writer.WriteAttributeString("value", Value);
		}

		#endregion

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public string Value { get; }

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ConstantExtractor));
	}
}
