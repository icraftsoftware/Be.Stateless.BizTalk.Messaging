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
using System.Linq;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public static class BaseMessageContextExtensions
	{
		public static string ToXml(this IBaseMessageContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			return MessageContextSerializer.Serialize(
				serializeProperty => Enumerable.Range(0, (int) context.CountProperties).Select(
					i => {
						var value = context.ReadAt(i, out var name, out var ns);
						return serializeProperty(name, ns, value, context.IsPromoted(name, ns));
					}));
		}
	}
}
