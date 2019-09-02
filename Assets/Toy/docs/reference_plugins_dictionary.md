# Dictionary

Instances of the Dictionary plugin are collections of key-value pairs.

## Usage

```
import "Dictionary";

var dictionary["key"] = "value";

dictionary.Insert("foo", "bar");
dictionary.Delete("key");

print dictionary.ContainsKey("foo"); //true

print Dictionary.IsDictionary(dictionary); //true
```

## IsDictionary(x)

This returns true if "x" is an instance of Dictionary, otherwise it returns false.

# Instances

Instances are the result of calling `Dictionary()`.

## Indexing

Dictionaries can be indexed using traditional bracket notation. Existing elements can be accessed or overwritten, or new ones inserted if they don't already exist this way:

```
dictionary["foo"] = "bar";
print dictionary["foo"];
```

## Insert(k, v)

This function inserts a new key-value pair "k:v", or the program fails if that key already exists.

## Delete(k)

This function deletes the key-value pair with the key "k". Nothing happens if the key-value pair doesn't exist.

## Length()

This function returns the number of key-value pairs in a dictionary.

## ContainsKey(k)

This function returns true if the dictionary contains a key "k", otherwise it returns false.

## ContainsValue(V)

This function returns true if the dictionary contains a value "v", otherwise it returns false.

## Every(cb(k, v))

This function calls "cb" once for every pair in the dictionary, with that key and value passed in as "k" and "v", respectfully. If any call to "cb" returns false, then the function exits early and returns false. Otherwise the function returns true.

## Any(cb(k, v))

This function calls "cb" once for every pair in the dictionary, with that key and value passed in as "k" and "v", respectfully. If any call to "cb" returns false, then the function exits early and returns true. Otherwise the function returns false.

## Filter(cb(k, v))

This function calls "cb" once for every pair in the dictionary, with that key and value passed in as "k" and "v", respectfully. This returns a new dictionary instance that contains every key-value pair for which the call to "cb" returned true.

## ForEach(cb(k, v))

This function calls "cb" once for every pair in the dictionary, with that key and value passed in as "k" and "v", respectfully. 

## Map(cb(k, v))

This function calls "cb" once for every pair in the dictionary, with that key and value passed in as "k" and "v", respectfully. It returns a new dictionary instance with keys copied from the current dictionary, and values replaced with the results of the calls to "cb".

## Recuce(default, cb(acc, k, v))

This function calls "cb" once for every pair in the dictionary, with that key and value passed in as "k" and "v", respectfully, and the value of the previous call passed in as "acc". For the first call to "cb", "default" is used for "acc".

## Concat(x)

This function requires a dictionary instance as "x". This function returns a new dictionary instance which contains the contents of the current dictionary combined with the contents of "x". In the envent of a key clash, the key-value pair in the current dictionary is included, and the key-value pair from "x" is discarded.

## Clear()

This function removes the contents of the dictionary instance, leave an empty dictionary.

## Equals(x)

This function requires a dictionary instance as "x". It returns false if the current dictionary and "x" have a different number of key-value pairs. Then, it iterates over each key-value pair in the current dictionary and checks "x" for a matching key, returning false if a key is missing. Then, before moving onto the next key-value pair, it compares the values with that key in each dictionary and returns false if a mismatch is found.

Finally, this function returns true.

## ToString()

This function returns a string representation of the dictionary. Each key-value pair is converted to a string with a colon between the two, and each pair is separated by commas. Finally, the whole string is surrounded by braces.

Nesting a dictionary within it's own data structure will cause the inner reference to be printed as "\<circular reference\>".

