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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Be.Stateless.Linq.Extensions;
using log4net;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Microsoft.BizTalk.XPath;

namespace Be.Stateless.BizTalk.Schema.Annotation
{
	/// <summary>
	/// Essentially an <see cref="XPathCollection"/>, which moreover supports and handles <see cref="XPathMutatorStream"/>'s <see
	/// cref="XPathMutatorStream.Mutator"/> callbacks.
	/// </summary>
	internal class ReactiveXPathExtractorCollection : XPathCollection
	{
		#region Nested Type: XPathMatch

		private class XPathMatch
		{
			public int Count { get; set; }

			public IEnumerable<XPathExtractor> Extractors { get; set; }
		}

		#endregion

		internal ReactiveXPathExtractorCollection(IEnumerable<XPathExtractor> xpathExtractors, Func<IBaseMessageContext> messageContextAccessor)
		{
			if (xpathExtractors == null) throw new ArgumentNullException(nameof(xpathExtractors));
			if (messageContextAccessor == null) throw new ArgumentNullException(nameof(messageContextAccessor));

			xpathExtractors.GroupBy(e => e.XPathExpression.XPath)
				.ForEach(
					extractorGroup => {
						if (_logger.IsDebugEnabled)
							_logger.DebugFormat(
								"Setting up extractor for properties <{0}> with XPath expression '{1}'.",
								string.Join(">, <", extractorGroup.Select(eg => eg.PropertyName.ToString()).ToArray()),
								extractorGroup.Key);
						var index = Add(extractorGroup.Key);
						_xpathMatches.Insert(index, new XPathMatch { Extractors = extractorGroup });
					});
			_messageContextAccessor = messageContextAccessor;
		}

		internal void OnMatch(int xpathCollectionIndex, XPathExpression xpathExpression, string originalValue, ref string newValue)
		{
			var xpathMatch = _xpathMatches[xpathCollectionIndex];
			Debug.Assert(
				xpathMatch.Extractors.All(e => e.XPathExpression.XPath == xpathExpression.XPath),
				"Cannot rely on XPathCollection's index to determine which XPathExtractors have been matched.");

			// only the first match is taken into account so far
			if (++xpathMatch.Count != 1) return;

			var messageContext = _messageContextAccessor();
			if (_logger.IsDebugEnabled) _logger.DebugFormat("Matching occurrence {0} of XPath {1}.", xpathMatch.Count, xpathExpression.XPath);
			foreach (var xpe in xpathMatch.Extractors)
			{
				xpe.Execute(messageContext, originalValue, ref newValue);
			}
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ReactiveXPathExtractorCollection));
		private readonly Func<IBaseMessageContext> _messageContextAccessor;
		private readonly IList<XPathMatch> _xpathMatches = new List<XPathMatch>();
	}
}
