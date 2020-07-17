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
	/// <summary>
	/// Denotes a context property whose only the local part of a QName value will be extracted from an <see
	/// cref="IBaseMessagePart"/>'s payload while being processed by the
	/// <c>Be.Stateless.BizTalk.Component.ContextPropertyExtractorComponent</c> pipeline component.
	/// </summary>
	public class QNameValueExtractor : XPathExtractor, IEquatable<QNameValueExtractor>
	{
		#region Operators

		public static bool operator ==(QNameValueExtractor left, QNameValueExtractor right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(QNameValueExtractor left, QNameValueExtractor right)
		{
			return !Equals(left, right);
		}

		#endregion

		public QNameValueExtractor(
			XmlQualifiedName propertyName,
			string xpathExpression,
			ExtractionMode contextExtractionMode = ExtractionMode.Write,
			QNameValueExtractionMode qNameValueExtractionMode = QNameValueExtractionMode.Default)
			: base(propertyName, xpathExpression, contextExtractionMode)
		{
			QNameValueExtractionMode = qNameValueExtractionMode;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
		public QNameValueExtractor(
			IMessageContextProperty property,
			string xpathExpression,
			ExtractionMode contextExtractionMode = ExtractionMode.Write,
			QNameValueExtractionMode qNameValueExtractionMode = QNameValueExtractionMode.Default)
			: this(property?.QName ?? throw new ArgumentNullException(nameof(property)), xpathExpression, contextExtractionMode, qNameValueExtractionMode) { }

		#region IEquatable<QNameValueExtractor> Members

		public bool Equals(QNameValueExtractor other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && QNameValueExtractionMode == other.QNameValueExtractionMode;
		}

		#endregion

		#region Base Class Member Overrides

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((QNameValueExtractor) obj);
		}

		public override void Execute(IBaseMessageContext messageContext, string originalValue, ref string newValue)
		{
			if (ExtractionMode == ExtractionMode.Demote)
			{
				base.Execute(messageContext, originalValue, ref newValue);
				if (originalValue.TryParseQName(out var prefix, out _))
					newValue = prefix.IsNullOrEmpty()
						? newValue
						: prefix + ":" + newValue;
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Demoting property {0} with QName value {1} from context.", PropertyName, newValue);
			}
			else
			{
				if (originalValue.TryParseQName(out _, out var localPart)) originalValue = localPart;
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Extracting localPart from QName value {0} from context.", originalValue);
				base.Execute(messageContext, originalValue, ref newValue);
			}
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode() * 397) ^ (int) QNameValueExtractionMode;
			}
		}

		#endregion

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public QNameValueExtractionMode QNameValueExtractionMode { get; }

		private static readonly ILog _logger = LogManager.GetLogger(typeof(QNameValueExtractor));
	}
}
