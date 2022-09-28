using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD_Project_M6
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string Name { get; set; }
        public DateTime JoinDate { get; set; }
        public decimal Salary { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool IsWorking { get; set; }
        public string Picture { get; set; }
    }
}
