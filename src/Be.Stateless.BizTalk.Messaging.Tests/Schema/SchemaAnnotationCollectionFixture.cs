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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dummies.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using FluentAssertions;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Schema
{
	public class SchemaAnnotationCollectionFixture
	{
		[Fact]
		public void FindEntailsSchemaAnnotationInstanceBuildingForEachSchema()
		{
			SchemaAnnotationCollection.Create(SchemaMetadata.For<Envelope>()).Find<AnnotationDummy>().Should().NotBeNull();
			AnnotationDummy.AnnotationDummySpy
				.Verify(ads => ads.Build(It.IsAny<ISchemaAnnotationReader>()), Times.Once);

			SchemaAnnotationCollection.Create(SchemaMetadata.For<Any>()).Find<AnnotationDummy>().Should().NotBeNull();
			AnnotationDummy.AnnotationDummySpy
				.Verify(ads => ads.Build(It.IsAny<ISchemaAnnotationReader>()), Times.Once);

			SchemaAnnotationCollection.Create(SchemaMetadata.For<BTS.soap_envelope_1__2.Fault>()).Find<AnnotationDummy>().Should().NotBeNull();
			AnnotationDummy.AnnotationDummySpy
				.Verify(ads => ads.Build(It.IsAny<ISchemaAnnotationReader>()), Times.Once);
		}

		[Fact]
		public void FindEntailsSingleSchemaAnnotationInstanceBuildingForOneSchema()
		{
			// notice we create and reuse SchemaAnnotationCollection similarly to what SchemaMetadata does
			var schemaAnnotationCollection = SchemaAnnotationCollection.Create(SchemaMetadata.For<Envelope>());

			var annotationDummy = schemaAnnotationCollection.Find<AnnotationDummy>();
			annotationDummy.Should().NotBeNull();

			AnnotationDummy.AnnotationDummySpy
				.Verify(ads => ads.Build(It.IsAny<ISchemaAnnotationReader>()), Times.Once);
			AnnotationDummy.AnnotationDummySpy.Invocations.Clear();

			// build is not called anymore and same SchemaAnnotation instance is returned
			schemaAnnotationCollection.Find<AnnotationDummy>().Should().BeSameAs(annotationDummy);
			AnnotationDummy.AnnotationDummySpy
				.Verify(ads => ads.Build(It.IsAny<ISchemaAnnotationReader>()), Times.Never);
		}

		[Fact]
		public void FindReturnsDifferentAnnotationInstanceForDifferentSchema()
		{
			SchemaMetadata.For<RootedSchema>().Annotations.Find<AnnotationDummy>()
				.Should().NotBeSameAs(SchemaMetadata.For<Any>().Annotations.Find<AnnotationDummy>());
		}

		[Fact]
		public void FindReturnsSameAnnotationInstanceForSameSchema()
		{
			SchemaMetadata.For<BTS.soap_envelope_1__2.Fault>().Annotations.Find<AnnotationDummy>()
				.Should().BeSameAs(SchemaMetadata.For<BTS.soap_envelope_1__2.Fault>().Annotations.Find<AnnotationDummy>());
		}

		[Fact]
		public void FindReturnsSoughtSchemaAnnotationInstance()
		{
			SchemaMetadata.For<BTS.soap_envelope_1__2.Fault>().Annotations.Find<AnnotationDummy>().Should().BeOfType<AnnotationDummy>();
		}

		public class AnnotationDummy : ISchemaAnnotation<AnnotationDummy>
		{
			internal static Mock<ISchemaAnnotation<AnnotationDummy>> AnnotationDummySpy { get; private set; }

			public AnnotationDummy()
			{
				AnnotationDummySpy = new Mock<ISchemaAnnotation<AnnotationDummy>>();
				AnnotationDummySpy
					.Setup(ads => ads.Build(It.IsAny<ISchemaAnnotationReader>()))
					.Returns(new AnnotationDummy(true));
			}

			[SuppressMessage("ReSharper", "UnusedParameter.Local")]
			private AnnotationDummy(bool skipSpySetup) { }

			#region ISchemaAnnotation<AnnotationDummy> Members

			public AnnotationDummy Build(ISchemaAnnotationReader schemaAnnotationReader)
			{
				return AnnotationDummySpy.Object.Build(schemaAnnotationReader);
			}

			#endregion
		}
	}
}
