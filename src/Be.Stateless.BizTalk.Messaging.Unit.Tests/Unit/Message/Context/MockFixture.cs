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

using System.Text.RegularExpressions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using FluentAssertions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Xunit;
using static Be.Stateless.BizTalk.DelegateFactory;

namespace Be.Stateless.BizTalk.Unit.Message.Context
{
	public class MockFixture
	{
		[Fact]
		public void AnyFunctionCanBeUsedToSetupExtensionMethod()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			Action(() => context.Object.GetProperty(BtsProperties.AckRequired)).Should().NotThrow();
			context.Object.GetProperty(BtsProperties.AckRequired).Should().BeTrue();
			Action(() => context.Object.GetProperty(BtsProperties.ActualRetryCount)).Should().NotThrow();
			context.Object.GetProperty(BtsProperties.ActualRetryCount).Should().Be(10);
			Action(() => context.Object.GetProperty(BtsProperties.SendPortName)).Should().NotThrow();
			context.Object.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");
		}

		[Fact]
		public void AnyPredicateCanBeUsedToSetupPromoteExtensionMethod()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, It.Is<bool>(b => b)));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, It.Is<int>(i => i % 2 == 0)));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, It.Is<string>(s => s.IsQName())));

			Action(() => context.Object.Promote(BtsProperties.AckRequired, true)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.ActualRetryCount, 12)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.SendPortName, "ns:name")).Should().NotThrow();
		}

		[Fact]
		public void AnyPredicateCanBeUsedToSetupSetPropertyExtensionMethod()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, It.Is<bool>(b => !b)));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, It.Is<int>(i => i % 2 != 0)));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, It.Is<string>(s => !s.IsQName())));

			Action(() => context.Object.SetProperty(BtsProperties.AckRequired, false)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 11)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.SendPortName, "any name")).Should().NotThrow();
		}

		[Fact]
		public void AnyValueCanBeUsedToSetupPromoteExtensionMethod()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, It.IsAny<string>()));

			Action(() => context.Object.Promote(BtsProperties.AckRequired, false)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.ActualRetryCount, 11)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.SendPortName, "any-send-port-name")).Should().NotThrow();
		}

		[Fact]
		public void AnyValueCanBeUsedToSetupSetPropertyExtensionMethod()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, It.IsAny<string>()));

			Action(() => context.Object.SetProperty(BtsProperties.AckRequired, false)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 11)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.SendPortName, "any-send-port-name")).Should().NotThrow();
		}

		[Fact]
		public void AnyValueOrPredicateCanBeUsedToVerifyPromoteExtensionMethod()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>();

			context.Object.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10);
			context.Object.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true);
			context.Object.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name");

			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)));
			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)), Times.Once);
			context.Verify(c => c.Promote(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Verify(c => c.Promote(BtsProperties.AckRequired, It.Is<bool>(b => b)));
			context.Verify(c => c.Promote(BtsProperties.AckRequired, It.Is<bool>(b => b)), Times.Once);
			context.Verify(c => c.Promote(BtsProperties.SendPortName, It.IsAny<string>()));
			context.Verify(c => c.Promote(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")));
			context.Verify(c => c.Promote(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")), Times.Once);
		}

		[Fact]
		public void AnyValueOrPredicateCanBeUsedToVerifySetPropertyExtensionMethod()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>();

			context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10);
			context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true);
			context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name");

			context.Verify(c => c.SetProperty(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Verify(c => c.SetProperty(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)));
			context.Verify(c => c.SetProperty(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)), Times.Once);
			context.Verify(c => c.SetProperty(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Verify(c => c.SetProperty(BtsProperties.AckRequired, It.Is<bool>(b => b)));
			context.Verify(c => c.SetProperty(BtsProperties.AckRequired, It.Is<bool>(b => b)), Times.Once);
			context.Verify(c => c.SetProperty(BtsProperties.SendPortName, It.IsAny<string>()));
			context.Verify(c => c.SetProperty(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")));
			context.Verify(c => c.SetProperty(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")), Times.Once);
		}

		[Fact]
		public void CallBackSetupAfterContextStringPropertyReadSetup()
		{
			var callbackCount = 0;

			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.ReceiveLocationName))
				.Callback((string n, string ns) => callbackCount += n == BtsProperties.ReceiveLocationName.Name ? 1 : -1)
				.Returns("receive-location-name");
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName))
				.Callback((string n, string ns) => callbackCount += n == BtsProperties.SendPortName.Name ? 1 : -1)
				.Returns(() => "send-port-name");

			Action(() => context.Object.GetProperty(BtsProperties.ReceiveLocationName)).Should().NotThrow();
			context.Object.GetProperty(BtsProperties.ReceiveLocationName).Should().Be("receive-location-name");
			Action(() => context.Object.GetProperty(BtsProperties.SendPortName)).Should().NotThrow();
			context.Object.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");

			callbackCount.Should().Be(4);
		}

		[Fact]
		public void ContextPropertyValueDefaultsToNullWithoutSetupIfMockBehaviorIsLoose()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Loose);

			context.Object.GetProperty(BtsProperties.AckRequired).Should().BeNull();
			context.Object.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace).Should().BeNull();
			context.Object.GetProperty(BtsProperties.ActualRetryCount).Should().BeNull();
			context.Object.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace).Should().BeNull();
			context.Object.GetProperty(BtsProperties.SendPortName).Should().BeNull();
			context.Object.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace).Should().BeNull();
		}

		[Fact]
		public void DeletePropertyExtensionMethodCanBeSetupByWrite()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, null));
			context.Setup(c => c.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, null));
			context.Setup(c => c.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, null));

			Action(() => context.Object.DeleteProperty(BtsProperties.AckRequired)).Should().NotThrow();
			Action(() => context.Object.DeleteProperty(BtsProperties.ActualRetryCount)).Should().NotThrow();
			Action(() => context.Object.DeleteProperty(BtsProperties.SendPortName)).Should().NotThrow();
		}

		[Fact]
		public void DeletePropertyExtensionMethodCanVerifyWriteSetupAndCall()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, null));
			context.Setup(c => c.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, null));
			context.Setup(c => c.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, null));

			Action(() => context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, null)).Should().NotThrow();
			Action(() => context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, null)).Should().NotThrow();
			Action(() => context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, null)).Should().NotThrow();

			context.Verify(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Verify(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Verify(c => c.DeleteProperty(BtsProperties.SendPortName));
		}

		[Fact]
		public void DeletePropertyExtensionMethodSetupAndCall()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Setup(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Setup(c => c.DeleteProperty(BtsProperties.SendPortName));

			Action(() => context.Object.DeleteProperty(BtsProperties.AckRequired)).Should().NotThrow();
			Action(() => context.Object.DeleteProperty(BtsProperties.ActualRetryCount)).Should().NotThrow();
			Action(() => context.Object.DeleteProperty(BtsProperties.SendPortName)).Should().NotThrow();
		}

		[Fact]
		public void DeletePropertyExtensionMethodSetupAndCallThroughWrite()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Setup(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Setup(c => c.DeleteProperty(BtsProperties.SendPortName));

			Action(() => context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, null)).Should().NotThrow();
			Action(() => context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, null)).Should().NotThrow();
			Action(() => context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, null)).Should().NotThrow();
		}

		[Fact]
		public void DeletePropertyExtensionMethodSetupAndVerify()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Setup(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Setup(c => c.DeleteProperty(BtsProperties.SendPortName));

			Action(() => context.Object.DeleteProperty(BtsProperties.AckRequired)).Should().NotThrow();
			Action(() => context.Object.DeleteProperty(BtsProperties.ActualRetryCount)).Should().NotThrow();
			Action(() => context.Object.DeleteProperty(BtsProperties.SendPortName)).Should().NotThrow();

			context.Verify(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Verify(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Verify(c => c.DeleteProperty(BtsProperties.SendPortName));
		}

		[Fact]
		public void DeletePropertyExtensionMethodSetupAndVerifyThroughWrite()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Setup(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Setup(c => c.DeleteProperty(BtsProperties.SendPortName));

			Action(() => context.Object.DeleteProperty(BtsProperties.AckRequired)).Should().NotThrow();
			Action(() => context.Object.DeleteProperty(BtsProperties.ActualRetryCount)).Should().NotThrow();
			Action(() => context.Object.DeleteProperty(BtsProperties.SendPortName)).Should().NotThrow();

			context.Verify(c => c.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, null));
			context.Verify(c => c.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, null));
			context.Verify(c => c.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, null));
		}

		[Fact]
		public void DeletePropertyExtensionMethodSetupAndVerifyWhenCalledThroughWrite()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Setup(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Setup(c => c.DeleteProperty(BtsProperties.SendPortName));

			Action(() => context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, null)).Should().NotThrow();
			Action(() => context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, null)).Should().NotThrow();
			Action(() => context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, null)).Should().NotThrow();

			context.Verify(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Verify(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Verify(c => c.DeleteProperty(BtsProperties.SendPortName));
		}

		[Fact]
		public void GetPropertyExtensionMethodCanBeSetupByRead()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace)).Returns(true);
			context.Setup(c => c.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace)).Returns(10);
			context.Setup(c => c.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace)).Returns("send-port-name");

			context.Object.GetProperty(BtsProperties.AckRequired).Should().BeTrue();
			context.Object.GetProperty(BtsProperties.ActualRetryCount).Should().Be(10);
			context.Object.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");
		}

		[Fact]
		public void GetPropertyExtensionMethodCanVerifyReadSetupAndCall()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace)).Returns(true);
			context.Setup(c => c.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace)).Returns(10);
			context.Setup(c => c.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace)).Returns("send-port-name");

			context.Object.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace).Should().Be(true);
			context.Object.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace).Should().Be(10);
			context.Object.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace).Should().Be("send-port-name");

			context.Verify(c => c.GetProperty(BtsProperties.AckRequired));
			context.Verify(c => c.GetProperty(BtsProperties.ActualRetryCount));
			context.Verify(c => c.GetProperty(BtsProperties.SendPortName));
		}

		[Fact]
		public void GetPropertyExtensionMethodSetupAndCall()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			context.Object.GetProperty(BtsProperties.AckRequired).Should().BeTrue();
			context.Object.GetProperty(BtsProperties.ActualRetryCount).Should().Be(10);
			context.Object.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");
		}

		[Fact]
		public void GetPropertyExtensionMethodSetupAndCallThroughRead()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			context.Object.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace).Should().Be(true);
			context.Object.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace).Should().Be(10);
			context.Object.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace).Should().Be("send-port-name");
		}

		[Fact]
		public void GetPropertyExtensionMethodSetupAndVerify()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			context.Object.GetProperty(BtsProperties.AckRequired).Should().BeTrue();
			context.Object.GetProperty(BtsProperties.ActualRetryCount).Should().Be(10);
			context.Object.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");

			context.Verify(c => c.GetProperty(BtsProperties.AckRequired));
			context.Verify(c => c.GetProperty(BtsProperties.ActualRetryCount));
			context.Verify(c => c.GetProperty(BtsProperties.SendPortName));
		}

		[Fact]
		public void GetPropertyExtensionMethodSetupAndVerifyThroughRead()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			context.Object.GetProperty(BtsProperties.AckRequired).Should().BeTrue();
			context.Object.GetProperty(BtsProperties.ActualRetryCount).Should().Be(10);
			context.Object.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");

			context.Verify(c => c.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace));
			context.Verify(c => c.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace));
			context.Verify(c => c.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace));
		}

		[Fact]
		public void GetPropertyExtensionMethodSetupAndVerifyWhenCalledThroughRead()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			context.Object.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace).Should().Be(true);
			context.Object.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace).Should().Be(10);
			context.Object.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace).Should().Be("send-port-name");

			context.Verify(c => c.GetProperty(BtsProperties.AckRequired));
			context.Verify(c => c.GetProperty(BtsProperties.ActualRetryCount));
			context.Verify(c => c.GetProperty(BtsProperties.SendPortName));
		}

		[Fact]
		public void IsPromotedExtensionMethodCallAndVerifyRequiresPromoteExtensionMethodSetup()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));

			// notice IsPromoted() extension method returns false because it also calls GetProperty() to ensure that the
			// property actually has a value as prescribed by BizTalk: a promoted property must have a value
			context.Object.IsPromoted(BtsProperties.AckRequired).Should().BeTrue();
			context.Object.IsPromoted(BtsProperties.ActualRetryCount).Should().BeTrue();
			context.Object.IsPromoted(BtsProperties.SendPortName).Should().BeTrue();

			// IsPromoted() call can be verified when setup via Promote() extension method because it actually sets up two
			// core operations as well: Read() and IsPromoted()
			context.Verify(c => c.IsPromoted(BtsProperties.AckRequired));
			context.Verify(c => c.IsPromoted(BtsProperties.ActualRetryCount));
			context.Verify(c => c.IsPromoted(BtsProperties.SendPortName));
		}

		[Fact]
		public void IsPromotedExtensionMethodCallWillFailWhenIsPromotedExtensionMethodOnlyIsSetup()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Loose); // notice behavior is loose
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName)).Returns(true);

			// notice IsPromoted() extension method returns false because it also calls GetProperty() to ensure that the
			// property actually has a value as prescribed by BizTalk: a promoted property must have a value
			context.Object.IsPromoted(BtsProperties.AckRequired).Should().BeFalse();
			context.Object.IsPromoted(BtsProperties.ActualRetryCount).Should().BeFalse();
			context.Object.IsPromoted(BtsProperties.SendPortName).Should().BeFalse();

			// IsPromoted() call cannot be verified as well because it is actually rewritten as two core operations: Read() and IsPromoted()
			Action(() => context.Verify(c => c.IsPromoted(BtsProperties.AckRequired))).Should().Throw<MockException>();
			Action(() => context.Verify(c => c.IsPromoted(BtsProperties.ActualRetryCount))).Should().Throw<MockException>();
			Action(() => context.Verify(c => c.IsPromoted(BtsProperties.SendPortName))).Should().Throw<MockException>();
		}

		[Fact]
		public void IsPromotedExtensionMethodCannotBeSetupByCoreIsPromoted()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Loose); // notice behavior is loose
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace)).Returns(true);

			// notice IsPromoted() extension method returns false because it also calls GetProperty() to ensure that the
			// property actually has a value as prescribed by BizTalk: a promoted property must have a value
			context.Object.IsPromoted(BtsProperties.AckRequired).Should().BeFalse();
			context.Object.IsPromoted(BtsProperties.ActualRetryCount).Should().BeFalse();
			context.Object.IsPromoted(BtsProperties.SendPortName).Should().BeFalse();
		}

		[Fact]
		public void IsPromotedExtensionMethodCanVerifyCoreIsPromotedSetupAndCall()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace)).Returns(true);

			context.Object.IsPromoted(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace).Should().BeTrue();
			context.Object.IsPromoted(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace).Should().BeTrue();
			context.Object.IsPromoted(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace).Should().BeTrue();

			context.Verify(c => c.IsPromoted(BtsProperties.AckRequired));
			context.Verify(c => c.IsPromoted(BtsProperties.ActualRetryCount));
			context.Verify(c => c.IsPromoted(BtsProperties.SendPortName));
		}

		[Fact]
		public void IsPromotedExtensionMethodSetupAndCallThroughCoreIsPromoted()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName)).Returns(true);

			context.Object.IsPromoted(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace).Should().BeTrue();
			context.Object.IsPromoted(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace).Should().BeTrue();
			context.Object.IsPromoted(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace).Should().BeTrue();
		}

		[Fact]
		public void IsPromotedExtensionMethodSetupAndVerifyThroughCoreIsPromoted()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Loose); // notice behavior is loose
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName)).Returns(true);

			// notice IsPromoted() extension method returns false because it also calls GetProperty() to ensure that the
			// property actually has a value as prescribed by BizTalk: a promoted property must have a value
			context.Object.IsPromoted(BtsProperties.AckRequired).Should().BeFalse();
			context.Object.IsPromoted(BtsProperties.ActualRetryCount).Should().BeFalse();
			context.Object.IsPromoted(BtsProperties.SendPortName).Should().BeFalse();

			// Core IsPromoted() cannot be verified because IsPromoted() extension method calls core IsPromoted() after
			// having called core Read() method only if the latter returns a property value
			Action(() => context.Verify(c => c.IsPromoted(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace))).Should().Throw<MockException>();
			Action(() => context.Verify(c => c.IsPromoted(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace))).Should().Throw<MockException>();
			Action(() => context.Verify(c => c.IsPromoted(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace))).Should().Throw<MockException>();
		}

		[Fact]
		public void IsPromotedExtensionMethodSetupAndVerifyWhenCalledThroughCoreIsPromoted()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName)).Returns(true);

			context.Object.IsPromoted(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace).Should().BeTrue();
			context.Object.IsPromoted(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace).Should().BeTrue();
			context.Object.IsPromoted(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace).Should().BeTrue();

			context.Verify(c => c.IsPromoted(BtsProperties.AckRequired));
			context.Verify(c => c.IsPromoted(BtsProperties.ActualRetryCount));
			context.Verify(c => c.IsPromoted(BtsProperties.SendPortName));
		}

		[Fact]
		public void IsPromotedExtensionMethodVerifyWillFailWhenIsPromotedExtensionMethodOnlyIsSetup()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Loose); // notice behavior is loose
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName)).Returns(true);

			// notice IsPromoted() extension method returns false because it also calls GetProperty() to ensure that the
			// property actually has a value as prescribed by BizTalk: a promoted property must have a value
			context.Object.IsPromoted(BtsProperties.AckRequired).Should().BeFalse();
			context.Object.IsPromoted(BtsProperties.ActualRetryCount).Should().BeFalse();
			context.Object.IsPromoted(BtsProperties.SendPortName).Should().BeFalse();

			// IsPromoted() call cannot be verified as well because it is actually rewritten as two core operations: Read() and IsPromoted()
			Action(() => context.Verify(c => c.IsPromoted(BtsProperties.AckRequired))).Should().Throw<MockException>();
			Action(() => context.Verify(c => c.IsPromoted(BtsProperties.ActualRetryCount))).Should().Throw<MockException>();
			Action(() => context.Verify(c => c.IsPromoted(BtsProperties.SendPortName))).Should().Throw<MockException>();
		}

		[Fact]
		public void PromoteExtensionMethodCanBeSetupByCorePromote()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"));

			Action(() => context.Object.Promote(BtsProperties.AckRequired, true)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.ActualRetryCount, 10)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.SendPortName, "send-port-name")).Should().NotThrow();
		}

		[Fact]
		public void PromoteExtensionMethodCanVerifyCorePromoteSetupAndCall()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"));

			Action(() => context.Object.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name")).Should().NotThrow();

			context.Verify(c => c.Promote(BtsProperties.AckRequired, true));
			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Verify(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));
		}

		[Fact]
		public void PromoteExtensionMethodSetupAndCall()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));

			Action(() => context.Object.Promote(BtsProperties.AckRequired, true)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.ActualRetryCount, 10)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.SendPortName, "send-port-name")).Should().NotThrow();
		}

		[Fact]
		public void PromoteExtensionMethodSetupAndCallThroughCorePromote()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));

			Action(() => context.Object.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name")).Should().NotThrow();
		}

		[Fact]
		public void PromoteExtensionMethodSetupAndVerify()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));

			Action(() => context.Object.Promote(BtsProperties.AckRequired, true)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.ActualRetryCount, 10)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.SendPortName, "send-port-name")).Should().NotThrow();

			context.Verify(c => c.Promote(BtsProperties.AckRequired, true));
			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Verify(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));
		}

		[Fact]
		public void PromoteExtensionMethodSetupAndVerifyThroughCorePromote()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));

			Action(() => context.Object.Promote(BtsProperties.AckRequired, true)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.ActualRetryCount, 10)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.SendPortName, "send-port-name")).Should().NotThrow();

			context.Verify(c => c.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true));
			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10));
			context.Verify(c => c.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"));
		}

		[Fact]
		public void PromoteExtensionMethodSetupAndVerifyWhenCalledThroughCorePromote()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));

			Action(() => context.Object.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name")).Should().NotThrow();

			context.Verify(c => c.Promote(BtsProperties.AckRequired, true));
			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Verify(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));
		}

		[Fact]
		public void SetPropertyExtensionMethodCanBeSetupByWrite()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true));
			context.Setup(c => c.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10));
			context.Setup(c => c.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"));

			Action(() => context.Object.SetProperty(BtsProperties.AckRequired, true)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 10)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.SendPortName, "send-port-name")).Should().NotThrow();
		}

		[Fact]
		public void SetPropertyExtensionMethodCanVerifyWriteSetupAndCall()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true));
			context.Setup(c => c.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10));
			context.Setup(c => c.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"));

			Action(() => context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true)).Should().NotThrow();
			Action(() => context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10)).Should().NotThrow();
			Action(() => context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name")).Should().NotThrow();

			context.Verify(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Verify(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Verify(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));
		}

		[Fact]
		public void SetPropertyExtensionMethodSetupAndCall()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			Action(() => context.Object.SetProperty(BtsProperties.AckRequired, true)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 10)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.SendPortName, "send-port-name")).Should().NotThrow();
		}

		[Fact]
		public void SetPropertyExtensionMethodSetupAndCallThroughWrite()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			Action(() => context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true)).Should().NotThrow();
			Action(() => context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10)).Should().NotThrow();
			Action(() => context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name")).Should().NotThrow();
		}

		[Fact]
		public void SetPropertyExtensionMethodSetupAndVerify()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			Action(() => context.Object.SetProperty(BtsProperties.AckRequired, true)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 10)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.SendPortName, "send-port-name")).Should().NotThrow();

			context.Verify(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Verify(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Verify(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));
		}

		[Fact]
		public void SetPropertyExtensionMethodSetupAndVerifyThroughWrite()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			Action(() => context.Object.SetProperty(BtsProperties.AckRequired, true)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 10)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.SendPortName, "send-port-name")).Should().NotThrow();

			context.Verify(c => c.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true));
			context.Verify(c => c.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10));
			context.Verify(c => c.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"));
		}

		[Fact]
		public void SetPropertyExtensionMethodSetupAndVerifyWhenCalledThroughWrite()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			Action(() => context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true)).Should().NotThrow();
			Action(() => context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10)).Should().NotThrow();
			Action(() => context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name")).Should().NotThrow();

			context.Verify(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Verify(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Verify(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));
		}

		[Fact]
		public void VerifyOfVerifiableExtensionMethodSetup()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, true)).Verifiable();
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, 10)).Verifiable();
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name")).Verifiable();

			context.Object.SetProperty(BtsProperties.ActualRetryCount, 10);
			context.Object.SetProperty(BtsProperties.AckRequired, true);
			context.Object.SetProperty(BtsProperties.SendPortName, "send-port-name");

			context.Verify();
		}

		[Fact]
		public void VerifyOfVerifiableExtensionMethodSetupThrowsIfUnmatchedExpectation()
		{
			var context = new Message.Context.Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(true).Verifiable();
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(It.IsAny<int?>()).Verifiable();
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name").Verifiable();

			context.Object.GetProperty(BtsProperties.ActualRetryCount);
			context.Object.GetProperty(BtsProperties.AckRequired);

			Action(() => context.Verify())
				.Should().Throw<MockException>()
				.Where(
					e => Regex.IsMatch(
						e.Message,
						"This mock failed verification due to the following:.+"
						+ $"{nameof(IBaseMessageContext)} context => context\\.Read\\(\"{BtsProperties.SendPortName.Name}\", \"{BtsProperties.SendPortName.Namespace}\"\\):.+"
						+ "This setup was not matched\\.",
						RegexOptions.Singleline));
		}
	}
}
