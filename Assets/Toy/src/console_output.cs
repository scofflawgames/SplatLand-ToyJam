#if TOY_UNITY
using UnityEngine;
#else
using System;
#endif

namespace Toy {
	public class ConsoleOutput {
		public static void Log(object obj) {
#if TOY_UNITY
		Debug.Log(obj);
#else
		if (obj is string) {
			Console.WriteLine((string)obj);
		} else {
			Console.WriteLine(obj);
		}
#endif
		}
	}
}