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

using System.Xml.Linq;
using Be.Stateless.BizTalk.Dummies.Schema;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Schema
{
	public class SchemaAnnotationReaderFixture
	{
		[Fact]
		public void CreateReturnsEmptySchemaAnnotationReaderForMicrosoftAssembly()
		{
			SchemaAnnotationReader.Create(SchemaMetadata.For<BTS.soap_envelope_1__2.Fault>()).Should().BeOfType<SchemaAnnotationReader.EmptySchemaAnnotationReader>();
		}

		[Fact]
		public void CreateReturnsEmptySchemaAnnotationReaderForRootlessSchemaMetadata()
		{
			SchemaAnnotationReader.Create(SchemaMetadata.For<RootlessSchema>()).Should().BeOfType<SchemaAnnotationReader.EmptySchemaAnnotationReader>();
		}

		[Fact]
		public void CreateReturnsEmptySchemaAnnotationReaderForUnknownSchemaMetadata()
		{
			SchemaAnnotationReader.Create(SchemaMetadata.For(typeof(string))).Should().BeOfType<SchemaAnnotationReader.EmptySchemaAnnotationReader>();
		}

		[Fact]
		public void CreateReturnsSchemaAnnotationReaderForRootedSchemaMetadata()
		{
			SchemaAnnotationReader.Create(SchemaMetadata.For<RootedSchema>()).Should().BeOfType<SchemaAnnotationReader>();
		}

		[Fact]
		public void GetAnnotationElementReturnsNullForEmptySchemaAnnotationReader()
		{
			SchemaAnnotationReader.Empty.GetAnnotationElement(null).Should().BeNull();
			SchemaAnnotationReader.Empty.GetAnnotationElement(string.Empty).Should().BeNull();
			SchemaAnnotationReader.Empty.GetAnnotationElement("any").Should().BeNull();
		}

		[Fact]
		public void GetAnnotationElementReturnsNullWhenAnnotationElementNameIsEmpty()
		{
			SchemaAnnotationReader.Create(SchemaMetadata.For<RootedSchema>()).GetAnnotationElement(string.Empty).Should().BeNull();
		}

		[Fact]
		public void GetAnnotationElementReturnsNullWhenAnnotationElementNameIsNull()
		{
			SchemaAnnotationReader.Create(SchemaMetadata.For<RootedSchema>()).GetAnnotationElement(null).Should().BeNull();
		}

		[Fact]
		public void GetAnnotationElementReturnsNullWhenNotFound()
		{
			SchemaAnnotationReader.Create(SchemaMetadata.For<RootedSchema>()).GetAnnotationElement("Unknown").Should().BeNull();
		}

		[Fact]
		public void GetAnnotationElementReturnsXElementWhenFound()
		{
			SchemaAnnotationReader.Create(SchemaMetadata.For<RootedSchema>()).GetAnnotationElement("Properties").Should().NotBeNull().And.BeOfType<XElement>();
		}
	}
}
