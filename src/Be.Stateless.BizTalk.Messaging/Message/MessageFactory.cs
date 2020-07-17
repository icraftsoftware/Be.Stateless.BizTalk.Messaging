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
using System.IO;
using System.Xml;
using Be.Stateless.BizTalk.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Xml;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Message
{
	public static class MessageFactory
	{
		/// <summary>
		/// Creates a envelope document with content for a given <see cref="SchemaBase"/>-derived envelope schema type
		/// <typeparamref name="TE"/> and <see cref="SchemaBase"/>-derived content schema type <typeparamref name="TC"/> .
		/// </summary>
		/// <typeparam name="TE">
		/// The <see cref="SchemaBase"/>-derived envelope schema type.
		/// </typeparam>
		/// <typeparam name="TC">
		/// The <see cref="SchemaBase"/>-derived content schema type.
		/// </typeparam>
		/// <returns>
		/// The envelope document with its content as an <see cref="XmlDocument"/>.
		/// </returns>
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
		public static XmlDocument CreateEnvelope<TE, TC>(string content)
			where TE : SchemaBase, new()
			where TC : SchemaBase, new()
		{
			using (var reader = XmlReader.Create(new StringReader(content), new XmlReaderSettings { XmlResolver = null }))
			using (var xmlReader = ValidatingXmlReader.Create<TE, TC>(reader))
			{
				var message = new XmlDocument { XmlResolver = null };
				message.Load(xmlReader);
				return message;
			}
		}

		/// <summary>
		/// Creates a dummy instance document of a given <see cref="SchemaBase"/>-derived schema type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="SchemaBase"/>-derived schema type.
		/// </typeparam>
		/// <returns>
		/// The dummy instance document as an <see cref="XmlDocument"/>.
		/// </returns>
		public static XmlDocument CreateMessage<T>() where T : SchemaBase, new()
		{
			return CreateMessage(SchemaMetadata.For<T>().DocumentSpec);
		}

		/// <summary>
		/// Creates an <see cref="XmlDocument"/> from a given <paramref name="content"/> and ensures its validity against a given
		/// <see cref="SchemaBase"/>-derived schema type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="SchemaBase"/>-derived schema type.
		/// </typeparam>
		/// <param name="content">
		/// The instance document content.
		/// </param>
		/// <returns>
		/// The valid instance document as an <see cref="XmlDocument"/>.
		/// </returns>
		[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global", Justification = "Public API.")]
		public static XmlDocument CreateMessage<T>(string content) where T : SchemaBase, new()
		{
			using (var reader = XmlReader.Create(new StringReader(content), new XmlReaderSettings { XmlResolver = null }))
			using (var xmlReader = ValidatingXmlReader.Create<T>(reader))
			{
				var message = new XmlDocument { XmlResolver = null };
				message.Load(xmlReader);
				return message;
			}
		}

		/// <summary>
		/// Creates a dummy instance document of a given <see cref="SchemaBase"/>-derived schema type <paramref name="schema"/>.
		/// </summary>
		/// <param name="schema">
		/// The <see cref="SchemaBase"/>-derived schema type.
		/// </param>
		/// <returns>
		/// The dummy instance document as an <see cref="XmlDocument"/>.
		/// </returns>
		[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Done by .IsSchema() extension method.")]
		[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global", Justification = "Public API.")]
		public static XmlDocument CreateMessage(Type schema)
		{
			if (!schema.IsSchema())
				throw new ArgumentException(
					$"{schema.FullName} does not derive from {typeof(SchemaBase).FullName}.",
					nameof(schema));
			return CreateMessage(SchemaMetadata.For(schema).DocumentSpec);
		}

		/// <summary>
		/// Creates a dummy instance document for a given schema <see cref="DocumentSpec"/>.
		/// </summary>
		/// <param name="documentSpec">
		/// The schema <see cref="DocumentSpec"/>.
		/// </param>
		/// <returns>
		/// The dummy instance document.
		/// </returns>
		/// <seealso href="http://biztalkmessages.vansplunteren.net/2008/06/19/create-message-instance-from-multiroot-xsd-using-documentspec/"/>
		private static XmlDocument CreateMessage(DocumentSpec documentSpec)
		{
			using (var writer = new StringWriter())
			using (var reader = XmlReader.Create(documentSpec.CreateXmlInstance(writer), new XmlReaderSettings { CloseInput = true, XmlResolver = null }))
			{
				var document = new XmlDocument { XmlResolver = null };
				document.Load(reader);
				return document;
			}
		}
	}
}
