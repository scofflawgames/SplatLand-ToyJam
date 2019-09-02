using CSString = System.String;
using System.Collections.Generic;

namespace Toy {
	namespace Library {
		class String : IPlugin {
			//singleton pattern
			public IPlugin Singleton {
				get {
					if (singleton == null) {
						return singleton = new String();
					}
					return singleton;
				}
			}
			static String singleton = null;

			//IPlugin
			public void Initialize(Environment env, string alias) {
				env.Define(CSString.IsNullOrEmpty(alias) ? "String" : alias, this, true);
			}

			//static indexing members
			public static object SliceNotationLiteral(Expr callee, Token token, object first, object second, object third) {
				//bounds checking
				if (!(first is double) || ((double)first != double.NegativeInfinity && (double)first < 0) || ((double)first != double.NegativeInfinity && (double)first >= ((string)((Literal)callee).value).Length)) {
					throw new ErrorHandler.RuntimeError(token, "First index must be a number and between 0 and String.Length(string) -1 (inclusive)");
				}

				if (second != null) {
					if (!(second is double) || ((double)second != double.PositiveInfinity && (double)second < 0) || ((double)second != double.PositiveInfinity && (double)second >= ((string)((Literal)callee).value).Length)) {
						throw new ErrorHandler.RuntimeError(token, "Second index must be a number and between 0 and String.Length(string) -1 (inclusive)");
					}
				}

				if (second == null) {
					//access this character only
					return ((string)((Literal)callee).value)[(int)(double)first];
				}

				//default values for slice notation (begin and end are inclusive)
				int begin = (double)first == double.NegativeInfinity ? 0 : (int)(double)first;
				int end = (double)second == double.PositiveInfinity ? ((string)((Literal)callee).value).Length - 1 : (int)(double)second;
				int step = third == null ? 1 : (int)(double)third;

				//check for infinite loops
				if (step == 0) {
					throw new ErrorHandler.RuntimeError(token, "Can't have a string step of 0");
				}

				//build the new string
				string newString = "";
				for (int index = step > 0 ? begin : end; index >= begin && index <= end; index += step) {
					newString += ((string)((Literal)callee).value)[index];
				}

				return new StringLiteralAssignableIndex(callee, newString, begin, end, token, third);
			}

			public class StringLiteralAssignableIndex : AssignableIndex {
				Expr callee;
				string newString;
				int begin;
				int end;
				Token token;
				object third;

				public StringLiteralAssignableIndex(Expr callee, string newString, int begin, int end, Token token, object third) {
					this.callee = callee;
					this.newString = newString;
					this.begin = begin;
					this.end = end;
					this.token = token;
					this.third = third;
				}

				public override object Value {
					get {
						return newString;
					}
					set {
						//disallow a third index
						if (third != null) {
							throw new ErrorHandler.RuntimeError(token, "Unexpected third slice index found");
						}

						//switch
						if (begin > end) {
							int tmp = begin;
							begin = end;
							end = tmp;
						}

						string mutable = ((string)((Literal)callee).value).Remove(begin, end - begin + 1);
						mutable = mutable.Insert(begin, (string)value);
						(((Literal)callee).value) = (object)mutable;
					}
				}
			}

			public static object SliceNotationVariable(Expr callee, Token token, Interpreter interpreter, object first, object second, object third) {
				//bounds checking
				if (!(first is double) || ((double)first != double.NegativeInfinity && (double)first < 0) || ((double)first != double.NegativeInfinity && (double)first >= ((string)interpreter.LookupVariable(callee)).Length)) {
					throw new ErrorHandler.RuntimeError(token, "First index must be a number and between 0 and String.Length(string) -1 (inclusive)");
				}

				if (second != null) {
					if (!(second is double) || ((double)second != double.PositiveInfinity && (double)second < 0) || ((double)second != double.PositiveInfinity && (double)second >= ((string)interpreter.LookupVariable(callee)).Length)) {
						throw new ErrorHandler.RuntimeError(token, "Second index must be a number and between 0 and String.Length(string) -1 (inclusive)");
					}
				}

				if (second == null) {
					//access this character only
					return ((string)interpreter.LookupVariable(callee))[(int)(double)first];
				}

