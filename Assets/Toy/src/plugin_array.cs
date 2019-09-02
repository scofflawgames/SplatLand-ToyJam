using System;
using System.Collections.Generic;

namespace Toy {
	namespace Plugin {
		//the plugin class
		public class Array : IPlugin, ICallable, IBundle {
			//singleton pattern
			public IPlugin Singleton {
				get {
					if (singleton == null) {
						return singleton = new Array();
					}
					return singleton;
				}
			}
			static Array singleton = null;

			//IPlugin
			public void Initialize(Environment env, string alias) {
				env.Define(String.IsNullOrEmpty(alias) ? "Array" : alias, this, true);
			}

			//ICallable
			public int Arity() {
				return 0;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				return new ArrayInstance(new List<object>());
			}

			//IBundle
			public object Property(Interpreter interpreter, Token token, object argument) {
				string propertyName = (string)argument;

				switch(propertyName) {
					case "IsArray": return new IsArrayInstance(this);

					default:
						throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
				}
			}

			//callable properties
			public class IsArrayInstance : ICallable {
				Array self = null;

				public IsArrayInstance(Array self) {
					this.self = self;
				}

				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					return arguments[0] is ArrayInstance;
				}

				public override string ToString() { return "<Array property>"; }
			}

			//the index assign helper class
			public class ArrayAssignableIndex : AssignableIndex { //TODO: should be called element assign?
				List<object> container;
				int index;

				public ArrayAssignableIndex(List<object> container, int index) {
					this.container = container;
					this.index = index;
				}

				public override object Value {
					get { return this.container[this.index]; }
					set { this.container[this.index] = value; }
				}
			}

			//the instance class
			public class ArrayInstance : ICollection, IBundle {
				//container members
				public List<object> container = null;

				//methods
				public ArrayInstance(List<object> arguments) {
					container = arguments;
				}

				//ICollection
				public object Access(Interpreter interpreter, Token token, object first, object second, object third) {
					//bounds checking
					if (!(first is double) || ((double)first != double.NegativeInfinity && (double)first < 0) || ((double)first != double.NegativeInfinity && (double)first >= container.Count)) {
						throw new ErrorHandler.RuntimeError(token, "First index must be a number and between 0 and Array.Length() -1 (inclusive)");
					}

					if (second != null) {
						if (!(second is double) || ((double)second != double.PositiveInfinity && (double)second < 0) || ((double)second != double.PositiveInfinity && (double)second >= container.Count)) {
							throw new ErrorHandler.RuntimeError(token, "Second index must be a number and between 0 and Array.Length() -1 (inclusive)");
						}
					}

					//index
					if (second == null) {
						return new ArrayAssignableIndex(this.container, (int)(double)first);
					}

					//default values for slice notation (begin and end are inclusive)
					int begin = (double)first == double.NegativeInfinity ? 0 : (int)(double)first;
					int end = (double)second == double.PositiveInfinity ? container.Count - 1 : (int)(double)second;
					int step = third == null ? 1 : (int)(double)third;

					//check for infinite loops
					if (step == 0) {
						throw new ErrorHandler.RuntimeError(token, "Can't have an array step of 0");
					}

					//build the new array
					List<object> result = new List<object>();
					for (int index = step > 0 ? begin : end; index >= begin && index <= end; index += step) {
						result.Add(container[index]);
					}

					//return a new array
					return new ArrayInstance(result);
				}

				//IBundle
				public object Property(Interpreter interpreter, Token token, object argument) {
					string propertyName = (string)argument;

					switch(propertyName) {
						case "Push": return new Push(this);
						case "Pop": return new Pop(this);
						case "Unshift": return new Unshift(this);
						case "Shift": return new Shift(this);
						case "Length": return new Length(this);
						case "Sort": return new Sort(this);
						case "Insert": return new Insert(this);
						case "Delete": return new Delete(this);

						case "ContainsValue": return new ContainsValue(this);
						case "Every": return new Every(this);
						case "Any": return new Any(this);
						case "Filter": return new Filter(this);
						case "ForEach": return new ForEach(this);
						case "Map": return new Map(this);
						case "Reduce": return new Reduce(this);
						case "Concat": return new Concat(this);
						case "Clear": return new Clear(this);
						case "Equals": return new EqualsCallable(this);

						case "ToString": return new ToStringCallable(this);

						default:
							throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
					}
				}

