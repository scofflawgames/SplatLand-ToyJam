namespace Toy {
	public class Token {
		//members
		public readonly TokenType type;
		public readonly string lexeme;
		public readonly object literal;
		public readonly int line;

		//methods
		public Token(TokenType t, string lex, object lit, int ln) {
			type = t;
			lexeme = lex;
			literal = lit;
			line = ln;
		}

		public override string ToString() {
			return $"{type} {lexeme} {literal}";
		}
	}
}