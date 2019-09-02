using System;
using CSString = System.String;
using System.Collections.Generic;

namespace Toy {
	namespace Library {
		public class Standard : IPlugin {
			//singleton pattern
			public IPlugin Singleton {
				get {
					if (singleton == null) {
						return singleton = new Standard();
					}
					return singleton;
				}
			}
			static Standard singleton = null;

			//the persistent functors
			static Clock clock = new Clock();
			static Random random = new Random();
			static RandomSeed randomSeed = new RandomSeed();
			static ToNumber toNumber = new ToNumber();
			static ToStringCallable toStringCallable = new ToStringCallable();
			static ToBoolean toBoolean = new ToBoolean();
			static GetTypeCallable getTypeCallable = new GetTypeCallable();
			static IsSame isSame = new IsSame();

			public void Initialize(Environment env, string alias) {
				if (CSString.IsNullOrEmpty(alias)) {
					//no alias, put these in the global scope
					env.Define("Clock", clock, true);
					env.Define("Random", random, true);
					env.Define("RandomSeed", randomSeed, true);
					env.Define("ToNumber", toNumber, true);
					env.Define("ToString", toStringCallable, true);
					env.Define("ToBoolean", toBoolean, true);
					env.Define("GetType", getTypeCallable, true);
					env.Define("IsSame", isSame, true);
				} else {
					env.Define(alias, new Bundle(), true);
				}
			}

			//member class - the library as a bundle (for the alias)
			public class Bundle : IBundle {
				public object Property(Interpreter interpreter, Token token, object argument) {
					string propertyName = (string)argument;

					switch(propertyName) { //TODO: string constants
						case "Clock": return clock;
						case "Random": return random;
						case "RandomSeed": return randomSeed;
						case "ToNumber": return toNumber;
						case "ToString": return toStringCallable;
						case "ToBoolean": return toBoolean;
						case "GetType": return getTypeCallable;
						case "IsSame": return isSame;

						default:
							throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
					}
				}
			}

			//member classes representing functions
			public class Clock : ICallable {
				public int Arity() {
					return 0;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					return new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() / (double)1000;
				}

				public override string ToString() { return "<native function>"; }
			}

			public class Random : ICallable {
				System.Random rand = null;

				public Random(int seed = int.MinValue) {
					if (seed == int.MinValue) {
						seed = new DateTimeOffset(DateTime.Now).Millisecond;
					}

					//initialize the randomizer
					if (rand == null) {
						rand = new System.Random(seed);
					}
				}

				public int Arity() {
					return 0;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					return rand.NextDouble();
				}

				public override string ToString() { return "<native function>"; }
			}

			public class RandomSeed : ICallable {
				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					//TODO: hashing a string seed
					if (!(arguments[0] is double)) {
						throw new ErrorHandler.RuntimeError(token, "Unexpected type received (expected number)");
					}

					random = new Random((int)(double)arguments[0]);
					return null;
				}

				public override string ToString() { return "<native function>"; }
			}

			public class ToNumber : ICallable {
				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					if (arguments[0] is double) {
						return arguments[0];
					}

					if (arguments[0] is string) {
						return Convert.ToDouble((string)arguments[0]);
					}

					if (arguments[0] is bool) {
						return interpreter.CheckIsTruthy(arguments[0]) ? 1 : 0;
					}

					throw new ErrorHandler.RuntimeError(token, "Can only convert booleans and strings to numbers");
				}

				public override string ToString() { return "<native function>"; }
			}

			public class ToStringCallable : ICallable {
				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					if (arguments[0] is bool) {
						return interpreter.CheckIsTruthy(arguments[0]) ? "true" : "false";
					}

					if (arguments[0] == null) {
						return "null";
					}

					return arguments[0].ToString();
				}

				public override string ToString() { return "<native function>"; }
			}

			public class ToBoolean : ICallable {
				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					if (arguments[0] is string) {
						if (((string)arguments[0]) == "false") {
							return false; //one exception to the normal rules
						}
					}

					return interpreter.CheckIsTruthy(arguments[0]);
				}

				public override string ToString() { return "<native function>"; }
			}

			public class GetTypeCallable : ICallable {
				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					if (arguments[0] is double) {
						return "number";
					}

					if (arguments[0] is string) {
						return "string";
					}

					if (arguments[0] is bool) {
						return "boolean";
					}

					if (arguments[0] is IPlugin) {
						return "plugin";
					}

					if (arguments[0] is ICallable) {
						return "function";
					}

					if (arguments[0] == null) {
						return "null";
					}

					if (arguments[0] is AliasedFile) {
						return "alias";
					}

					//default
					return "instance";
				}

				public override string ToString() { return "<native function>"; }
			}

			public class IsSame : ICallable {
				public int Arity() {
					return 2;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					return Object.ReferenceEquals(arguments[0], arguments[1]);
				}

				public override string ToString() { return "<native function>"; }
			}
		}
	}
}
