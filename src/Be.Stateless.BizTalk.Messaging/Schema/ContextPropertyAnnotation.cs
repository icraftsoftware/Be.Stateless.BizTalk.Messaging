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
using Be.Stateless.BizTalk.Schema.Annotation;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Schema
{
	public class ContextPropertyAnnotation : ISchemaAnnotation<ContextPropertyAnnotation>
	{
		#region ISchemaAnnotation<ContextPropertyAnnotation> Members

		public ContextPropertyAnnotation Build(ISchemaAnnotationReader schemaAnnotationReader)
		{
			if (schemaAnnotationReader == null) throw new ArgumentNullException(nameof(schemaAnnotationReader));
			Extractors = schemaAnnotationReader.GetAnnotationElement("Properties").IfNotNull(
					p => {
						var extractorCollection = new PropertyExtractorCollection();
						extractorCollection.ReadXml(p.CreateReader());
						return extractorCollection;
					})
				?? PropertyExtractorCollection.Empty;
			return this;
		}

		#endregion

		/// <summary>
		/// Collection of <see cref="PropertyExtractor"/>-derived extractors used to read, write or promote values to and from
		/// the context properties of an <see cref="IBaseMessagePart"/>'s payload while being processed through the pipelines.
		/// </summary>
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Internals visible to BizTalk.Unit")]
		public PropertyExtractorCollection Extractors { get; internal set; }
	}
}
