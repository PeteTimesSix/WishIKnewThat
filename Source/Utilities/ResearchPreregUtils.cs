using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PeteTimesSix.WishIKnewThat.Utilities
{
    public static class ResearchPreregUtils
    {
        public static HashSet<ResearchProjectDef> PreregsDFS(IEnumerable<ResearchProjectDef> preregs, int? maxDepth, int? maxCount, out bool wentOver)
        {
            var stack = new Stack<(int depth, ResearchProjectDef project)>(preregs.Select(p => (1, p)));
            var visited = new HashSet<ResearchProjectDef>(preregs);

            wentOver = false;

            while (stack.Count != 0)
            {
                var (depth, project) = stack.Pop();
                if ((maxDepth.HasValue && maxDepth < depth) || (maxCount.HasValue && maxCount < visited.Count))
                {
                    wentOver = true;
                    break;
                }
                //just to handle projects that have no real preregs but do have hidden preregs...
                var allPreregs = (project.prerequisites ?? Enumerable.Empty<ResearchProjectDef>()).ConcatIfNotNull(project.hiddenPrerequisites);

                if (project.prerequisites != null)
                {
                    foreach (var prereg in allPreregs)
                    {
                        if (!prereg.IsFinished && !visited.Contains(prereg))
                        {
                            visited.Add(prereg);
                            stack.Push((depth + 1, prereg));
                        }
                    }
                }
            }

            return visited;
        }
    }
}
