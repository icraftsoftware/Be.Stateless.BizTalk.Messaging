#region Copyright & License

// Copyright © 2012 - 2021 François Chabot
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
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using Be.Stateless.BizTalk.ContextProperties;
using Microsoft.BizTalk.Message.Interop;
using WCF;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public static class BaseMessageContextExtensions
	{
		public static string ToXml(this IBaseMessageContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			// cache xmlns while constructing xml info set...
			var nsCache = new XmlDictionary();
			var xmlDocument = new XElement(
				"context",
				Enumerable.Range(0, (int) context.CountProperties).Select(
					i => {
						var value = context.ReadAt(i, out var name, out var ns);
						// give each property element a name of 'p' and store its actual name inside the 'n' attribute, which avoids
						// the cost of the name.IsValidQName() check for each of them as the name could be an xpath expression in the
						// case of a distinguished property
						return name.IsSensitiveProperty()
							? null
							: new XElement(
								(XNamespace) nsCache.Add(ns).Value + "p",
								new XAttribute("n", name),
								context.IsPromoted(name, ns) ? new XAttribute("promoted", true) : null,
								name.IsHttpHeaders(ns) ? value.ToString().Redact() : value);
					}));

			// ... and declare/alias all of them at the root element level to minimize xml string size
			for (var i = 0; nsCache.TryLookup(i, out var xds); i++)
			{
				xmlDocument.Add(new XAttribute(XNamespace.Xmlns + "s" + xds.Key.ToString(CultureInfo.InvariantCulture), xds.Value));
			}

			return xmlDocument.ToString(SaveOptions.DisableFormatting);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsHttpHeaders(this string name, string ns)
		{
			return name.Equals(WcfProperties.HttpHeaders.Name, StringComparison.OrdinalIgnoreCase)
				&& ns.Equals(WcfProperties.HttpHeaders.Namespace, StringComparison.OrdinalIgnoreCase);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsSensitiveProperty(this string name)
		{
			return name.IndexOf("password", StringComparison.OrdinalIgnoreCase) > -1
				|| name.Equals(nameof(SharedAccessKey), StringComparison.OrdinalIgnoreCase)
				|| name.Equals(nameof(IssuerSecret), StringComparison.OrdinalIgnoreCase);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string Redact(this string value)
		{
			return value.IndexOf(nameof(HttpRequestHeaders.Authorization), StringComparison.OrdinalIgnoreCase) < 0
				? value
				: string.Join(
					Environment.NewLine,
					value
						.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
						.Select(h => h.Trim())
						.Where(h => !h.StartsWith(nameof(HttpRequestHeaders.Authorization), StringComparison.OrdinalIgnoreCase)));
		}
	}
}
