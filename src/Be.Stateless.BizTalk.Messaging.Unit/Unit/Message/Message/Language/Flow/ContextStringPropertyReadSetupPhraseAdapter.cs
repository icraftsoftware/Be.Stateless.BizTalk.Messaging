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
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Moq.Language.Flow;

namespace Be.Stateless.BizTalk.Unit.Message.Message.Language.Flow
{
	internal class ContextStringPropertyReadSetupPhraseAdapter<TMock> : ISetup<TMock, string> where TMock : class, IBaseMessageContext
	{
		internal ContextStringPropertyReadSetupPhraseAdapter(ISetup<TMock, object> setup)
		{
			_setupImplementation = setup;
		}

		#region ISetup<TMock,string> Members

		public IReturnsThrows<TMock, string> Callback(InvocationAction action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback(System.Delegate callback)
		{
			_setupImplementation.Callback(callback);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback(Action action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T>(Action<T> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2>(Action<T1, T2> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3>(Action<T1, T2, T3> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsThrows<TMock, string> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action)
		{
			_setupImplementation.Callback(action);
			return this;
		}

		public IReturnsResult<TMock> Returns(string value)
		{
			return _setupImplementation.Returns(value);
		}

		public IReturnsResult<TMock> Returns(InvocationFunc valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns(System.Delegate valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns(Func<string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T>(Func<T, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> CallBase()
		{
			return _setupImplementation.CallBase();
		}

		public IReturnsResult<TMock> Returns<T1, T2>(Func<T1, T2, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3>(Func<T1, T2, T3, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, string> valueFunction)
		{
			return _setupImplementation.Returns(valueFunction);
		}

		public IThrowsResult Throws(Exception exception)
		{
			return _setupImplementation.Throws(exception);
		}

		public IThrowsResult Throws<TException>() where TException : Exception, new()
		{
			return _setupImplementation.Throws<TException>();
		}

		public void Verifiable()
		{
			_setupImplementation.Verifiable();
		}

		public void Verifiable(string failMessage)
		{
			_setupImplementation.Verifiable(failMessage);
		}

		#endregion

		private readonly ISetup<TMock, object> _setupImplementation;
	}
}
