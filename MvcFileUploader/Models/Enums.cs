using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcFileUploader.Models
{
    public enum ErrorType
    {
        FileAlreadyExists,
        Unknown
    }

    public enum UploadUI
    {
        Bootstrap,
        JQueryUI
    }
}
