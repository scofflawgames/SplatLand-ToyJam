# Standard

Standard is exactly that - the standard library for Toy. When imported without an alias, the following functions are added to the global scope. When an alias is used, they are bundled into the alias as normal.

## Clock()

Returns a number representing unix time, including milliseconds.

## Random()

Returns a pseudo-random number between 0 (inclusive) and 1 (exclusive). This relies on the underlying pseudo-random number generator implementation, and uses a seed based on unix time in milliseconds by default.

## RandomSeed(seed)

This sets the seed to be used by the Random() function to equal the first argument. It only accepts a number as an argument, and ignores any decimals.

## ToNumber(x)

This function requires either a number, a string or a boolean for "x", anything else is an error. This function converts a string to a number, or converts true to 1 and false to 0. Passing it a number does nothing.

## ToString(x)

This function converts "x" to a string. Booleans become either "true" of "false" depending on their truthiness, while null becomes "null".

## ToBoolean(x)

This function returns the truthiness of "x". The only exception is a string with the exact value "false", which returns false.

## GetType(x)

This function returns a string representation of the type of "x". Possible return values are:

* "null"
* "number"
* "string"
* "boolean"
* "function"
* "alias"
* "plugin"
* "instance"

## IsSame(x, y)

This function returns true of "x" and "y" are the same instance of an object, otherwise it returns false. This is more than just an equality test.

