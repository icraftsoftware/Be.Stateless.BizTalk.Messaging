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

using System.Xml.Linq;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Schema
{
	/// <summary>
	/// XML-serialized schema annotation reader.
	/// </summary>
	public interface ISchemaAnnotationReader
	{
		/// <summary>
		/// The <see cref="ISchemaMetadata"/> of the <see cref="SchemaBase"/>-derived schema for which annotations are being
		/// retrieved.
		/// </summary>
		ISchemaMetadata SchemaMetadata { get; }

		/// <summary>
		/// Get annotation's XML-serialized element by annotation's name.
		/// </summary>
		/// <param name="annotationElementLocalName">
		/// Local name of the annotation whose XML-serialized element need to be returned.
		/// </param>
		/// <returns>
		/// XML-serialized element for annotation whose name is <paramref name="annotationElementLocalName"/>.
		/// </returns>
		XElement GetAnnotationElement(string annotationElementLocalName);
	}
}