				public class Push : ICallable {
					ArrayInstance self = null;

					public Push(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						self.container.Add(arguments[0]);
						return null;
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Pop : ICallable {
					ArrayInstance self = null;

					public Pop(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 0;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						object result = self.container[self.container.Count - 1];
						self.container.RemoveAt(self.container.Count - 1);
						return result;
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Unshift : ICallable {
					ArrayInstance self = null;

					public Unshift(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						self.container.Insert(0, arguments[0]);
						return null;
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Shift : ICallable {
					ArrayInstance self = null;

					public Shift(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 0;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						object result = self.container[0];
						self.container.RemoveAt(0);
						return result;
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Length : ICallable {
					ArrayInstance self = null;

					public Length(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 0;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						return (double)self.container.Count;
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Sort : ICallable {
					ArrayInstance self = null;
					ScriptFunction comparator = null;
					Interpreter interpreter = null;
					Token token = null;

					public Sort(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						this.comparator = (ScriptFunction)arguments[0];
						this.interpreter = interpreter;
						this.token = token;

						if (this.comparator.Arity() != 2) {
							throw new ErrorHandler.RuntimeError(token, "Callback has an incorrect number of parameters (expected 2, found " + this.comparator.Arity() + ")");
						}

						self.container.Sort(SortComparison);

						return null;
					}

					int SortComparison(object lhs, object rhs) {
						object result = comparator.Call(interpreter, token, new List<object>() { lhs, rhs });
						if (!(result is double)) {
							throw new ErrorHandler.RuntimeError(token, "Unexpected result type from comparator function (expected number)");
						}

						return Convert.ToInt32(result);
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Insert : ICallable {
					ArrayInstance self = null;

					public Insert(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 2;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						int pos = Convert.ToInt32(arguments[0]);

						if (pos < 0 || pos > self.container.Count) {
							throw new ErrorHandler.RuntimeError(token, "Specified index is out of bounds");
						}

						self.container.Insert(pos, arguments[1]);
						return null;
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Delete : ICallable {
					ArrayInstance self = null;

					public Delete(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						int pos = Convert.ToInt32(arguments[0]);

						if (pos < 0 || pos >= self.container.Count) {
							throw new ErrorHandler.RuntimeError(token, "Specified index is out of bounds");
						}

						self.container.RemoveAt(pos);
						return null;
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class ContainsValue : ICallable {
					ArrayInstance self = null;

					public ContainsValue(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						return self.container.Contains(arguments[0]);
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Every : ICallable {
					ArrayInstance self = null;

					public Every(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						ScriptFunction callback = (ScriptFunction)arguments[0];

						if (callback.Arity() != 1) {
							throw new ErrorHandler.RuntimeError(token, "Callback has an incorrect number of parameters (expected 1, found " + callback.Arity() + ")");
						}

						foreach(object element in self.container) {
							object result = callback.Call(interpreter, token, new List<object>() { element });
							if (!interpreter.CheckIsTruthy(result)) {
								return false;
							}
						}

						return true;
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Any : ICallable {
					ArrayInstance self = null;

					public Any(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						ScriptFunction callback = (ScriptFunction)arguments[0];

						if (callback.Arity() != 1) {
							throw new ErrorHandler.RuntimeError(token, "Callback has an incorrect number of parameters (expected 1, found " + callback.Arity() + ")");
						}

						foreach(object element in self.container) {
							object result = callback.Call(interpreter, token, new List<object>() { element });
							if (interpreter.CheckIsTruthy(result)) {
								return true;
							}
						}

						return false;
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Filter : ICallable {
					ArrayInstance self = null;

					public Filter(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						ScriptFunction callback = (ScriptFunction)arguments[0];
						List<object> resultsContainer = new List<object>();

						if (callback.Arity() != 1) {
							throw new ErrorHandler.RuntimeError(token, "Callback has an incorrect number of parameters (expected 1, found " + callback.Arity() + ")");
						}

						foreach(object element in self.container) {
							object result = callback.Call(interpreter, token, new List<object>() { element });
							if (interpreter.CheckIsTruthy(result)) {
								resultsContainer.Add(element);
							}
						}

						return new ArrayInstance(resultsContainer);
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class ForEach : ICallable {
					ArrayInstance self = null;

					public ForEach(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						ScriptFunction callback = (ScriptFunction)arguments[0];

						if (callback.Arity() != 1) {
							throw new ErrorHandler.RuntimeError(token, "Callback has an incorrect number of parameters (expected 1, found " + callback.Arity() + ")");
						}

						foreach(object element in self.container) {
							callback.Call(interpreter, token, new List<object>() { element });
						}

						return null;
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Map : ICallable {
					ArrayInstance self = null;

					public Map(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						ScriptFunction callback = (ScriptFunction)arguments[0];
						List<object> resultsContainer = new List<object>();

						if (callback.Arity() != 1) {
							throw new ErrorHandler.RuntimeError(token, "Callback has an incorrect number of parameters (expected 1, found " + callback.Arity() + ")");
						}

						foreach(object element in self.container) {
							object result = callback.Call(interpreter, token, new List<object>() { element });
							resultsContainer.Add(result);
						}

						return new ArrayInstance(resultsContainer);
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Reduce : ICallable {
					ArrayInstance self = null;

					public Reduce(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 2;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						object accumulator = arguments[0];
						ScriptFunction callback = (ScriptFunction)arguments[1];

						if (callback.Arity() != 2) {
							throw new ErrorHandler.RuntimeError(token, "Callback has an incorrect number of parameters (expected 2, found " + callback.Arity() + ")");
						}

						foreach(object element in self.container) {
							accumulator = callback.Call(interpreter, token, new List<object>() { accumulator, element });
						}

						return accumulator;
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Concat : ICallable {
					ArrayInstance self = null;

					public Concat(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						if (!(arguments[0] is ArrayInstance)) {
							throw new ErrorHandler.RuntimeError(token, "Expected an Array instance");
						}

						ArrayInstance rhs = (ArrayInstance)arguments[0];

						List<object> resultsList = new List<object>();

						foreach(object element in self.container) {
							resultsList.Add(element);
						}

						foreach(object element in rhs.container) {
							resultsList.Add(element);
						}

						return new ArrayInstance(resultsList);
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class Clear : ICallable {
					ArrayInstance self = null;

					public Clear(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 0;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						self.container.Clear();
						return null;
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class EqualsCallable : ICallable {
					ArrayInstance self = null;

					public EqualsCallable(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						if (!(arguments[0] is ArrayInstance)) {
							throw new ErrorHandler.RuntimeError(token, "Expected an Array instance");
						}

						ArrayInstance rhs = (ArrayInstance)arguments[0];

						if (self.container.Count != rhs.container.Count) {
							return false;
						}

						for (int i = 0; i < self.container.Count; i++) {
							if (!interpreter.CheckIsEqual(self.container[i], rhs.container[i])) {
								return false;
							}
						}

						return true;
					}

					public override string ToString() { return "<Array function>"; }
				}

				public class ToStringCallable : ICallable {
					ArrayInstance self = null;

					public ToStringCallable(ArrayInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 0;
					}

					public object Call(Interpreter interpreter, Token token, List<object> arguments) {
						return self.ToString();
					}

					public override string ToString() { return "<Array function>"; }
				}

				public override string ToString() {
					//prevent circular references
					if (ToStringHelper.passed.ContainsKey(this)) {
						return "<circular reference>";
					}
					ToStringHelper.passed[this] = 0;

					//build the result
					string result = "";
					foreach(object o in container) {
						if (o is Literal) {
							result += ((Literal)o).ToString();
						} else {
							result += o.ToString();
						}

						result += ",";
					}

					//trim the last ',' character
					if (result.Length > 0 && result[result.Length - 1] == ',') {
						result = result.Substring(0, result.Length - 1);
					}

					//cleanup and return
					ToStringHelper.passed.Remove(this);
					return "[" + result + "]";
				}
			}

			public override string ToString() { return "<plugin>"; }
		}
	}
}