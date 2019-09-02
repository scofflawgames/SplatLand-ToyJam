using System;

namespace Toy {
	class CLI {
		//static methods
		static void Main(string[] args) {
			//get the app's name
			string appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

			switch(args.Length) {
				case 0:
					RunPrompt();
					break;

				case 1:
					if (Runner.RunFile(args[0]) == null) {
						System.Environment.Exit(-1);
					}
					break;

				case 2:
					if (args[0] == "run") {
						if (Runner.Run(args[1]) == null) {
							System.Environment.Exit(-1);
						}
					}
					break;

				default:
					Console.Write($"Usage: {appName} [<file name> | run \"<toy code>\"]");
					break;
			}
		}

		static void RunPrompt() {
			string input;

			Environment env = new Environment();

			while(true) {
				Console.Write(">");
				input = Console.ReadLine();
				Runner.Run(env, input);

				//ignore errors in prompt mode
				if (ErrorHandler.HadError) {
					ErrorHandler.ResetError();
				}
			}
		}
	}
}