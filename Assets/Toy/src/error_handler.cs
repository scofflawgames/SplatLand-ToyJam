using System;

using static Toy.TokenType;

namespace Toy {
	class ErrorHandler {
		//types for throwing
		public class AssertError : ApplicationException {
			public AssertError(Token token, string message) {
				if (token.type == EOF) {
					ErrorHandler.Report(token.line, "at end", message);
				} else {
					ErrorHandler.Report(token.line, "at '" + token.lexeme + "'", message);
				}
			}
		}

		public class ParserError : ApplicationException {
			public ParserError(Token token, string message) {
				if (token.type == EOF) {
					ErrorHandler.Report(token.line, "at end", message);
				} else {
					ErrorHandler.Report(token.line, "at '" + token.lexeme + "'", message);
				}
			}
		}

		public class ResolverError : ApplicationException {
			public ResolverError(Token token, string message) {
				if (token.type == EOF) {
					ErrorHandler.Report(token.line, "at end", message);
				} else {
					ErrorHandler.Report(token.line, "at '" + token.lexeme + "'", message);
				}
			}
		}

		public class RuntimeError : ApplicationException {
			public RuntimeError(Token token, string message) {
				ErrorHandler.Report(token.line, "at '" + token.lexeme + "'", message);
			}
		}

		//variables
		public static bool HadError { get; private set; } = false;

		//basic error handling
		public static void Error(int line, string message) {
			Report(line, "", message);
		}

		private static void Report(int line, string where, string message) {
			ConsoleOutput.Log($"[line {line}] Error {where}: {message}");
			HadError = true;
		}

		public static void ResetError() {
			HadError = false;
		}
	}
}