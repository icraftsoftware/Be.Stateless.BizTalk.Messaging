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
using System.Net.Http;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Adapter.Metadata
{
	public class HttpUrlMappingFixture
	{
		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("  ")]
		public void MethodCannotBeEmpty(string method)
		{
			Invoking(() => new HttpUrlMapping { new("AddCustomer", method, null) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("method");
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("  ")]
		public void OperationNameCannotBeEmpty(string name)
		{
			Invoking(() => new HttpUrlMapping { new(name, null, null) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("name");
		}

		[Fact]
		public void SerializeToXmlString()
		{
			var httpUrlMapping = new HttpUrlMapping {
				new("AddCustomer", HttpMethod.Post.Method, "/Customer/{id}"),
				new("DeleteCustomer", HttpMethod.Delete.Method, "/Customer/{id}")
			};

			((string) httpUrlMapping).Should().Be(
				"<BtsHttpUrlMapping>"
				+ "<Operation Name=\"AddCustomer\" Method=\"POST\" Url=\"/Customer/{id}\" />"
				+ "<Operation Name=\"DeleteCustomer\" Method=\"DELETE\" Url=\"/Customer/{id}\" />"
				+ "</BtsHttpUrlMapping>");
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("  ")]
		public void UrlCannotBeEmpty(string url)
		{
			Invoking(() => new HttpUrlMapping { new("AddCustomer", HttpMethod.Delete.Method, url) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("url");
		}
	}
}
