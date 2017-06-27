using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// JSON & DB CONNEXION
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
// envoie SFTP
using Renci.SshNet;
using System.IO;
using System.Threading;
// a effecer apres
using System.Windows.Forms;


namespace GED.Handlers
{
    // Classe qui concerne SPIRICA (prod {json,pdf})
    public class Spirica : Acte,IActe
    {
        List<Spirica> production = new List<Spirica>();

        [JsonProperty(PropertyName = "support_saisie", Order = 5)]
        private string supsaisie = "bo";
        [JsonProperty(PropertyName = "pieces", Order = 7)]
        public List<DetailPiece> pieces = new List<DetailPiece>();
        [JsonProperty(PropertyName = "file", Order = 999)]
        public string fichiers; // cette propriete ontint les fichiers binaire et leurs noms  

        // pointeur de fonction PreProcessInformation appelé quand l'envoie asynchrone est fini
        private delegate void finishTask(SftpClient cli, FileStream fs, IAsyncResult ar);

        public Spirica() { }
        //passez par ce constructeur au moment de la generation (le cast ne marche pas)
        public Spirica(List<Acte> actes)
        {
            foreach(Acte a in actes)
            {
                Spirica item = new Spirica();
                item.NomType = a.NomType;
                item.NomActeAdministratif = a.NomActeAdministratif;
                item.ReferenceInterne = a.ReferenceInterne;
                item.NomCompletSouscripteurs = a.NomCompletSouscripteurs;
                item.NumContrat = a.NumContrat;
                item.CodeApporteur = a.CodeApporteur;
                item.NomApporteur = a.NomApporteur;
                item.MontantBrut = a.MontantBrut;
                item.TypeFrais = a.TypeFrais;
                item.Frais = a.Frais;
                item.ID_ProfilCompagnie = a.ID_ProfilCompagnie;
                item.NomEnveloppe = a.NomEnveloppe;
                item.ListeSupportDesinvestir = a.ListeSupportDesinvestir;
                item.ListeSupportInvestir = a.ListeSupportInvestir;
                item.ListeDocument = a.ListeDocument;
                item.IsTraitementEdi = a.IsTraitementEdi;
                item.DateCreation = a.DateCreation;
                item.DateAcquisition = a.DateAcquisition;
                item.Commentaire = a.Commentaire;
                item.InvestissementImmediat = a.InvestissementImmediat;
                item.Regul = a.Regul;

                production.Add(item);
            }
            //fill missing data
            getDetailPiece();

    }

        // cherche les details des documents (nom et type) et rempli la liste d'objet Spirica.pieces
        private void getDetailPiece()
        {
            SqlConnection con = Definition.connexion;
            foreach(Spirica a in production)
            {
                List<int> id_docNortia = new List<int>();
                foreach (DocumentProduction r in a.ListeDocument) id_docNortia.Add(r.ID_DocumentNortia);
                var cmd = new SqlCommand("select nom,type_doc='type' from pli where ID_Pli in ({id_doc})");
                cmd.addArrayCommand(id_docNortia, "id_doc");
                cmd.Connection = con;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DetailPiece detPiece = new DetailPiece();
                    detPiece.nomFichier = reader[0].ToString();
                    detPiece.typeFicher = reader[1].ToString();
                    a.pieces.Add(detPiece);
                }
                reader.Close();
                con.Close();
                id_docNortia.Clear();
            }

        }

        // genere la prod en Json
        public string genJSON() // ca c'est moche
        {
                        JsonSerializerSettings jsonSetting = new JsonSerializerSettings
            {
                // ces options sont valable aussi pour les sous objets
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new ShouldSerializeContractResolver()
            };
            return JsonConvert.SerializeObject(production, jsonSetting);
        }

        //envoie la prod PDF (pdfs separés)
        public void sendProd()
        {
            try
            {
                FileInfo f = new FileInfo(Definition.sourcePath + "\\test.pdf");
                SftpClient client = new SftpClient(Definition.sftpHost, 22, Definition.sftpUser, Definition.sftpPswd);
                client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(60); // set auth timeout = 60 sec
                client.Connect();
                if (client.IsConnected)
                {
                    FileStream fstream = new FileStream(f.FullName, FileMode.Open);
                    Definition.getProgressBar().Invoke(
                                (MethodInvoker)delegate { Definition.getProgressBar().Maximum = (int)fstream.Length; });
                    finishTask funcPtr = new finishTask(PreProcessInformation);
                    IAsyncResult ar = client.BeginUploadFile(fstream, Definition.remotePath + "" + f.Name, ac => {
                        funcPtr(client, fstream,null); }
                    , client, UpdateProgresBar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }   
        }


        //## UPDATE PROGRESS BAR ON FOREGROUD THREAD
        private void UpdateProgresBar(ulong uploaded)
        {   
            Definition.getProgressBar().Invoke((MethodInvoker)delegate { Definition.getProgressBar().Value = (int)uploaded; });
        }


        //METHOD AUTO CALLED WHEN CLIENT FINISH TO TRANSFER FILES (ASYNC TASK)
        private void PreProcessInformation(SftpClient cli, FileStream fs,IAsyncResult ar = null)
        {
            // do stuff when completed, basicly send a message or close the connexiox

            // free resources
            cli.Disconnect();
            fs.Close();
            Definition.getProgressBar().Invoke((MethodInvoker)delegate { Definition.getProgressBar().Value = 0; });
            MessageBox.Show("finished");
        }
        

        public static void showFiles(string dir = "//SPIRICA//" ){
            IEnumerable<string> str = null;

            FileInfo f = new FileInfo(Definition.sourcePath + "\\test.pdf");
            SftpClient client = new SftpClient(Definition.sftpHost, 22, Definition.sftpUser, Definition.sftpPswd);
            client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(60); // set auth timeout = 60 sec
            client.Connect();
            if (client.IsConnected)
            {
                
                str = client.ListDirectory(dir).Select(s => s.FullName);
            }

            foreach (var v in str)
            {
                MessageBox.Show(v);
            }
            
        }

    }
}

