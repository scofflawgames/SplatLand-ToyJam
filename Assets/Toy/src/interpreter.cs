using System;
using System.Collections.Generic;

#if TOY_UNITY
using UnityEngine;
#endif

using static Toy.TokenType;

namespace Toy {
	public class Interpreter : ExprVisitor<object>, StmtVisitor<object> {
		//members
		public Environment globals;
		public Environment environment;
		static Dictionary<Expr, int> locals = new Dictionary<Expr, int>();

		public Interpreter() {
			globals = new Environment();
			environment = globals;
		}

		public Interpreter(Environment env) {
			//use an existing environment
			globals = env;
			environment = env;
		}

		//access
		public int Interpret(List<Stmt> stmtList) {
			foreach (Stmt stmt in stmtList) {
				Token signal = (Token)Execute(stmt);

				if (signal != null) {
					if (signal.type == BREAK) {
						throw new ErrorHandler.RuntimeError(signal, "Unexpected break statement outside of a loop");
					} else if (signal.type == CONTINUE) {
						throw new ErrorHandler.RuntimeError(signal, "Unexpected continue statement outside of a loop");
					}
				}
			}

			return 0;
		}

		//visitor pattern
		public object Visit(Print stmt) {
			object value = Evaluate(stmt.expression);

			//unescape a string
			if (value is string) {
				ConsoleOutput.Log(System.Text.RegularExpressions.Regex.Unescape((string)value));
			} else if (value is bool) {
				ConsoleOutput.Log(value.ToString().ToLower());
			} else if (value == null) {
				ConsoleOutput.Log("null");
			} else {
				ConsoleOutput.Log(value);
			}

			return null;
		}

		public object Visit(Import stmt) {
			string libname = (string)((Literal)(stmt.library)).value;

			//load another file instead
			if (libname.Length > 4 && libname.Substring(libname.Length - 4) == ".toy") {

#if TOY_UNITY
				libname = Application.streamingAssetsPath + "/" + libname;
#endif

				Environment env = Runner.RunFile(libname);

				//merge the sub-environment into this one, possibly under an alias
				environment.Define(stmt.alias != null ? ((Variable)stmt.alias).name.lexeme : null, env, true);
				return null;
			}

			//try a bunch of different names
			Type type = null;

			if (type == null) {
				type = Type.GetType("Toy.Plugin." + libname);
			}

			if (type == null) { //user plugins take precedence over built-in libraries
				type = Type.GetType("Toy.Library." + libname);
			}

			//still not found
			if (type == null) {
				throw new ErrorHandler.RuntimeError(stmt.keyword, "Unexpected library name");
			}

			//create the library and cast it to the correct type
			dynamic library = ((IPlugin)Convert.ChangeType(Activator.CreateInstance(type), type)).Singleton;

			//initialize the library
			library.Initialize(environment, stmt.alias != null ? ((Variable)stmt.alias).name.lexeme : null);

			return null;
		}

		public object Visit(If stmt) {
			if (CheckIsTruthy( Evaluate(stmt.cond) )) {
				return Execute(stmt.thenBranch);
			} else if (stmt.elseBranch != null) {
				return Execute(stmt.elseBranch);
			}

			return null;
		}

		public object Visit(Do stmt) {
			do {
				Token signal = (Token)Execute(stmt.body);

				if (signal != null && signal.type == BREAK) {
					break;
				}
			} while (CheckIsTruthy(Evaluate(stmt.cond)));

			return null;
		}

		public object Visit(While stmt) {
			while (CheckIsTruthy(Evaluate(stmt.cond))) {
				Token signal = (Token)Execute(stmt.body);

				if (signal != null && signal.type == BREAK) {
					break;
				}
			}

			return null;
		}

		public object Visit(For stmt) {
			Environment previous = environment;
			environment = new Environment(previous);

			try {
				Execute(stmt.initializer);

				while (CheckIsTruthy(Evaluate(stmt.cond))) {
					Token signal = (Token)Execute(stmt.body);

					if (signal != null && signal.type == BREAK) {
						break;
					}

					Evaluate(stmt.increment);
				}
			} finally {
				environment = previous;
			}

			return null;
		}

		public object Visit(Break stmt) {
			return stmt.keyword;
		}

		public object Visit(Continue stmt) {
			return stmt.keyword;
		}

		public object Visit(Return stmt) {
			object value = null;
			if (stmt.value != null) {
				value = Evaluate(stmt.value);
			}

			throw new ReturnException(value);
		}

		public object Visit(Assert stmt) {
			if (!CheckIsTruthy(Evaluate(stmt.cond))) {
				string msg = "<no message>";

				if (stmt.message != null) {
					msg = (string)Evaluate(stmt.message);
				}

				throw new ErrorHandler.AssertError(stmt.keyword, msg);
			}

