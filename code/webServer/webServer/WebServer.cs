/// ETML
/// Auteur : David Dieperink
/// Date : 06.10.2021
/// Description : Projet Reseau (WebServer) -> Class Webserver, cette class lance le serveur et analyse les requêtes envoyées par le client

using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WebServer
{
    /// <summary>
    /// Class WebServer
    /// </summary>
    class WebServer
    {
        #region[Déclaration des variables]

        /// <summary>
        /// Initialisation d'un TcpListener
        /// </summary>
        private TcpListener myListener;
        /// <summary>
        /// Initialisation d'un LogInFile
        /// </summary>
        LogInFile server = new LogInFile();

        #endregion

        /// <summary>
        /// Constructeur de la classe WebServer, il permet d'instancier un nouveau TcpLister avec une adresse IP et un Port donné
        /// </summary>
        /// <param name="serverAddress">Addresse IP du server</param>
        /// <param name="port">Port du serveur</param>
        public WebServer(string serverAddress, string port)
        {
            #region[WebServer code]
            try
            {
                // Récupération et contrôle de l'adresse IP
                IPAddress address = IPAddress.Parse(serverAddress);

                // Appel du TcpListener pour mettre le port donnée en écoute
                myListener = new TcpListener(address, Convert.ToInt32(port));
                myListener.Start();

                // Affichage du message de démarrage du serveur
                Console.WriteLine("Web Server Running {0}:{1}...", address, port);

                server.Running(Convert.ToString(address), port);

                // Initialisation et lancement d'un Thread qui appel la méthode StartListen
                Thread th = new Thread(new ThreadStart(StartListen));
                th.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Une exception s'est produite" + e.ToString());
                server.Exception(e.ToString());
            }
            #endregion
        }

        /// <summary>
        /// Méthode StartListen qui permet d'écouter en continue les actions et traite les requêtes du client
        /// </summary>
        public void StartListen()
        {
            #region[StartListen code]
            int startPos = 0;               // Variable qui contient l'index de position dans la requête 
            string request;                 // Variable qui récupère la requête
            string dirName;                 // Variable qui contient le path du répertoire
            string requestedFile;           // Variable qui récupère le nom du fichier demandé par le client
            string localDir;                // Variable qui contient le path du répertoir local
            string physicalFilePath;        // Variable qui récupère le path physique du fichier

            string myWebServerRoot = "C:\\Partage\\etml-web-server\\web-pages\\src\\html\\";          // Variable qui contient le root path du server
            string myWebServerPathError404 = "404.html";
            string myWebServerPathIndex = "index.html";

            while (true)
            {
                // Accepter une nouvelle connection  
                Socket mySocket = myListener.AcceptSocket();

                IPEndPoint remoteIpEndPoint = mySocket.RemoteEndPoint as IPEndPoint;
                IPEndPoint localIpEndPoint = mySocket.LocalEndPoint as IPEndPoint;

                // Si un client s'est connecté 
                if (mySocket.Connected)
                {
                    // Ecriture des information du client dans la console
                    Console.WriteLine("\nCLIENT: [{0}:{1}]", remoteIpEndPoint.Address, localIpEndPoint.Port);

                    // Tableau de byte qui possède les données du client
                    byte[] receiveDataClient = new byte[1024];

                    // Réception des données du client
                    mySocket.Receive(receiveDataClient, receiveDataClient.Length, 0);

                    // Convertion des données reçues en string
                    string buffer = Encoding.ASCII.GetString(receiveDataClient);

                    // Si le Substring ne contient pas GET
                    if (buffer.Substring(0, 3) != "GET")
                    {
                        Console.WriteLine("Il y a que les méthodes GET qui sont supportées");
                        mySocket.Close();
                        return;
                    }

                    // Obtention de la requête HTTP
                    startPos = buffer.IndexOf("HTTP", 1);

                    // Obtention du text HTTP et la version
                    string httpVersion = buffer.Substring(startPos, 8);

                    // Récupération de la requête (dossier/fichîers)
                    request = buffer.Substring(0, startPos - 1);

                    // Ecriture des logs
                    server.Client(Convert.ToString(remoteIpEndPoint.Address), Convert.ToString(remoteIpEndPoint.Port), request);

                    request.Replace("\\", "/");

                    // Si la requête ne contient pas de ficher
                    if ((request.IndexOf(".") < 1) && (!request.EndsWith("/")))
                    {
                        // Ajout d'un slash à la requête
                        request += "/";
                    }

                    // Récupération du nom de fichier reçu dans la requête
                    startPos = request.LastIndexOf("/") + 1;
                    requestedFile = request.Substring(startPos);

                    // Récupération du nom du répertoire reçu dans la requête
                    dirName = request.Substring(request.IndexOf("/"), request.LastIndexOf("/") - 3);

                    // Si le nom de répertoire est un slash, on renvoie le nom du répertoire par défaut
                    if (dirName == "/")
                    {
                        localDir = myWebServerRoot;
                        Console.WriteLine("Répertoire demandé : " + localDir);
                    }
                    else
                    {
                        localDir = "";
                    }

                    // Initialisation du path local pour fourner au client le fichier demandé
                    physicalFilePath = localDir + requestedFile;
                    Console.WriteLine("Fichier demandé : " + requestedFile);

                    string newRequest = "";

                    // Boucle for qui perment d'enlever le "GET /" pour avoir que le chemin du fichier image
                    for (int x = 5; x < request.Length; x++)
                    {
                        newRequest = newRequest += request[x];
                    }

                    // Contrôler que le path de l'image soit valide
                    if (File.Exists(myWebServerRoot + newRequest) == true)
                    {
                        // Contruction du path de l'image
                        physicalFilePath = myWebServerRoot + newRequest;

                        // Ecriture des logs
                        server.Send(requestedFile, Convert.ToString(remoteIpEndPoint.Address), Convert.ToString(remoteIpEndPoint.Port));
                    }

                    // Si le chemin spécifié n'existe pas
                    if (File.Exists(physicalFilePath) == false)
                    {
                        // Si le répertoire demandé n'est pas spcéifié
                        if (requestedFile == "")
                        {
                            physicalFilePath = myWebServerRoot + myWebServerPathIndex; 
                        }
                        else
                        {
                            physicalFilePath = myWebServerRoot + myWebServerPathError404;
                        }

                        int totalBytes = 0;
                        string response = "";

                        // Initialisation d'un FileStream pour lire et envoyer les fichiers au client
                        FileStream fs = new FileStream(physicalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                        // Initialisation d'un BinaryReader qui permet de lire le byte de FileStream
                        BinaryReader reader = new BinaryReader(fs);

                        byte[] bytes = new byte[fs.Length];
                        int read;

                        while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            response += Encoding.ASCII.GetString(bytes, 0, read);
                            totalBytes += read;
                        }

                        reader.Close();
                        fs.Close();
                        SendHeader(httpVersion, totalBytes, " 200 OK", ref mySocket);
                        CheckDataSendedToBrowser(bytes, ref mySocket);
                    }
                    else
                    {
                        int totalBytes = 0;
                        string response = "";

                        // Initialisation d'un FileStream pour lire et envoyer les fichiers au client
                        FileStream fs = new FileStream(physicalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                        // Initialisation d'un BinaryReader qui permet de lire le byte de FileStream
                        BinaryReader reader = new BinaryReader(fs);

                        byte[] bytes = new byte[fs.Length];
                        int read;

                        while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            response += Encoding.ASCII.GetString(bytes, 0, read);
                            totalBytes += read;
                        }

                        reader.Close();
                        fs.Close();
                        SendHeader(httpVersion, totalBytes, " 200 OK", ref mySocket);
                        CheckDataSendedToBrowser(bytes, ref mySocket);
                    }

                    // Fermeture du Socket
                    mySocket.Close();
                }
            }
            #endregion
        }

        /// <summary>
        /// Méthode SendHeader qui construit et envoie les informations d'en-tête HTTP au navigateur
        /// </summary>
        /// <param name="httpVersion">Version de l'http</param>
        /// <param name="totalBytes">Nombre total de byte</param>
        /// <param name="statusCode">Le status du code(erreur/ok)</param>
        /// <param name="mySocket">Utilise le socket créé pour le client</param>
        public void SendHeader(string httpVersion, int totalBytes, string statusCode, ref Socket mySocket)
        {
            #region[SendHeader code]
            string buffer = "";

            buffer += httpVersion + statusCode + "\r\n";
            buffer += "Server: MyWebServer\r\n";
            buffer += "Accept-Ranges: bytes\r\n";
            buffer += "Longueur du contenu: " + totalBytes + "\r\n\r\n";

            // Convertion du du buffer en tableau de byte
            byte[] sendData = Encoding.ASCII.GetBytes(buffer);

            CheckDataSendedToBrowser(sendData, ref mySocket);

            #endregion
        }

        /// <summary>
        /// Méthode CheckDataSendedToBrowser, elle contrôle la connection du socket et si les paquets envoyés sont intègre
        /// </summary>
        /// <param name="sendData">tableau de byte contenant les données à envoyer au socket</param>
        /// <param name="mySocket">utilise le socket créé pour le client</param>
        public void CheckDataSendedToBrowser(byte[] sendData, ref Socket mySocket)
        {
            #region[CheckDataSendedToBrowser code]
            int numBytes = 0;  // Nombre de byte envoyé

            try
            {
                // Si le socket est connecté
                if (mySocket.Connected)
                {
                    // Contrôle de la longueur des données envoyées au socket + initialisation du nombre de byte et envoie les données au navigateur
                    if ((numBytes = mySocket.Send(sendData, sendData.Length, 0)) == -1)
                    {
                        Console.WriteLine("Erreur de socket, le socket ne peut pas envoyer de paquet");
                    }
                }
                else
                {
                    Console.WriteLine("Connection perdue...");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Une erreur s'est produite : {0} ", e);
            }
            #endregion
        }
    }
}
