﻿using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Server.Attributes;

namespace NeoServer.Loaders;

public static class ScriptSearch
{
    public static IEnumerable<Type> All => AppDomain.CurrentDomain.GetAssemblies().AsParallel()
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => type.IsDefined(typeof(ExtensionAttribute), false));

    public static Type Get(string name)
    {
        var type = All.FirstOrDefault(x => x.Name.Equals(name));
        return type;
    }
}