			return null;
		}

		public object Visit(Block stmt) {
			return ExecuteBlock(stmt, new Environment(environment));
		}

		public object Visit(Var stmt) {
			object value = null;
			if (stmt.initializer != null) {
				value = Evaluate(stmt.initializer);
			}
			environment.Define(stmt.name, value, false);
			return null;
		}

		public object Visit(Const stmt) {
			environment.Define(stmt.name, Evaluate(stmt.initializer), true);
			return null;
		}

		public object Visit(Pass stmt) {
			//DO NOTHING
			return null;
		}

		public object Visit(Expression stmt) {
			Evaluate(stmt.expression);
			return null;
		}

		public object Visit(Variable expr) {
			return LookupVariable(expr);
		}

		public object Visit(Assign expr) {
			if (expr.left is Variable) {
				return VisitAssignToVariable(expr);
			}

			if (expr.left is Index) {
				return VisitAssignToIndex(expr);
			}

			if (expr.left is Property) {
				return VisitAssignToIndex(expr);
			}

			throw new ErrorHandler.RuntimeError(expr.oper, "Unknown assignment target (slipped by the parser)");
		}

		public object Visit(Increment expr) {
			object value = LookupVariable(expr.variable);

			if (!(value is double)) {
				throw new ErrorHandler.RuntimeError(expr.oper, "Unexpected type (expected a number, received " + (value != null ? value.ToString() : "null") + ")");
			}

			object originalValue = value;

			if (expr.oper.type == PLUS_PLUS) {
				value = (double)value + 1;
			} else if (expr.oper.type == MINUS_MINUS) {
				value = (double)value - 1;
			} else {
				throw new ErrorHandler.RuntimeError(expr.oper, "Bad increment implementation");
			}

			AssignVariable(expr.variable, value);

			if (expr.prefix) {
				return value;
			} else {
				return originalValue;
			}
		}

		public object Visit(Literal expr) {
			return expr.value;
		}

		public object Visit(Logical expr) {
			object left = Evaluate(expr.left);

			if (expr.oper.type == OR_OR) {
				if (CheckIsTruthy(left)) return left;
			} else {
				if (!CheckIsTruthy(left)) return left;
			}

			return Evaluate(expr.right);
		}

		public object Visit(Unary expr) {
			object right = Evaluate(expr.right);

			switch(expr.oper.type) {
				case MINUS:
					return -(double)right;

				case BANG:
					return !CheckIsTruthy(right);
			}

			return null;
		}

		public object Visit(Binary expr) {
			object left = Evaluate(expr.left);
			object right = Evaluate(expr.right);

			switch(expr.oper.type) {
				case PLUS:
					if (left is double && right is double) {
						return (double)left + (double)right;
					} else if (left is string && right is string) {
						return (string)left + (string)right;
					} else {
						throw new ErrorHandler.RuntimeError(expr.oper, "Unexpected operand type (expected both numbers or both strings, got " + (left != null ? left : "null") + " and " + (right != null ? right : "null") + ")");
					}

				case MINUS:
					CheckNumberOperands(expr.oper, left, right);
					return (double)left - (double)right;

				case STAR:
					CheckNumberOperands(expr.oper, left, right);
					return (double)left * (double)right;

				case SLASH:
					CheckNumberOperands(expr.oper, left, right);
					if ((double)right == 0) {
						throw new ErrorHandler.RuntimeError(expr.oper, "Can't divide by 0");
					}
					return (double)left / (double)right;

				case MODULO:
					CheckNumberOperands(expr.oper, left, right);
					if ((double)right == 0) {
						throw new ErrorHandler.RuntimeError(expr.oper, "Can't modulo by 0");
					}
					return (double)left % (double)right;

				case GREATER:
					CheckNumberOperands(expr.oper, left, right);
					return (double)left > (double)right;

				case GREATER_EQUAL:
					CheckNumberOperands(expr.oper, left, right);
					return (double)left >= (double)right;

				case LESS:
					CheckNumberOperands(expr.oper, left, right);
					return (double)left < (double)right;

				case LESS_EQUAL:
					CheckNumberOperands(expr.oper, left, right);
					return (double)left <= (double)right;

				case BANG_EQUAL:
					return !CheckIsEqual(left, right);

				case EQUAL_EQUAL:
					return CheckIsEqual(left, right);
			}

			return null;
		}

		public object Visit(Call expr) {
			object callee = Evaluate(expr.callee);

			List<object> arguments = new List<object>();
			foreach (Expr argument in expr.arguments) {
				arguments.Add(Evaluate(argument));
			}

