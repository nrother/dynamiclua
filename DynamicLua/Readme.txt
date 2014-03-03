DynamicLua 1.1 beta
===================
developed by Niklas Rother (niklas-rother.de)

DynamicLua is a wrapper for NLua heavily using the .NET 4 "dynamic" Feature. (NLua is a Lua/C# Bridge)
It makes it easier and more idiomatic to use Lua in C# Code.
DynamicLua is written in C# and includes unit tests for nearly everything. Most of the tests are converted from the LuaInterface/NLua project.

"DynamicLua" is the main project containing the code for DynamicLua.dll
"DynamicLuaTest" contains the Unittests for DynamicLua, partly copied from the LuaInterface/NLua project
"LuaTest" is a console app just containing some random tests for development. You can ignore it

Documentation
-------------
Please refer to the official documentation on the GitHub site: https://github.com/nrother/dynamiclua/wiki

Bugs/Feedback
-------------
You might report bugs on the Codeplex site in the Issue Tracker: https://github.com/nrother/dynamiclua/issues
You can also drop me an email, write to info (at) niklas-rother (dot) de, in English or German.

Help
----
If you want to help this project, write an email to the adress above, any help is appreciated!

Changes
-------
Version 1.1 beta
- x64 support (!)
- Switched from LuaInterface to fork NLua
- Updated Lua to version 5.2.2
- Made a lot of iternal changes
- Just one DLL, NLua, KeraLua and Lua itself are embedded.

Version 1.0 beta
- Initial release

Licence
-------
Copyright 2011-2014 Niklas Rother

DynamicLua is licensed under the Apache License, Version 2.0.
Lua and NLua are licensed under the MIT Licence.

A full copy of the Licences can be found in the "LICENSE_Apache.txt"
and "LICENSE_MIT.txt" file. Please include the "NOTICE.txt" File,
containing attribution, that comes with this software, in any derivative
work, as required by the Apache license. If the Licence Files got lost you
can optain a copy at the following locations:
Apache License: http://www.apache.org/licenses/LICENSE-2.0
MIT License: http://www.opensource.org/licenses/mit-license.php