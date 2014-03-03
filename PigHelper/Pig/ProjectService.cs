using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PigHelper.Pig
{
    public class ProjectService
    {
        public static List<ProjectInfo> Projects = new List<ProjectInfo>();

        public static void Add(ProjectInfo pro)
        {
            if (Projects.Where(s => s.URL == pro.URL).FirstOrDefault() != null)
                return;
            else
                Projects.Add(pro);

        }

        public static void AddRange(List<ProjectInfo> pros)
        {
            for (int i = pros .Count -1; i >=0; i--)
            {
                var pro=Projects.Where(s => s.URL == pros[i].URL).FirstOrDefault();
                if (pro == null )
                {
                    Projects.Add(pros[i]);
                    continue;
                }
                else 
                {
                    pros.Remove(pros[i]);
                }
            }
        }

        public static List<ProjectInfo> GetUnLookedProjects()
        {
            return Projects.Where(s => s.Looked == false).ToList();
        }
    }
}
