using System.Collections.Generic;

namespace Toy {
	public abstract class Expr {
		public abstract R Accept<R>(ExprVisitor<R> visitor);
	}

	public interface ExprVisitor<R> {
		R Visit(Variable Expr);
		R Visit(Assign Expr);
		R Visit(Increment Expr);
		R Visit(Literal Expr);
		R Visit(Logical Expr);
		R Visit(Unary Expr);
		R Visit(Binary Expr);
		R Visit(Call Expr);
		R Visit(Index Expr);
		R Visit(Pipe Expr);
		R Visit(Function Expr);
		R Visit(Property Expr);
		R Visit(Grouping Expr);
		R Visit(Ternary Expr);
	}

	public class Variable : Expr {
		public Variable(Token name) {
			this.name = name;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public Token name;
	}

	public class Assign : Expr {
		public Assign(Expr left, Token oper, Expr right) {
			this.left = left;
			this.oper = oper;
			this.right = right;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public Expr left;
		public Token oper;
		public Expr right;
	}

	public class Increment : Expr {
		public Increment(Token oper, Variable variable, bool prefix) {
			this.oper = oper;
			this.variable = variable;
			this.prefix = prefix;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public Token oper;
		public Variable variable;
		public bool prefix;
	}

	public class Literal : Expr {
		public Literal(object value) {
			this.value = value;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public object value;
	}

	public class Logical : Expr {
		public Logical(Expr left, Token oper, Expr right) {
			this.left = left;
			this.oper = oper;
			this.right = right;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public Expr left;
		public Token oper;
		public Expr right;
	}

	public class Unary : Expr {
		public Unary(Token oper, Expr right) {
			this.oper = oper;
			this.right = right;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public Token oper;
		public Expr right;
	}

	public class Binary : Expr {
		public Binary(Expr left, Token oper, Expr right) {
			this.left = left;
			this.oper = oper;
			this.right = right;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public Expr left;
		public Token oper;
		public Expr right;
	}

	public class Call : Expr {
		public Call(Expr callee, Token paren, List<Expr> arguments) {
			this.callee = callee;
			this.paren = paren;
			this.arguments = arguments;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public Expr callee;
		public Token paren;
		public List<Expr> arguments;
	}

	public class Index : Expr {
		public Index(Expr callee, Expr first, Expr second, Expr third, Token bracket) {
			this.callee = callee;
			this.first = first;
			this.second = second;
			this.third = third;
			this.bracket = bracket;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public Expr callee;
		public Expr first;
		public Expr second;
		public Expr third;
		public Token bracket;
	}

	public class Pipe : Expr {
		public Pipe(Expr callee, Token pipe, Expr following) {
			this.callee = callee;
			this.pipe = pipe;
			this.following = following;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public Expr callee;
		public Token pipe;
		public Expr following;
	}

	public class Function : Expr {
		public Function(List<Expr> parameters, List<Stmt> body) {
			this.parameters = parameters;
			this.body = body;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public List<Expr> parameters;
		public List<Stmt> body;
	}

	public class Property : Expr {
		public Property(Expr expression, Token name) {
			this.expression = expression;
			this.name = name;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public Expr expression;
		public Token name;
	}

	public class Grouping : Expr {
		public Grouping(Expr expression) {
			this.expression = expression;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public Expr expression;
	}

	public class Ternary : Expr {
		public Ternary(Expr cond, Expr left, Expr right) {
			this.cond = cond;
			this.left = left;
			this.right = right;
		}

		public override R Accept<R>(ExprVisitor<R> visitor) {
			return visitor.Visit(this);
		}

		public Expr cond;
		public Expr left;
		public Expr right;
	}

}
