using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models { }
public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Weight { get; set; }

    public List<Grade> Grades { get; set; } = new List<Grade>();
}

