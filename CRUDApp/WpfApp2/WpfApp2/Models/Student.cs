using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models { }
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string GroupNumber { get; set; }

    public List<Grade> Grades { get; set; } = new List<Grade>();
}