			if (!(callee is ICallable)) {
				throw new ErrorHandler.RuntimeError(expr.paren, "Can't call this datatype: " + ( callee == null ? "null" : callee.ToString() ));
			}

			ICallable called = (ICallable)callee;

			if (arguments.Count != called.Arity()) {
				throw new ErrorHandler.RuntimeError(expr.paren, "Expected " + called.Arity() + " arguments but received " + arguments.Count);
			}

			return called.Call(this, expr.paren, arguments);
		}

		public object Visit(Index expr) {
			object callee = Evaluate(expr.callee);

			if (callee is ICollection) {
				ICollection called = (ICollection)callee;

				object first = Evaluate(expr.first);
				object second = expr.second != null ? Evaluate(expr.second) : null;
				object third = expr.third != null ? Evaluate(expr.third) : null;

				return called.Access(this, expr.bracket, first, second, third);
			}

			if (callee is string) {
				string str = (string)callee;

				object first = Evaluate(expr.first);
				object second = expr.second != null ? Evaluate(expr.second) : null;
				object third = expr.third != null ? Evaluate(expr.third) : null;

				if (expr.callee is Literal) {
					return Toy.Library.String.SliceNotationLiteral(expr.callee, expr.bracket, first, second, third);
				}

				if (expr.callee is Variable) {
					return Toy.Library.String.SliceNotationVariable(expr.callee, expr.bracket, this, first, second, third);
				}
			}

			throw new ErrorHandler.RuntimeError(expr.bracket, "Expected indexable type (found " + (callee == null ? "null" : callee.ToString()) + ")");
		}

		public object Visit(Pipe expr) {
			object callee = Evaluate(expr.callee);

			//iterate along manually rather than using the tree
			for (object iter = (Pipe)expr; iter is Pipe; iter = ((Pipe)iter).following) {
				//get the following callable
				ICallable callable;

				object following = ((Pipe)iter).following;

				if (following is Pipe) {
					callable = (ICallable)Evaluate( ((Pipe)following).callee );
				} else {
					callable = (ICallable)Evaluate( (Expr)following ); //variable
				}

				callee = callable.Call(this, ((Pipe)iter).pipe, new List<object>() { callee });
			}

			return callee;
		}

		public object Visit(Function expr) {
			return new ScriptFunction(expr, environment);
		}

		public object Visit(Property expr) {
			//built-in properties for strings and numbers here

			object result = Evaluate(expr.expression);

			if (result is IBundle) {
				return ((IBundle)result).Property(this, expr.name, expr.name.lexeme);
			}

			if (result is string) {
				return Toy.Library.String.LiteralProperty((string)result, expr.name, expr.name.lexeme);
			}

			if (result is Variable) {
				return Toy.Library.String.VariableProperty((Variable)result, this, expr.name, expr.name.lexeme);
			}

			throw new ErrorHandler.RuntimeError(expr.name, "Expected type with properties (found " + (result == null ? "null" : result.ToString()) + ")");
		}

		public object Visit(Grouping expr) {
			return Evaluate(expr.expression);
		}

		public object Visit(Ternary expr) {
			object cond = Evaluate(expr.cond);
			if (CheckIsTruthy(cond)) {
				return Evaluate(expr.left);
			} else {
				return Evaluate(expr.right);
			}
		}

		//helpers
		public object VisitAssignToIndex(Assign expr) {
			dynamic assignableIndex = expr.left.Accept(this);
			object right = Evaluate(expr.right);

			switch(expr.oper.type) {
				case EQUAL:
					assignableIndex.Value = right;
					break;

				case PLUS_EQUAL:
					if (assignableIndex.Value is double && right is double) {
						assignableIndex.Value = (double)assignableIndex.Value + (double)right;
					} else if (assignableIndex.Value is string && right is string) {
						assignableIndex.Value = (string)assignableIndex.Value + (string)right;
					} else {
						throw new ErrorHandler.RuntimeError(expr.oper, "Unexpected operand type (expected both numbers or both strings, got " + (assignableIndex.Value != null ? assignableIndex.Value : "null") + " and " + (right != null ? right : "null") + ")");
					}

					break;

				case MINUS_EQUAL:
					CheckNumberOperands(expr.oper, assignableIndex.Value, right);
					assignableIndex.Value -= (double)right;
					break;

				case STAR_EQUAL:
					CheckNumberOperands(expr.oper, assignableIndex.Value, right);
					assignableIndex.Value *= (double)right;
					break;

				case SLASH_EQUAL:
					CheckNumberOperands(expr.oper, assignableIndex.Value, right);

					if ((double)right == 0) {
						throw new ErrorHandler.RuntimeError(expr.oper, "Can't divide by 0");
					}

					assignableIndex.Value /= (double)right;
					break;

				case MODULO_EQUAL:
					CheckNumberOperands(expr.oper, assignableIndex.Value, right);

					if ((double)right == 0) {
						throw new ErrorHandler.RuntimeError(expr.oper, "Can't modulo by 0");
					}

					assignableIndex.Value %= (double)right;
					break;

				default:
					throw new ErrorHandler.RuntimeError(expr.oper, "Unknown operator");
			}

