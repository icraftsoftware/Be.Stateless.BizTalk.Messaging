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

using Be.Stateless.BizTalk.Schemas.Xml;
using BTS;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Extensions
{
	public class TypeExtensionsFixture
	{
		[Fact]
		public void AnyType()
		{
			typeof(string).IsSchema().Should().BeFalse();
			typeof(string).IsDocumentSchema().Should().BeFalse();
			typeof(string).IsSchemaRoot().Should().BeFalse();
		}

		[Fact]
		public void PropertySchema()
		{
			typeof(Schemas.BizTalkFactory.Properties).IsSchema().Should().BeTrue();
			typeof(Schemas.BizTalkFactory.Properties).IsDocumentSchema().Should().BeFalse();
			typeof(Schemas.BizTalkFactory.Properties).IsSchemaRoot().Should().BeFalse();
		}

		[Fact]
		public void RootFromSchemaWithSeveralRoots()
		{
			typeof(soap_envelope_1__2.Envelope).IsSchema().Should().BeTrue();
			typeof(soap_envelope_1__2.Envelope).IsDocumentSchema().Should().BeFalse();
			typeof(soap_envelope_1__2.Envelope).IsSchemaRoot().Should().BeTrue();
		}

		[Fact]
		public void SchemaWithSeveralRoots()
		{
			typeof(soap_envelope_1__2).IsSchema().Should().BeTrue();
			typeof(soap_envelope_1__2).IsDocumentSchema().Should().BeTrue();
			typeof(soap_envelope_1__2).IsSchemaRoot().Should().BeFalse();
		}

		[Fact]
		public void SchemaWithSingleRoot()
		{
			typeof(Any).IsSchema().Should().BeTrue();
			typeof(Any).IsDocumentSchema().Should().BeTrue();
			typeof(Any).IsSchemaRoot().Should().BeTrue();
		}
	}
}
