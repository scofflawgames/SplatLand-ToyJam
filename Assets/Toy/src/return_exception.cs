using System;

namespace Toy {
	public class ReturnException : ApplicationException {
		public object result;
		public ReturnException(object value) {
			result = value;
		}
	}
}