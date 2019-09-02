using System;
using List = System.Collections.Generic.List<object>;
using Dict = System.Collections.Generic.Dictionary<object, object>;
using Pair = System.Collections.Generic.KeyValuePair<object, object>;

namespace Toy {
	namespace Plugin {
		//the plugin class
		public class Dictionary : IPlugin, ICallable, IBundle {
			public IPlugin Singleton {
				get {
					if (singleton == null) {
						return singleton = new Dictionary();
					}
					return singleton;
				}
			}
			static Dictionary singleton = null;

			//IPlugin
			public void Initialize(Environment env, string alias) {
				env.Define(String.IsNullOrEmpty(alias) ? "Dictionary" : alias, this, true);
			}

			//ICallable
			public int Arity() {
				return 0;
			}

			public object Call(Interpreter interpreter, Token token, List arguments) {
				return new DictionaryInstance(new Dict());
			}

			//IBundle
			public object Property(Interpreter interpreter, Token token, object argument) {
				string propertyName = (string)argument;

				switch(propertyName) {
					case "IsDictionary": return new IsDictionaryInstance(this);

					default:
						throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
				}
			}

			//callable properties
			public class IsDictionaryInstance : ICallable {
				Dictionary self = null;

				public IsDictionaryInstance(Dictionary self) {
					this.self = self;
				}

				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List arguments) {
					return arguments[0] is DictionaryInstance;
				}

				public override string ToString() { return "<Dictionary property>"; }
			}

			//the index assign helper class
			public class DictionaryAssignableIndex : AssignableIndex {
				Dict container;
				object index;

				public DictionaryAssignableIndex(Dict container, object index) {
					this.container = container;
					this.index = index;
				}

				public override object Value {
					get {
						return this.container.ContainsKey(index) ? this.container[this.index] : null;
					}
					set {
						if (this.container.ContainsKey(this.index)) {
							this.container.Remove(this.index);
						}
						this.container.Add(this.index, value);
					}
				}
			}

			//the instance class
			public class DictionaryInstance : ICollection, IBundle {
				//container members
				Dict container = null;

				//methods
				public DictionaryInstance(Dict dict) {
					container = dict;
				}

				//ICollection
				public object Access(Interpreter interpreter, Token token, object first, object second, object third) {
					if (first is double && (double)first == double.NegativeInfinity && second is double && (double)second == double.PositiveInfinity) {
						return new DictionaryInstance(new Dict(this.container));
					} else if (second != null || third != null) {
						throw new ErrorHandler.RuntimeError(token, "Can't use the slice notation with a Dictionary, except Dictionary[:]");
					}

					return new DictionaryAssignableIndex(container, first);
				}

				public object Property(Interpreter interpreter, Token token, object argument) {
					string propertyName = (string)argument;

