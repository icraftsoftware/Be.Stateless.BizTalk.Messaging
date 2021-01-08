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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dummies.Schema;
using Be.Stateless.BizTalk.Schema.Annotation;
using Be.Stateless.BizTalk.Schema.Extensions;
using Be.Stateless.BizTalk.Schemas.Xml;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Schema
{
	public class ContextPropertyAnnotationFixture
	{
		[Fact]
		public void ExtractorCollectionIsEmptyForEmptySchemaAnnotationReader()
		{
			var sut = new ContextPropertyAnnotation();

			sut.Build(SchemaAnnotationReader.Empty);

			sut.Extractors.Should().BeEmpty().And.BeOfType<PropertyExtractorCollection.EmptyPropertyExtractorCollection>();
		}

		[Fact]
		public void ExtractorCollectionIsEmptyForMicrosoftSoap12Schema()
		{
			SchemaMetadata.For<BTS.soap_envelope_1__2.Fault>().Annotations.Find<ContextPropertyAnnotation>().Extractors.Should().BeEmpty();
		}

		[Fact]
		public void ExtractorCollectionIsEmptyWhenSchemaHasNoAnnotation()
		{
			SchemaMetadata.For<Envelope>().Annotations.Find<ContextPropertyAnnotation>().Extractors.Should().BeEmpty();
		}

		[Fact]
		public void ExtractorCollectionIsNotEmptyWhenSchemaHasSomeAnnotations()
		{
			SchemaMetadata.For<RootedSchema>().Annotations.Find<ContextPropertyAnnotation>().Extractors
				.Should().BeEquivalentTo(
					new PropertyExtractorCollection(
						new XPathExtractor(BizTalkFactoryProperties.MapTypeName, "/*[local-name()='Root']//*[local-name()='Id']")
					));
		}

		[Fact]
		public void GetExtractorsIsAnAcceleratorExtensionMethod()
		{
			SchemaMetadata.For<RootedSchema>().Annotations.Find<ContextPropertyAnnotation>().Extractors
				.Should().BeSameAs(SchemaMetadata.For<RootedSchema>().GetExtractors());
		}
	}
}
