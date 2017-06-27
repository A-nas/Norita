using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GED.Handlers
{
    public interface IActe
    {
        
        string genJSON();
        void sendProd(); // je teste avec des fichiers dans mon bureau dans un premier temmps 
    }
}
