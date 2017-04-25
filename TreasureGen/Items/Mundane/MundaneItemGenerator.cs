﻿using System.Collections.Generic;

namespace TreasureGen.Items.Mundane
{
    public interface MundaneItemGenerator
    {
        Item Generate();
        Item GenerateFrom(Item template, bool allowRandomDecoration = false);
        Item GenerateFrom(IEnumerable<string> subset);
    }
}