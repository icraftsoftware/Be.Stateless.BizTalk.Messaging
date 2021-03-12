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
using System.IO;
using FluentAssertions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public class BaseMessagePartFixture
	{
		[Fact]
		public void SetDataStream()
		{
			var stream = new MemoryStream();
			var part = new Mock<IBaseMessagePart>().SetupAllProperties();
			var tracker = new Mock<IResourceTracker>();

			part.Object.SetDataStream(stream, tracker.Object);

			part.Object.Data.Should().BeSameAs(stream);
			tracker.Verify(t => t.AddResource(stream));
		}

		[Fact]
		public void SetDataStreamThrowsIfNullArguments()
		{
			Invoking(() => ((IBaseMessagePart) null).SetDataStream(null, null))
				.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("messagePart");

			var part = new Mock<IBaseMessagePart>().SetupAllProperties();
			Invoking(() => part.Object.SetDataStream(null, null))
				.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("stream");

			Invoking(() => part.Object.SetDataStream(new MemoryStream(), null))
				.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("tracker");
		}

		[Fact]
		public void WrapOriginalDataStream()
		{
			var originalStream = new MemoryStream();
			var part = new Mock<IBaseMessagePart>()
				.SetupAllProperties();
			part.Setup(p => p.GetOriginalDataStream())
				.Returns(originalStream);
			var tracker = new Mock<IResourceTracker>();

			System.IO.Stream wrapper = null;
			part.Object.WrapOriginalDataStream(s => wrapper = new MarkableForwardOnlyEventingReadStream(s), tracker.Object);

			part.Object.Data.Should().BeSameAs(wrapper);
			tracker.Verify(t => t.AddResource(wrapper));
		}

		[Fact]
		public void WrapOriginalDataStreamDoesNothingIfWrappingStreamIsSameAsOriginalStream()
		{
			var originalStream = new MemoryStream();
			var part = new Mock<IBaseMessagePart>()
				.SetupAllProperties();
			part.Setup(p => p.GetOriginalDataStream())
				.Returns(originalStream);
			var tracker = new Mock<IResourceTracker>();

			part.Object.WrapOriginalDataStream(s => s, tracker.Object);

			// is null because mock did not setup property... which allows us to indirectly test that no wrapping has taken place
			part.Object.Data.Should().BeNull();
			tracker.Verify(t => t.AddResource(It.IsAny<object>()), Times.Never());
		}

		[Fact]
		public void WrapOriginalDataStreamThrowsIfNullArguments()
		{
			Invoking(() => ((IBaseMessagePart) null).WrapOriginalDataStream<System.IO.Stream>(null, null))
				.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("messagePart");

			var part = new Mock<IBaseMessagePart>().SetupAllProperties();
			Invoking(() => part.Object.WrapOriginalDataStream<System.IO.Stream>(null, null))
				.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("wrapper");

			Invoking(() => part.Object.WrapOriginalDataStream(s => new MarkableForwardOnlyEventingReadStream(s), null))
				.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("tracker");
		}
	}
}
