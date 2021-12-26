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
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Adapter.Metadata
{
	public class ActionMappingFixture
	{
		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("  ")]
		public void OperationActionCannotBeEmpty(string action)
		{
			Invoking(() => new ActionMapping { new("UpdateTicket", action) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("action");
		}

		[Fact]
		public void OperationCanBeMappedToNoAction()
		{
			Invoking(() => new ActionMapping { new("UpdateTicket") }).Should().NotThrow();
		}

		[Fact]
		public void OperationCanBeMappedToSomeAction()
		{
			Invoking(() => new ActionMapping { new("UpdateTicket", "http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/UPDATE_TICKET") })
				.Should().NotThrow();
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("  ")]
		public void OperationNameCannotBeEmpty(string name)
		{
			Invoking(() => new ActionMapping { new(name) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("name");
		}

		[Fact]
		public void SerializeToXmlString()
		{
			var actionMapping = new ActionMapping {
				new("CreateTicket"),
				new("UpdateTicket", "http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/UPDATE_TICKET")
			};

			((string) actionMapping).Should().Be(
				"<BtsActionMapping>"
				+ "<Operation Name=\"CreateTicket\" />"
				+ "<Operation Name=\"UpdateTicket\" Action=\"http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/UPDATE_TICKET\" />"
				+ "</BtsActionMapping>");
		}
	}
}
