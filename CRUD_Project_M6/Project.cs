using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD_Project_M6
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public decimal Budget { get; set; }
        public bool IsRunning { get; set; }
        public int TeacherId { get; set; }
    }
}
