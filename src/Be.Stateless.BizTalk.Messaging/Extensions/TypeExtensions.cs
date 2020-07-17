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

using System;
using System.Linq;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Extensions
{
	public static class TypeExtensions
	{
		/// <summary>
		/// Returns whether the <paramref name="type"/> is a <see cref="SchemaBase"/>-derived schema type.
		/// </summary>
		/// <param name="type">
		/// The <see cref="Type"/> to inspect.
		/// </param>
		/// <returns>
		/// <c>true</c> if <paramref name="type"/> is a <see cref="SchemaBase"/>-derived <see cref="Type"/>.
		/// </returns>
		public static bool IsSchema(this Type type)
		{
			return type != null && type.BaseType == typeof(SchemaBase);
		}

		/// <summary>
		/// Returns whether the <paramref name="type"/> is a <see cref="SchemaBase"/>-derived schema type and denotes an XML
		/// document schema.
		/// </summary>
		/// <param name="type">
		/// The <see cref="Type"/> to inspect.
		/// </param>
		/// <returns>
		/// <c>true</c> if <paramref name="type"/> is a <see cref="SchemaBase"/>-derived <see cref="Type"/> and denotes an XML
		/// document schema.
		/// </returns>
		public static bool IsDocumentSchema(this Type type)
		{
			return type.IsSchema() && Attribute
				.GetCustomAttributes(type, typeof(SchemaTypeAttribute))
				.Cast<SchemaTypeAttribute>()
				.Any(sta => sta.Type == SchemaTypeEnum.Document);
		}

		/// <summary>
		/// Returns whether the <paramref name="type"/> is a <see cref="SchemaBase"/>-derived schema type and denotes a schema
		/// root element.
		/// </summary>
		/// <param name="type">
		/// The <see cref="Type"/> to inspect.
		/// </param>
		/// <returns>
		/// <c>true</c> if <paramref name="type"/> is a <see cref="SchemaBase"/>-derived <see cref="Type"/> and denotes a schema
		/// root.
		/// </returns>
		public static bool IsSchemaRoot(this Type type)
		{
			return type.IsSchema() && Attribute.GetCustomAttributes(type, typeof(SchemaAttribute)).Any();
		}
	}
}
