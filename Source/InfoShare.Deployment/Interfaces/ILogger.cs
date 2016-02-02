﻿using System;

namespace InfoShare.Deployment.Interfaces
{
    public interface ILogger
    {
        void WriteVerbose(string message);

        void WriteProgress(string message, string statusDescription);

        void WriteDetail(string message);

        void WriteDebug(string message);

        void WriteWarning(string message);

        void WriteError(Exception ex);
    }
}