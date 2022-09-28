using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD_Project_M6
{
    public interface ICrossDataFormLoadSync
    {
        void ReloadData(List<Teacher> Teachers);
        void UpdateTeacher(Teacher Teachers);
        void RemoveTeacher(int id);
        void ReloadProject(List<Project> project);
        void UpdateProject(Project p);
        void RemoveProject(int id);
    }
}
