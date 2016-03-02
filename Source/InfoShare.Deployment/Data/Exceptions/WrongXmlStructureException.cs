﻿using System;

namespace InfoShare.Deployment.Data.Exceptions
{
    public class WrongXmlStructureException : Exception
    {
        public WrongXmlStructureException()
        { }

        public WrongXmlStructureException(string message)
            : base(message)
        { }
    }
}