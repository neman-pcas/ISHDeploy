﻿using System.Xml.Linq;

namespace InfoShare.Deployment.Data.Services
{
    public interface IFileManager
    {
        void Backup(string originalFilePath, string backupFilePath);
        void RestoreOriginal(string backupFilePath, string originalFilePath);
        XDocument Load(string filePath);
        void Save(string filePath, XDocument doc);
    }
}