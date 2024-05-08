using System;
using static BackendPw.CustomObject.PwObject;

namespace BackendPw.Validation
{
    public static class Validation
    {
        public static PwException Create_Exception(int Id, string Message)
        {
            PwException pwException = new PwException(Id, Message);

            return pwException;

        }
    }



    public class DiyProjectAddException : Exception
    {
        public DiyProjectAddException()
        {
        }
        public DiyProjectAddException(string message)
            : base(message)
        {
        }
    }

    public class DiyProjectMaterialAddException : Exception
    {
        public DiyProjectMaterialAddException()
        {
        }
        public DiyProjectMaterialAddException(string message)
            : base(message)
        {
        }
    }

}