				//default values for slice notation (begin and end are inclusive)
				int begin = (double)first == double.NegativeInfinity ? 0 : (int)(double)first;
				int end = (double)second == double.PositiveInfinity ? ((string)interpreter.LookupVariable(callee)).Length - 1 : (int)(double)second;
				int step = third == null ? 1 : (int)(double)third;

				//check for infinite loops
				if (step == 0) {
					throw new ErrorHandler.RuntimeError(token, "Can't have a string step of 0");
				}

				//build the new string
				string newString = "";
				for (int index = step > 0 ? begin : end; index >= begin && index <= end; index += step) {
					newString += ((string)interpreter.LookupVariable(callee))[index];
				}

				return new StringVariableAssignableIndex(callee, newString, interpreter, begin, end, token, third);
			}

			public class StringVariableAssignableIndex : AssignableIndex {
				Expr callee;
				string newString;
				Interpreter interpreter;
				int begin;
				int end;
				Token token;
				object third;

				public StringVariableAssignableIndex(Expr callee, string newString, Interpreter interpreter, int begin, int end, Token token, object third) {
					this.callee = callee;
					this.newString = newString;
					this.interpreter = interpreter;
					this.begin = begin;
					this.end = end;
					this.token = token;
					this.third = third;
				}

				public override object Value {
					get {
						return newString;
					}
					set {
						//disallow a third index
						if (third != null) {
							throw new ErrorHandler.RuntimeError(token, "Unexpected third slice index found");
						}

						//switch
						if (begin > end) {
							int tmp = begin;
							begin = end;
							end = tmp;
						}

						string mutable = ((string)interpreter.LookupVariable(callee)).Remove(begin, end - begin + 1);
						mutable = mutable.Insert(begin, (string)value);
						interpreter.AssignVariable(callee, (object)mutable);
					}
				}
			}

			//static property members
			public static object LiteralProperty(string str, Token name, string lexeme) {
				switch(lexeme) {
					case "Length": return new Length(str);
					case "ToLower": return new ToLower(str);
					case "ToUpper": return new ToUpper(str);
					case "Replace": return new Replace(str);
					case "Trim": return new Trim(str);
					case "IndexOf": return new IndexOf(str);
					case "LastIndexOf": return new LastIndexOf(str);

					default:
						throw new ErrorHandler.RuntimeError(name, "Unknown property '" + lexeme + "'");
				}
			}

			public static object VariableProperty(Variable expr, Interpreter interpreter, Token name, string lexeme) {
				string str = (string)interpreter.LookupVariable(expr);

				switch(lexeme) {
					case "Length": return new Length(str);
					case "ToLower": return new ToLower(str);
					case "ToUpper": return new ToUpper(str);
					case "Replace": return new Replace(str);
					case "Trim": return new Trim(str);
					case "IndexOf": return new IndexOf(str);
					case "LastIndexOf": return new LastIndexOf(str);

					default:
						throw new ErrorHandler.RuntimeError(name, "Unknown property '" + lexeme + "'");
				}
			}

			//callable members
			public class Length : ICallable {
				string self;

				public Length(string self) {
					this.self = self;
				}

				public int Arity() {
					return 0;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					return (double)self.Length;
				}
			}

			public class ToLower : ICallable {
				string self;

				public ToLower(string self) {
					this.self = self;
				}

				public int Arity() {
					return 0;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					return self.ToLower();
				}
			}

			public class ToUpper : ICallable {
				string self;

				public ToUpper(string self) {
					this.self = self;
				}

				public int Arity() {
					return 0;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					return self.ToUpper();
				}
			}

			public class Replace : ICallable {
				string self;

				public Replace(string self) {
					this.self = self;
				}

				public int Arity() {
					return 2;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					string pat = (string)arguments[0];
					string rep = (string)arguments[1];

					return self.Replace(pat, rep);
				}
			}

			public class Trim : ICallable {
				string self;

				public Trim(string self) {
					this.self = self;
				}

				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					string chars = (string)arguments[0];

					return self.Trim(chars.ToCharArray());
				}
			}

			public class IndexOf : ICallable {
				string self;

				public IndexOf(string self) {
					this.self = self;
				}

				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					string other = (string)arguments[0];

					return (double)self.IndexOf(other);
				}
			}

			public class LastIndexOf : ICallable {
				string self;

				public LastIndexOf(string self) {
					this.self = self;
				}

				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					string other = (string)arguments[0];

					return (double)self.LastIndexOf(other);
				}
			}
		}
	}
}