using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DebugHelper
{
    using SocketCommon;

    public class DebugViewModel
    {
        public DebugViewModel()
        {
            DataResponce result = DebugModel.Instance.GetAll();
            if (result != null)
            {
                LoadedTypes = ((IEnumerable<string>)result.Data).OrderBy(x => x);
            }
        }

        public IEnumerable<object> LoadedTypes { get; set; }
    }
}
