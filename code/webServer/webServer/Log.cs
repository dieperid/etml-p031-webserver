/// ETML
/// Auteur : David Dieperink
/// Date : 06.10.2021
/// Description : Projet Reseau (WebServer) -> Class LogInGile contenant les différents types de log à effectué

using System;
using System.IO;

namespace WebServer
{
    /// <summary>
    /// Class LogInFile
    /// </summary>
    class LogInFile
    {
        #region[Déclaration des variables]

        /// <summary>
        /// Variable _writeInFile
        /// </summary>
        private StreamWriter _writeInFile;
        /// <summary>
        /// Variable _fileNamePath
        /// </summary>
        private string _fileNamePath;
        /// <summary>
        /// Variable _dateTimeFormat
        /// </summary>
        private string _dateTimeFormat;

        #endregion

        /// <summary>
        /// Constructeur de la class LogInFile
        /// </summary>
        public LogInFile()
        {
            #region[LogInFile code]
            // Récupération de la date actuelle
            DateTime date = DateTime.Now;

            // Format de date : dd.mm.yyyy
            string dateFormat = date.ToString("d");

            // Format de date : hh.mm.ss
            _dateTimeFormat = date.ToString("T");

            // Envoie du Path ou le fichier de log est écrit
            _fileNamePath = @"C:\Partage\log.txt";

            // Si le fichier n'existe pas
            if (File.Exists(_fileNamePath) == false)
            {
                // Création du fichier
                StreamWriter createFile = new StreamWriter(_fileNamePath);
                createFile.Close();
            }
            #endregion
        }

        /// <summary>
        /// Méthode Running qui écrit dans les logs quand le serveur démarre
        /// </summary>
        /// <param name="ipAddress">Adresse IP du server</param>
        /// <param name="port">Port du serveur</param>
        public void Running(string ipAddress, string port)
        {
            #region[Running code]

            _writeInFile = new StreamWriter(_fileNamePath, append: true);
            _writeInFile.WriteLine("[{0}] [INFO] [Web Server Running {1}:{2}]", _dateTimeFormat, ipAddress, port);
            _writeInFile.Close();

            #endregion

        }

        /// <summary>
        /// Méthode Client qui écrit dans les logs la requête du client
        /// </summary>
        /// <param name="ipAddress">IP du client</param>
        /// <param name="port">Port utilisé par le client</param>
        /// <param name="URL">URL du client</param>
        public void Client(string ipAddress, string port, string URL)
        {
            #region[Client code]

            _writeInFile = new StreamWriter(_fileNamePath, append: true);
            _writeInFile.WriteLine("[{0}] [INFO] [Client connected {1}:{2}/{3}]", _dateTimeFormat, ipAddress, port, URL);
            _writeInFile.Close();

            #endregion
        }

        /// <summary>
        /// Méthode Send qui écrit dans les logs l'envoi du serveur au client
        /// </summary>
        /// <param name="fileName">Nom du fichier demandé</param>
        /// <param name="ipAddress">IP du client</param>
        /// <param name="port">Port du client</param>
        public void Send(string fileName, string ipAddress, string port)
        {
            #region[Send code]

            _writeInFile = new StreamWriter(_fileNamePath, append: true);
            _writeInFile.Write("[{0}] [INFO] [Send {1} to client {2}:{3}]", _dateTimeFormat, fileName, ipAddress, port);
            _writeInFile.Close();

            #endregion
        }

        /// <summary>
        /// Méthode Error qui écrit dans les logs si le serveur rencontre une erreur
        /// </summary>
        /// <param name="errorType">Le type de l'erreur</param>
        /// <param name="ipAddress">L'adresse du server</param>
        /// <param name="port">Le port du serveur</param>
        /// <param name="URL">URL</param>
        public void Error(string errorType, string ipAddress, string port, string URL)
        {
            #region[Error code]

            _writeInFile = new StreamWriter(_fileNamePath, append: true);
            _writeInFile.WriteLine("[{0}] [ERROR] [{1} {2}:{3}/{4}]", _dateTimeFormat, errorType, ipAddress, port, URL);
            _writeInFile.Close();

            #endregion
        }

        /// <summary>
        /// Méthode Exception qui écrit dans les logs les exeptions rencontrées du serveur
        /// </summary>
        /// <param name="exception">Nom de l'exeption</param>
        public void Exception(string exception)
        {
            #region[Exception code]

            _writeInFile = new StreamWriter(_fileNamePath, append: true);
            _writeInFile.WriteLine("[{0}] [Exeption] [Exeption : {1}]", _dateTimeFormat, exception);
            _writeInFile.Close();

            #endregion
        }
    }
}