			return assignableIndex.Value;
		}

		public object VisitAssignToVariable(Assign expr) {
			object left = LookupVariable(expr.left);
			object right = Evaluate(expr.right);

			switch(expr.oper.type) {
				case EQUAL:
					return AssignVariable(expr.left, right);

				case PLUS_EQUAL:
					if (left is double && right is double) {
						return AssignVariable(expr.left, (double)left + (double)right);
					} else if (left is string && right is string) {
						return AssignVariable(expr.left, (string)left + (string)right);
					} else {
						throw new ErrorHandler.RuntimeError(expr.oper, "Unexpected operand type (expected both numbers or both strings, got " + (left != null ? left : "null") + " and " + (right != null ? right : "null") + ")");
					}

				case MINUS_EQUAL:
					CheckNumberOperands(expr.oper, left, right);
					return AssignVariable(expr.left, (double)left - (double)right);

				case STAR_EQUAL:
					CheckNumberOperands(expr.oper, left, right);
					return AssignVariable(expr.left, (double)left * (double)right);

				case SLASH_EQUAL:
					CheckNumberOperands(expr.oper, left, right);

					if ((double)right == 0) {
						throw new ErrorHandler.RuntimeError(expr.oper, "Can't divide by 0");
					}

					return AssignVariable(expr.left, (double)left / (double)right);

				case MODULO_EQUAL:
					CheckNumberOperands(expr.oper, left, right);

					if ((double)right == 0) {
						throw new ErrorHandler.RuntimeError(expr.oper, "Can't modulo by 0");
					}

					return AssignVariable(expr.left, (double)left % (double)right);

				default:
					throw new ErrorHandler.RuntimeError(expr.oper, "Unknown operator");
			}
		}

		public object Execute(Stmt stmt) {
			return stmt.Accept(this);
		}

		public object ExecuteBlock(Block stmt, Environment env, bool killOnBreak = false) {
			Environment previous = environment;
			environment = env;
			Token signal = null;

			try {
				foreach (Stmt s in stmt.statements) {
					signal = (Token)Execute(s);

					if (signal != null) {
						if (signal.type == BREAK || signal.type == CONTINUE) {
							if (killOnBreak) {
								throw new ErrorHandler.RuntimeError(signal, "Unexpected break or continue statement");
							}
							break;
						}
					}
				}
			} finally {
				environment = previous;
			}

			return signal;
		}

		public object Evaluate(Expr expr) {
			object res = expr.Accept(this);

			while (res is AssignableIndex) {
				res = ((AssignableIndex)res).Value;
			}

			return res;
		}

		public void Resolve(Expr expr, int depth) {
			locals[expr] = depth;
		}

		public object LookupVariable(Expr expr) {
			if (locals.ContainsKey(expr)) {
				return environment.GetAt(locals[expr], ((Variable)expr).name);
			} else {
				return globals.Get(((Variable)expr).name);
			}
		}

		public object AssignVariable(Expr expr, object value) {
			if (locals.ContainsKey(expr)) {
				return environment.SetAt(locals[expr], ((Variable)expr).name, value);
			} else {
				return globals.Set(((Variable)expr).name, value);
			}
		}

		public bool CheckIsTruthy(object obj) {
			if (obj == null) return false;
			if (obj is bool) return (bool)obj;
			if (obj is double) return (double)obj != 0;
			return true;
		}

		public bool CheckIsEqual(object left, object right) {
			if (left == null && right == null) {
				return true;
			}

			if (left == null) {
				return false;
			}

			//all numbers but 0 are truthy
			if ((left is double && right is bool) || (left is bool && right is double)) {
				return CheckIsTruthy(left) == CheckIsTruthy(right);
			}

			return left.Equals(right);
		}

		void CheckNumberOperands(Token oper, params object[] operands) {
			foreach(object obj in operands) {
				if (!(obj is double)) {
					throw new ErrorHandler.RuntimeError(oper, "Unexpected operand type (expected a number, recieved " + (obj == null ? "null" : obj.GetType().ToString()) + ")");
				}
			}
		}
	}
}
