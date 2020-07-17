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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Extensions;
using Be.Stateless.BizTalk.Stream.Extensions;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	/// <summary>
	/// Various <see cref="IBaseMessage"/> and <see cref="XLANGMessage"/> extension methods.
	/// </summary>
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public static class BaseMessageExtensions
	{
		public static bool IsAbout<T>(this IBaseMessage message) where T : SchemaBase
		{
			return message.GetProperty(BtsProperties.MessageType).IsOfType<T>();
		}

		public static bool IsAbout<T>(this IBaseMessageContext context) where T : SchemaBase
		{
			return context.GetProperty(BtsProperties.MessageType).IsOfType<T>();
		}

		public static string GetOrProbeMessageType(this IBaseMessage message, IResourceTracker resourceTracker)
		{
			var messageType = message.GetProperty(BtsProperties.MessageType);
			return messageType.IsNullOrEmpty()
				? message.ProbeMessageType(resourceTracker)
				: messageType;
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public static string ProbeMessageType(this IBaseMessage message, IResourceTracker resourceTracker)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			var markableForwardOnlyEventingReadStream = message.BodyPart.WrapOriginalDataStream(
				originalStream => originalStream.AsMarkable(),
				resourceTracker);
			var messageType = message.BodyPart.Data.EnsureMarkable().Probe().MessageType;
			markableForwardOnlyEventingReadStream.StopMarking();
			return messageType;
		}
	}
}
