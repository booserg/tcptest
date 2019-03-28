using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.Common
{
    public interface IRequestProcessor
    {
        //string ProcessRequest(string request);

        byte[] ProcessRawRequest(string request);
    }
}
