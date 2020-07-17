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
using System.Collections.Generic;
using Be.Stateless.BizTalk.Schema.Annotation;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;

namespace Be.Stateless.BizTalk.Stream
{
#pragma warning disable 1574
	/// <summary>
	/// <see cref="XPathMutatorStream"/> that goes along with <see cref="Be.Stateless.BizTalk.MicroComponent.ContextPropertyExtractor"/>
	/// and <see cref="Component.ContextPropertyExtractorComponent"/>.
	/// </summary>
#pragma warning restore 1574
	public static class XPathMutatorStreamFactory
	{
		public static XPathMutatorStream Create(System.IO.Stream stream, IEnumerable<XPathExtractor> extractors, Func<IBaseMessageContext> messageContextAccessor)
		{
			if (stream == null) throw new ArgumentNullException(nameof(stream));
			if (extractors == null) throw new ArgumentNullException(nameof(extractors));
			if (messageContextAccessor == null) throw new ArgumentNullException(nameof(messageContextAccessor));

			var reactiveExtractorCollection = new ReactiveXPathExtractorCollection(extractors, messageContextAccessor);
			return new(stream, reactiveExtractorCollection, reactiveExtractorCollection.OnMatch);
		}
	}
}
