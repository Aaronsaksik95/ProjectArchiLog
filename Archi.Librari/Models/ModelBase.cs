using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archi.Librari.Models
{
    public abstract class ModelBase
    {
        public int ID { get; set; }
        public bool Active { get; set; }
    }
}
