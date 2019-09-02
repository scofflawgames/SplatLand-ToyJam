# Toy

This is the meta-library, holding information about the Toy language itself. Use this for determining version information.

## version

This string is equal to "major.minor.patch".

## major

This number is the language's major version number.

## minor

This number is the language's minor version number.

## patch

This number is the language's patch version number.

## VersionGreater(x, y, z)

This is a helper function - it returns true of the current version is greater than "x.y.z", otherwise it returns false.

## VersionEqual(x, y, z)

This is a helper function - it returns true of the current version is equal to "x.y.z", otherwise it returns false.

## VersionLess(x, y, z)

This is a helper function - it returns true of the current version is less than "x.y.z", otherwise it returns false.

