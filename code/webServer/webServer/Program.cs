/// ETML
/// Auteur : David Dieperink
/// Date : 06.10.2021
/// Description : Projet Reseau (WebServer) 

using System.IO;
using Newtonsoft.Json;

namespace WebServer
{
    /// <summary>
    /// Class Program
    /// </summary>
    class Program
    {
        /// <summary>
        /// Méthode Main qui est le main du programme
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            #region[Main code]
            // Récupèration des données du fichier config.json 
            StreamReader r = new StreamReader("config.json");
            string jsonString = r.ReadToEnd();

            Config serverData = JsonConvert.DeserializeObject<Config>(jsonString);

            // Initialisation d'un nouvel objet de l'instance WebServer
            WebServer webServer = new WebServer(serverData.addressServWin, serverData.port);
            #endregion
        }
    }
}
