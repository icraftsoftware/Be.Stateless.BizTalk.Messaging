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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Schema
{
	public class SchemaAnnotationReader : ISchemaAnnotationReader
	{
		#region Nested Type: EmptySchemaAnnotationReader

		internal class EmptySchemaAnnotationReader : ISchemaAnnotationReader
		{
			#region ISchemaAnnotationReader Members

			public XElement GetAnnotationElement(string annotationElementLocalName)
			{
				return null;
			}

			#endregion
		}

		#endregion

		internal static ISchemaAnnotationReader Create(ISchemaMetadata schemaMetadata)
		{
			if (schemaMetadata == null) throw new ArgumentNullException(nameof(schemaMetadata));
			return schemaMetadata is SchemaMetadata.RootlessSchemaMetadata
				|| schemaMetadata is SchemaMetadata.UnknownSchemaMetadata
				|| schemaMetadata.Type.Assembly.FullName.StartsWith("Microsoft.", StringComparison.Ordinal)
					? Empty
					: new SchemaAnnotationReader(schemaMetadata);
		}

		private SchemaAnnotationReader(ISchemaMetadata schemaMetadata)
		{
			_schemaMetadata = schemaMetadata;
		}

		#region ISchemaAnnotationReader Members

		public XElement GetAnnotationElement(string annotationElementLocalName)
		{
			var schema = (SchemaBase) Activator.CreateInstance(_schemaMetadata.Type);
			using (var stringReader = new StringReader(schema.XmlContent))
			{
				var document = XDocument.Load(stringReader);
				var namespaceManager = new XmlNamespaceManager(new NameTable());
				namespaceManager.AddNamespace("xs", XmlSchema.Namespace);
				namespaceManager.AddNamespace("san", SchemaAnnotationCollection.NAMESPACE);
				var annotationXmlElements = document.XPathSelectElements(
					$"/*/xs:element[@name='{_schemaMetadata.RootElementName}']/xs:annotation/xs:appinfo/san:*",
					namespaceManager);
				return annotationXmlElements.SingleOrDefault(e => e.Name.LocalName == annotationElementLocalName);
			}
		}

		#endregion

		public static readonly ISchemaAnnotationReader Empty = new EmptySchemaAnnotationReader();
		private readonly ISchemaMetadata _schemaMetadata;
	}
}
