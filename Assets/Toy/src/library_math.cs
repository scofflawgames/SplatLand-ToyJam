using CSMath = System.Math;
using CSString = System.String;
using System;
using System.Collections.Generic;

namespace Toy {
	namespace Library {
		class Math : IPlugin, IBundle {
			//singleton pattern
			public IPlugin Singleton {
				get {
					if (singleton == null) {
						return singleton = new Math();
					}
					return singleton;
				}
			}
			static Math singleton = null;

			//IPlugin
			public void Initialize(Environment env, string alias) {
				env.Define(CSString.IsNullOrEmpty(alias) ? "Math" : alias, this, true);
			}

			//IBundle
			public object Property(Interpreter interpreter, Token token, object argument) {
				string propertyName = (string)argument;

				//utility functions
				Func<double, double, double> NthRoot = (b, e) => {
					if (e % 1 != 0 || e <= 0) {
						throw new ErrorHandler.RuntimeError(token, "Exponent must be a whole number above 0");
					}
					return CSMath.Pow(b, 1 / e);
				};

				Func<double, double> Asinh = (x) => CSMath.Log(x + CSMath.Sqrt(x * x + 1));
				Func<double, double> Acosh = (x) => CSMath.Log(x + CSMath.Sqrt(x * x - 1));
				Func<double, double> Atanh = (x) => CSMath.Log((1 + x) / (1 - x)) / 2;

				switch(propertyName) {
					case "PI": return 3.14159265358979;
					case "E": return 2.71828182845905;

					case "Abs": return new CallableArity1(this, CSMath.Abs);
					case "Floor": return new CallableArity1(this, CSMath.Floor);
					case "Ceil": return new CallableArity1(this, CSMath.Ceiling);
					case "Round": return new CallableArity1(this, CSMath.Round);

					case "Pow": return new CallableArity2(this, CSMath.Pow);
					case "Root": return new CallableArity2(this, NthRoot); //doesn't exist in C# yet

					case "Log": return new CallableArity1(this, CSMath.Log);
					case "Exp": return new CallableArity1(this, CSMath.Exp);

					case "Sin": return new CallableArity1(this, CSMath.Sin);
					case "Cos": return new CallableArity1(this, CSMath.Cos);
					case "Tan": return new CallableArity1(this, CSMath.Tan);
					case "Asin": return new CallableArity1(this, CSMath.Asin);
					case "Acos": return new CallableArity1(this, CSMath.Acos);
					case "Atan": return new CallableArity1(this, CSMath.Atan);

					case "Sinh": return new CallableArity1(this, CSMath.Sinh);
					case "Cosh": return new CallableArity1(this, CSMath.Cosh);
					case "Tanh": return new CallableArity1(this, CSMath.Tanh);
					case "Asinh": return new CallableArity1(this, Asinh); //doesn't exist in C# yet
					case "Acosh": return new CallableArity1(this, Acosh); //doesn't exist in C# yet
					case "Atanh": return new CallableArity1(this, Atanh); //doesn't exist in C# yet

					default:
						throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
				}
			}

			//callable types
			public class CallableArity1 : ICallable {
				Math self = null;
				Func<double, double> expression = null;

				public CallableArity1(Math self, Func<double, double> expression) {
					this.self = self;
					this.expression = expression;
				}

				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					double result = expression((double)arguments[0]);

					if (result == double.NegativeInfinity || result == double.PositiveInfinity || double.IsNaN(result)) {
						throw new ErrorHandler.RuntimeError(token, "Invalid result from math function");
					}

					return result;
				}
			}

			public class CallableArity2 : ICallable {
				Math self = null;
				Func<double, double, double> expression = null;

				public CallableArity2(Math self, Func<double, double, double> expression) {
					this.self = self;
					this.expression = expression;
				}

				public int Arity() {
					return 2;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					double result = expression((double)arguments[0], (double)arguments[1]);

					if (result == double.NegativeInfinity || result == double.PositiveInfinity || double.IsNaN(result)) {
						throw new ErrorHandler.RuntimeError(token, "Invalid result from math function");
					}

					return result;
				}
			}
		}
	}
}