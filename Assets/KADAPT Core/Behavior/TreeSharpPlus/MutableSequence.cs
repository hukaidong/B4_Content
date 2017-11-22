﻿#region License

// A simplistic Behavior Tree implementation in C#
// Copyright (C) 2010-2011 ApocDev apocdev@gmail.com
// 
// This file is part of TreeSharp
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System.Collections.Generic;

namespace TreeSharpPlus
{
    /// <summary>
    ///   The base sequence class. This will execute each branch of logic, in order.
    ///   If all branches succeed, this composite will return a successful run status.
    ///   If any branch fails, this composite will return a failed run status.
    /// </summary>
    public class MutableSequence : NodeGroup
    {
        bool successable;

        public MutableSequence(params Node[] children)
            : base(children)
        {
            successable = false;
        }
        public MutableSequence(bool successable, params Node[] children)
        : base(children)
        {
            this.successable = successable;
        }
        public override IEnumerable<RunStatus> Execute()
        {
            while (true)
            {
                if (Length() > 0)
                {
                    Node node = Dequeue();
                    Selection = node;
                    node.Start();

                    RunStatus result;
                    while ((result = this.TickNode(node)) == RunStatus.Running)
                        yield return result;

                    node.Stop();

                    Selection.ClearLastStatus();
                    Selection = null;

                    if (result == RunStatus.Failure)
                    {
                        yield return RunStatus.Failure;
                        yield break;
                    }
                    yield return RunStatus.Running;
                }
                else if (successable)
                {
                    yield return RunStatus.Success;
                    yield break;
                }

                yield return RunStatus.Running;
            }
        }
    }
}