					switch(propertyName) {
						case "Insert": return new Insert(this);
						case "Delete": return new Delete(this);
						case "Length": return new Length(this);

						case "ContainsKey": return new ContainsKey(this);
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

				public class Insert : ICallable {
					DictionaryInstance self = null;

					public Insert(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 2;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						if (self.container.ContainsKey(arguments[0])) {
							throw new ErrorHandler.RuntimeError(token, "Can't insert a duplicate key");
						}
						self.container[arguments[0]] = arguments[1];
						return null;
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class Delete : ICallable {
					DictionaryInstance self = null;

					public Delete(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						self.container.Remove(arguments[0]);
						return null;
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class Length : ICallable {
					DictionaryInstance self = null;

					public Length(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 0;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						return (double)self.container.Count;
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class ContainsKey : ICallable {
					DictionaryInstance self = null;

					public ContainsKey(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						return self.container.ContainsKey(arguments[0]);
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class ContainsValue : ICallable {
					DictionaryInstance self = null;

					public ContainsValue(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						return self.container.ContainsValue(arguments[0]);
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class Every : ICallable {
					DictionaryInstance self = null;

					public Every(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						ScriptFunction callback = (ScriptFunction)arguments[0];

						if (callback.Arity() != 2) {
							throw new ErrorHandler.RuntimeError(token, "Callback has an incorrect number of parameters (expected 2, found " + callback.Arity() + ")");
						}

						foreach(Pair kvp in self.container) {
							object result = callback.Call(interpreter, token, new List() { kvp.Key, kvp.Value });
							if (!interpreter.CheckIsTruthy(result)) {
								return false;
							}
						}

						return true;
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class Any : ICallable {
					DictionaryInstance self = null;

					public Any(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						ScriptFunction callback = (ScriptFunction)arguments[0];

						if (callback.Arity() != 2) {
							throw new ErrorHandler.RuntimeError(token, "Callback has an incorrect number of parameters (expected 2, found " + callback.Arity() + ")");
						}

						foreach(Pair kvp in self.container) {
							object result = callback.Call(interpreter, token, new List() { kvp.Key, kvp.Value });
							if (interpreter.CheckIsTruthy(result)) {
								return true;
							}
						}

						return false;
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class Filter : ICallable {
					DictionaryInstance self = null;

					public Filter(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						ScriptFunction callback = (ScriptFunction)arguments[0];
						Dict resultsContainer = new Dict();

						if (callback.Arity() != 2) {
							throw new ErrorHandler.RuntimeError(token, "Callback has an incorrect number of parameters (expected 2, found " + callback.Arity() + ")");
						}

						foreach(Pair kvp in self.container) {
							object result = callback.Call(interpreter, token, new List() { kvp.Key, kvp.Value });
							if (interpreter.CheckIsTruthy(result)) {
								resultsContainer[kvp.Key] = kvp.Value;
							}
						}

						return new DictionaryInstance(resultsContainer);
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class ForEach : ICallable {
					DictionaryInstance self = null;

					public ForEach(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						ScriptFunction callback = (ScriptFunction)arguments[0];

						if (callback.Arity() != 2) {
							throw new ErrorHandler.RuntimeError(token, "Callback has an incorrect number of parameters (expected 2, found " + callback.Arity() + ")");
						}

						foreach(Pair kvp in self.container) {
							callback.Call(interpreter, token, new List() { kvp.Key, kvp.Value });
						}

						return null;
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class Map : ICallable {
					DictionaryInstance self = null;

					public Map(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						ScriptFunction callback = (ScriptFunction)arguments[0];
						Dict resultsContainer = new Dict();

						if (callback.Arity() != 2) {
							throw new ErrorHandler.RuntimeError(token, "Callback has an incorrect number of parameters (expected 2, found " + callback.Arity() + ")");
						}

						foreach(Pair kvp in self.container) {
							object result = callback.Call(interpreter, token, new List() { kvp.Key, kvp.Value });
							resultsContainer[kvp.Key] = result;
						}

						return new DictionaryInstance(resultsContainer);
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class Reduce : ICallable {
					DictionaryInstance self = null;

					public Reduce(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 2;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						object accumulator = arguments[0];
						ScriptFunction callback = (ScriptFunction)arguments[1];

						if (callback.Arity() != 3) {
							throw new ErrorHandler.RuntimeError(token, "Callback has an incorrect number of parameters (expected 3, found " + callback.Arity() + ")");
						}

						foreach(Pair kvp in self.container) {
							accumulator = callback.Call(interpreter, token, new List() { accumulator, kvp.Key, kvp.Value });
						}

						return accumulator;
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class Concat : ICallable {
					DictionaryInstance self = null;

					public Concat(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						if (!(arguments[0] is DictionaryInstance)) {
							throw new ErrorHandler.RuntimeError(token, "Expected a Dictionary instance");
						}

						DictionaryInstance rhs = (DictionaryInstance)arguments[0];

						Dict resultsDict = new Dict();

						foreach(Pair kvp in self.container) {
							resultsDict[kvp.Key] = kvp.Value;
						}

						foreach(Pair kvp in rhs.container) {
							if (!resultsDict.ContainsKey(kvp.Key)) {
								resultsDict[kvp.Key] = kvp.Value;
							}
						}

						return new DictionaryInstance(resultsDict);
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class Clear : ICallable {
					DictionaryInstance self = null;

					public Clear(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 0;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						self.container.Clear();
						return null;
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class EqualsCallable : ICallable {
					DictionaryInstance self = null;

					public EqualsCallable(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 1;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						if (!(arguments[0] is DictionaryInstance)) {
							throw new ErrorHandler.RuntimeError(token, "Expected a Dictionary instance");
						}

						DictionaryInstance rhs = (DictionaryInstance)arguments[0];

						if (self.container.Count != rhs.container.Count) {
							return false;
						}

						foreach(Pair kvp in self.container) { //NOTE: I'm a little worried about key comparison
							if (!rhs.container.ContainsKey(kvp.Key)) {
								return false;
							}

							if (!interpreter.CheckIsEqual(kvp.Value, rhs.container[kvp.Key])) {
								return false;
							}
						}

						return true;
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public class ToStringCallable : ICallable {
					DictionaryInstance self = null;

					public ToStringCallable(DictionaryInstance self) {
						this.self = self;
					}

					public int Arity() {
						return 0;
					}

					public object Call(Interpreter interpreter, Token token, List arguments) {
						return self.ToString();
					}

					public override string ToString() { return "<Dictionary function>"; }
				}

				public override string ToString() {
					//prevent circular references
					if (ToStringHelper.passed.ContainsKey(this)) {
						return "<circular reference>";
					}
					ToStringHelper.passed[this] = 0;

					//build the result
					string result = "";
					foreach(Pair kvp in container) {
						result += kvp.Key.ToString() + ":";
						result += kvp.Value.ToString() + ",";
					}

					//trim the last ',' character
					if (result.Length > 0 && result[result.Length - 1] == ',') {
						result = result.Substring(0, result.Length - 1);
					}

					//cleanup and return
					ToStringHelper.passed.Remove(this);
					return "{" + result + "}";
				}
			}

			public override string ToString() { return "<plugin>"; }
		}
	}
}