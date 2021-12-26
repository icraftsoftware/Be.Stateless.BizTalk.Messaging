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

using System.Collections.Generic;
using System.Linq;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Adapter.Wcf.Metadata;

namespace Be.Stateless.BizTalk.Adapter.Metadata
{
	/// <summary>
	/// Describes the BizTalk Server HTTP Method and URL mapping operations.
	/// </summary>
	public class HttpUrlMapping : List<HttpUrlMappingOperation>
	{
		#region Operators

		public static implicit operator string(HttpUrlMapping mapping)
		{
			var btsHttpUrlMapping = new BtsHttpUrlMapping {
				Operation = mapping.Select(hum => new BtsHttpUrlMappingOperation { Method = hum.Method, Name = hum.Name, Url = hum.Url }).ToArray()
			};
			return btsHttpUrlMapping.ToXml();
		}

		#endregion
	}
}
