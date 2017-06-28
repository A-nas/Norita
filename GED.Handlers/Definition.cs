using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
// a enlever
using System.Windows.Forms;

namespace GED.Handlers
{
    public static class Definition
    {
        // #### CLASS TO HANDLE ALL MY TMP CONFIG VARS #### (a déplacé vers un fichier de configuration)

        //SPIRICA PARAMS
        public static readonly string url = "https://api-recette.spirica.fr/sylveaRS/v1/contrats/"; //113100096/arbitrages";
        public static readonly string id = "NORTIAWS";
        public static readonly string pass = "a*yixw9.8sq"; // auth is base64(id:pass)

        // SFTP PARAMS
        public static readonly string sftpHost = "home663743708.1and1-data.host"; //home663743708.1and1-data.host
        public static readonly string sftpPswd = "ananass123";
        public static readonly string sftpUser = "u87840885-bot";
        public static readonly string sourcePath = "C:\\Users\\alaghouaouta\\Desktop\\Spirica Doc";
        public static readonly string remotePath = "//SPIRICA//"; //sftp://u87840885@home663743708.1and1-data.host/SPIRICA

        // SPIRICA SERIALIZABLE PROPERTIES
        public static List<string> pptActeNames = new List<string> { "reference_externe", "desinvestissements", "reinvestissements", "pieces", "commentaire", "support_saisie", "code_support", "pourcentage", "montant", "nom", "type" };

        // DATABASE STRING
        public static readonly SqlConnection connexion = new SqlConnection("data source=192.168.1.2\\SQL2005DEV;Database=Nortiaca_MEDIA;Uid=sa;password=NICKEL2000;");

        
        //THIS IS FOR TEST
        private static ProgressBar progBar;
        public static ProgressBar getProgressBar()
        {
            return progBar;
        }
        public static void setProgressBar(ProgressBar pb)
        {
            progBar = pb;
        }

        //
        //
    }
}
