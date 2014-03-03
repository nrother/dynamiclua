# DynamicLua
![DynamicLua Logo](https://github.com/nrother/dynamiclua/blob/master/dynamiclua_logo.png)

**DynamicLua** is a wrapper for [NLua](https://github.com/NLua/NLua) heavily using the .NET 4 `dynamic` Feature. (NLua is a Lua/C# Bridge)
It makes it easier and more idiomatic to use Lua in C# Code. DynamicLua is written in C# and includes unit tests for nearly everything. Most of the tests are converted from the LuaInterface/NLua project.

## Example/Features

* Easy access to Lua
* Use Lua-Metatables in C# ([see documentation](https://github.com/nrother/dynamiclua/wiki/MetaTablesInCSharp))
* Nearly complete unit-tested
* _Works as you expect it to_
* OpenSource under Apache License 2.0
* It's free...

**Code Example:**

```csharp
// Start a new Lua interpreter
dynamic lua = new DynamicLua.DynamicLua(); //Namespace and class name are the same!
// Run Lua chunks
lua("num = 2"); //no DoString()!
lua("str = ’a string’");
// Read global variables ’num’ and ’str’
double num = lua.num; //No explicit casting, no index operator!
string str = lua.str;
// Write to global variable ’str’
lua.str = "another string"; //No index operator
//Increase a global value
lua.num += 10; //A LOT cleaner
```

There a more features witch makes it much easier to use Lua in C#. All features are explained on the [Features](https://github.com/nrother/dynamiclua/wiki/Features) site.

## Project Status
A lot of the work is done. Some of the implemented features a shown in the Examples above.
There is still some polishing to be done, and some problems to be solved. Detailed information on every feature's status is available on the [Features](https://github.com/nrother/dynamiclua/wiki/Features) site.

## Contributing
This project is currently developed only by [me](http://niklas-rother.de). I appreciate any help, just have a look on the [issue tracker](https://github.com/nrother/dynamiclua/issues). You can contact me in German or English.

## License
DynamicLua is licensed under the Apache License 2.0. It includes [NLua](https://github.com/NLua/NLua), [KeraLua](https://github.com/NLua/KeraLua) and [Lua](http://lua.org) itself (all included in the binary), these are licensed under the MIT License.

## News
* 06.09.13: There is a [NuGet-Package for DynamicLua](https://www.nuget.org/packages/DynamicLua/)! Thanks for Jim Counts both for the idea and the patch.
* 19.01.12: The german magazin [dotnetpro](http://dotnetpro.de) has an [article](http://www.dotnetpro.de/articles/onlinearticle3990.aspx) about DynamicLua (written by me)
* 13.10.11: Version 1.0beta released!

(Logo based on [Story Bridge by Cyron](http://www.flickr.com/photos/cyron/10813776/) under [CC-BY-2.0](http://creativecommons.org/licenses/by/2.0/deed.de))