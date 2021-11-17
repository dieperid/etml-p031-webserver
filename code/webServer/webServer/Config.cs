/// ETML
/// Auteur : David Dieperink
/// Date : 06.10.2021
/// Description : Projet Reseau (WebServer) -> Class Config qui récupère les information du fichier config.json

namespace WebServer
{
    /// <summary>
    /// Class Config
    /// </summary>
    class Config
    {
        #region[Déclaration des variables]

        /// <summary>
        /// Getter, setter sur l'adresse IP locale
        /// </summary>
        public string addressLocal { get; set; }
        /// <summary>
        /// Getter, setter szr l'adresse IP de la VM serveur
        /// </summary>
        public string addressServWin { get; set; }
        /// <summary>
        /// Getter, setter sur le port
        /// </summary>
        public string port { get; set; }

        #endregion
    }
}
