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
using log4net;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public static class BaseMessagePart
	{
		/// <summary>
		/// Replaces this <paramref name="messagePart"/>'s original data stream by another <paramref name="stream"/>.
		/// </summary>
		/// <param name="messagePart">
		/// The part whose original data stream is replaced.
		/// </param>
		/// <param name="stream">
		/// The replacement stream.
		/// </param>
		/// <param name="tracker">
		/// Pipeline's resource tracker to which to report the newly created wrapping stream.
		/// </param>
		public static void SetDataStream(this IBaseMessagePart messagePart, System.IO.Stream stream, IResourceTracker tracker)
		{
			if (messagePart == null) throw new ArgumentNullException(nameof(messagePart));
			if (stream == null) throw new ArgumentNullException(nameof(stream));
			if (tracker == null) throw new ArgumentNullException(nameof(tracker));

			messagePart.Data = stream;
			tracker.AddResource(stream);
		}

		/// <summary>
		/// Wraps this message part's original data stream in another stream returned by the <paramref name="wrapper"/> delegate.
		/// </summary>
		/// <param name="messagePart">
		/// The part whose original data stream is wrapped.
		/// </param>
		/// <param name="wrapper">
		/// A delegate, or stream factory, that returns the stream wrapping the original one.
		/// </param>
		/// <param name="tracker">
		/// Pipeline's resource tracker to which to report the newly created wrapping stream.
		/// </param>
		/// <returns>
		/// The new wrapping <see cref="Stream"/> if it is not the same instance as the original one. The original <see
		/// cref="Stream"/> otherwise.
		/// </returns>
		public static T WrapOriginalDataStream<T>(this IBaseMessagePart messagePart, Func<System.IO.Stream, T> wrapper, IResourceTracker tracker) where T : System.IO.Stream
		{
			if (messagePart == null) throw new ArgumentNullException(nameof(messagePart));
			if (wrapper == null) throw new ArgumentNullException(nameof(wrapper));
			if (tracker == null) throw new ArgumentNullException(nameof(tracker));

			var originalDataStream = messagePart.GetOriginalDataStream();
			if (originalDataStream == null) return null;

			var wrappingStream = wrapper(originalDataStream);
			if (ReferenceEquals(originalDataStream, wrappingStream)) return (T) originalDataStream;

			if (_logger.IsDebugEnabled) _logger.DebugFormat("Wrapping message part's original data stream in a '{0}' stream.", wrappingStream.GetType().FullName);
			messagePart.SetDataStream(wrappingStream, tracker);
			return wrappingStream;
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(BaseMessagePart));
	}
}
