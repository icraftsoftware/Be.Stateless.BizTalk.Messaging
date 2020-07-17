﻿#region Copyright & License

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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Adapter.Wcf.Metadata;

namespace Be.Stateless.BizTalk.Adapter.Metadata
{
	/// <summary>
	/// Describes one BizTalk Server variable mapping declared through HTTP Method and URL mapping operations.
	/// </summary>
	public class VariablePropertyMapping : BtsVariablePropertyMappingVariable
	{
		public VariablePropertyMapping(string name, IMessageContextProperty property)
		{
			if (name.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(name));
			if (property == null) throw new ArgumentNullException(nameof(property));
			Name = name;
			PropertyName = property.Name;
			PropertyNamespace = property.Namespace;
		}
	}
}
