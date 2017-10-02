using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SpeciesToCount
{
    public string Name { get; set; }
    public int Id { get; set; }
    public List<int> CountsOverTime { get; set; }

    public SpeciesToCount()
    {
        CountsOverTime = new List<int>();
    }
}