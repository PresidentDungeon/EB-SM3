using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.Entities.Security
{
    public class UpdatePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
