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

using System.Diagnostics.CodeAnalysis;
using System.Xml;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Schema.Annotation
{
	public class PropertyExtractorCollectionConverterFixture
	{
		[Fact]
		public void CanConvertFrom()
		{
			var sut = new PropertyExtractorCollectionConverter();
			sut.CanConvertFrom(typeof(string)).Should().BeTrue();
		}

		[Fact]
		public void CanConvertTo()
		{
			var sut = new PropertyExtractorCollectionConverter();
			sut.CanConvertTo(typeof(string)).Should().BeTrue();
		}

		[Fact]
		[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
		public void ConvertFrom()
		{
			var xml = $@"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>
  <s0:Property1 xpath='*/some-node'/>
  <s0:Property2 promoted='true' xpath='*/other-node'/>
</san:Properties>";

			var sut = new PropertyExtractorCollectionConverter();
			sut.ConvertFrom(xml)
				.Should().BeOfType<PropertyExtractorCollection>()
				.Which.Should().BeEquivalentTo(
					new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write),
					new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote)
				);
		}

		[Fact]
		public void ConvertFromEmpty()
		{
			var sut = new PropertyExtractorCollectionConverter();
			sut.ConvertFrom(string.Empty)
				.Should().BeOfType<PropertyExtractorCollection.EmptyPropertyExtractorCollection>()
				.Which.Should().BeEmpty();
		}

		[Fact]
		public void ConvertFromNull()
		{
			var sut = new PropertyExtractorCollectionConverter();
			sut.ConvertFrom(null)
				.Should().BeOfType<PropertyExtractorCollection.EmptyPropertyExtractorCollection>()
				.Which.Should().BeEmpty();
		}

		[Fact]
		[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
		public void ConvertTo()
		{
			var xml = $"<s0:Properties xmlns:s0=\"{SchemaAnnotationCollection.NAMESPACE}\" xmlns:s1=\"urn\">"
				+ "<s1:Property1 xpath=\"*/some-node\" />"
				+ "<s1:Property2 mode=\"promote\" xpath=\"*/other-node\" />"
				+ "</s0:Properties>";

			var sut = new PropertyExtractorCollectionConverter();
			var extractorCollection = new PropertyExtractorCollection(
				new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote));

			sut.ConvertTo(extractorCollection, typeof(string)).Should().Be(xml);
		}

		[Fact]
		public void ConvertToNull()
		{
			var sut = new PropertyExtractorCollectionConverter();
			sut.ConvertTo(new PropertyExtractorCollection(), typeof(string)).Should().BeNull();
		}
	}
}
