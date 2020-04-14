using System;
using System.IO;

namespace TravelExpenses.Common.interfaces
{
    public interface IFilesHelper
    {
        byte[] ReadFully(Stream input);
    }
}
