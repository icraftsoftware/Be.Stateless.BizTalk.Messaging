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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Schema;
using Moq;

namespace Be.Stateless.BizTalk.Unit.Schema
{
	[SuppressMessage("Design", "CA1063:Implement IDisposable Correctly")]
	[SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Public API.")]
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
	public class SchemaMetadataMockInjectionScope : IDisposable
	{
		public SchemaMetadataMockInjectionScope()
		{
			_schemaMetadataFactory = SchemaMetadata.SchemaMetadataFactory;
			Mock = new();
			SchemaMetadata.SchemaMetadataFactory = _ => Mock.Object;
		}

		#region IDisposable Members

		[SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
		public void Dispose()
		{
			SchemaMetadata.SchemaMetadataFactory = _schemaMetadataFactory;
		}

		#endregion

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public Mock<ISchemaMetadata> Mock { get; }

		private readonly Func<Type, ISchemaMetadata> _schemaMetadataFactory;
	}
}
