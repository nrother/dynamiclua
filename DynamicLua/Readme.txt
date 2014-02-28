DynamicLua 1.1 beta
===================
developed by Niklas Rother (niklas-rother.de)

DynamicLua is a wrapper for NLua heavily using the .NET 4 "dynamic" Feature. (NLua is a Lua<->C# Bridge)
It makes it easier and more idiomatic to use Lua in C# Code.
DynamicLua is written in C# and includes unit tests for nearly everything. Most of the tests are converted from the LuaInterface/NLua project.

You might download the binary or source distribution. The binary distribution contains only the three DLL Files and some Texts,
the source distribution the whole source code. The source code is packaged as a Visual Studio 2012 Solution, containing 3 projects:

"DynamicLua" is the main project containing the code for DynamicLua.dll
"DynamicLuaTest" contains the Unittests for DynamicLua, partly copied from the LuaInterface/NLua project
"LuaTest" is a console app just containing some random tests for development. You can ignore it

Documentation
-------------
Please refer to the official documentation on the Codeplex site: http://dynamiclua.codeplex.com/documentation

Bugs/Feedback
-------------
You might report bugs on the Codeplex site in the Issue Tracker: http://dynamiclua.codeplex.com/workitem/list/basic
You can also drop me an email, write to info (ät) niklas-rother (dot) de, in English or German.

Help
----
If you want to help this project, write an email to the adress above, any help is appreciated!

Changes
-------
Version 1.1 beta
- Switched from LuaInterface to fork NLua
- Updated Lua to version 5.2.2

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