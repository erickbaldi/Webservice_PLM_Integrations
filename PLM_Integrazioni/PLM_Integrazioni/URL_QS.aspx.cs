using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace PLM_Integrazioni
{
    public partial class URL_QS : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string articolo = Request.QueryString["articolo"];
            string tipoOutput = Request.QueryString["output"];
            string dimensione = Request.QueryString["dimensione"];
            string tipoDoc = Request.QueryString["tipoDoc"];

            if (articolo != null && tipoOutput == "url")
            {
                String ambiente = ConfigurationManager.AppSettings["serverPLM"];
                String url_iniziale = "";

                if (ambiente == "TEST")
                {
                    url_iniziale = "https://jdewebsrv01.felsineo.local/gsm/baseforms/frmTradeUnit.aspx?MaintainSpec=true&SpecID=";
                }
                else if (ambiente == "PROD")
                {
                    url_iniziale = "https://jdewebsrv02.felsineo.local/gsm/baseforms/frmTradeUnit.aspx?MaintainSpec=true&SpecID=";
                }
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PLM4PConnectionString"].ConnectionString);
                conn.Open();
                string checkURL = "select '"+ url_iniziale +"'+fkspecID as webURL from dbo.specLegacySpecJoin where Equivalent = '"+articolo+"'";
                SqlCommand cmd = new SqlCommand(checkURL, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    String url = (String)reader.GetValue(0);
                    Response.Redirect(url);
                }
                reader.Close();
                cmd.Dispose();
                conn.Close();
            }

            if (articolo != null && tipoOutput == "img")
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PLM4PConnectionString"].ConnectionString);
                conn.Open();

                string checkImg = "";
                string varSize = "";
                if (dimensione == "original")
                {
                    checkImg = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+c.pkid+'' as webImg from dbo.specLegacySpecJoin a inner join dbo.Thumbnail b on a.fkSpecID = b.FKSpecID inner join dbo.DrlAttachment c on c.pkid = b.OriginDRLId where Equivalent = '" + articolo + "'";
                    varSize = "100%";
                }
                else if (dimensione == "medium")
                {
                    checkImg = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+c.pkid+'' as webImg from dbo.specLegacySpecJoin a inner join dbo.Thumbnail b on a.fkSpecID = b.FKSpecID inner join dbo.DrlAttachment c on c.pkid = b.OriginDRLId where Equivalent = '" + articolo + "'";
                    varSize = "50%";
                }
                else if (dimensione == "small")
                {
                    checkImg = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+c.pkid+'' as webImg from dbo.specLegacySpecJoin a inner join dbo.Thumbnail b on a.fkSpecID = b.FKSpecID inner join dbo.DrlAttachment c on c.pkid = b.OriginDRLId where Equivalent = '" + articolo + "'";
                    varSize = "25%";
                }
                else
                    return;

                SqlCommand cmd = new SqlCommand(checkImg, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    String url = (String)reader.GetValue(0);
                    
                    //converto l'immagine originale in Base64
                    byte[] imageArray = System.IO.File.ReadAllBytes(Page.ResolveUrl(url));
                    string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                    //creo il file HTML e poi lo mostro nel browser
                    string html = "<img alt=\"\" style=\"width:"+varSize+";\" src=\"data:image/jpeg;base64," + base64ImageRepresentation + "\"/>";
                    File.WriteAllText("C:\\PLM_Integrazioni\\TEMP\\file.html", html);
                    Response.Write(html);
                }
                reader.Close();
                cmd.Dispose();
                conn.Close();
            }

            if (articolo != null && tipoOutput == "doc")
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PLM4PConnectionString"].ConnectionString);
                conn.Open();

                string checkDoc = "";

                switch (tipoDoc)
                {
                    case "SchedaTecnica":
                        checkDoc = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+a.pkid+'' as webDoc from dbo.specLegacySpecJoin z inner join dbo.SDMSupportingDocument d on z.fkSpecID = d.fkEntity inner join dbo.SDMTypes t on d.fkDocumentType = t.pkid inner join dbo.SDMAttachment s on s.fkOwner = d.fkOriginator and s.fkEntity = d.pkid inner join dbo.DrlAttachmentReference r on r.fkOwner = s.pkid inner join dbo.DrlAttachment a on r.DrlAttachmentId = a.pkid inner join dbo.SDMStatus st on d.fkStatus = st.pkid and st.Category = 'GSM' where d.InactiveDate >= getdate() and st.Name = 'Approved' and t.Name = 'Scheda Tecnica' and z.Equivalent = '" + articolo + "'";
                        break;

                    case "DichiarazioneNutrizionale":
                        checkDoc = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+a.pkid+'' as webDoc from dbo.specLegacySpecJoin z inner join dbo.SDMSupportingDocument d on z.fkSpecID = d.fkEntity inner join dbo.SDMTypes t on d.fkDocumentType = t.pkid inner join dbo.SDMAttachment s on s.fkOwner = d.fkOriginator and s.fkEntity = d.pkid inner join dbo.DrlAttachmentReference r on r.fkOwner = s.pkid inner join dbo.DrlAttachment a on r.DrlAttachmentId = a.pkid inner join dbo.SDMStatus st on d.fkStatus = st.pkid and st.Category = 'GSM' where d.InactiveDate >= getdate() and st.Name = 'Approved' and t.Name = 'Dichiarazione nutrizionale' and z.Equivalent = '" + articolo + "'";
                        break;

                    case "ParametriMicroBiologiciChimicoFisici":
                        checkDoc = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+a.pkid+'' as webDoc from dbo.specLegacySpecJoin z inner join dbo.SDMSupportingDocument d on z.fkSpecID = d.fkEntity inner join dbo.SDMTypes t on d.fkDocumentType = t.pkid inner join dbo.SDMAttachment s on s.fkOwner = d.fkOriginator and s.fkEntity = d.pkid inner join dbo.DrlAttachmentReference r on r.fkOwner = s.pkid inner join dbo.DrlAttachment a on r.DrlAttachmentId = a.pkid inner join dbo.SDMStatus st on d.fkStatus = st.pkid and st.Category = 'GSM' where d.InactiveDate >= getdate() and st.Name = 'Approved' and t.Name = 'Parametri microbiologici, chimico-fisici' and z.Equivalent = '" + articolo + "'";
                        break;

                    case "StandardEtichettePF":
                        checkDoc = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+a.pkid+'' as webDoc from dbo.specLegacySpecJoin z inner join dbo.SDMSupportingDocument d on z.fkSpecID = d.fkEntity inner join dbo.SDMTypes t on d.fkDocumentType = t.pkid inner join dbo.SDMAttachment s on s.fkOwner = d.fkOriginator and s.fkEntity = d.pkid inner join dbo.DrlAttachmentReference r on r.fkOwner = s.pkid inner join dbo.DrlAttachment a on r.DrlAttachmentId = a.pkid inner join dbo.SDMStatus st on d.fkStatus = st.pkid and st.Category = 'GSM' where d.InactiveDate >= getdate() and st.Name = 'Approved' and t.Name = 'Standard etichette PF' and z.Equivalent = '" + articolo + "'";
                        break;

                    case "SchedaAllergeni":
                        checkDoc = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+a.pkid+'' as webDoc from dbo.specLegacySpecJoin z inner join dbo.SDMSupportingDocument d on z.fkSpecID = d.fkEntity inner join dbo.SDMTypes t on d.fkDocumentType = t.pkid inner join dbo.SDMAttachment s on s.fkOwner = d.fkOriginator and s.fkEntity = d.pkid inner join dbo.DrlAttachmentReference r on r.fkOwner = s.pkid inner join dbo.DrlAttachment a on r.DrlAttachmentId = a.pkid inner join dbo.SDMStatus st on d.fkStatus = st.pkid and st.Category = 'GSM' where d.InactiveDate >= getdate() and st.Name = 'Approved' and t.Name = 'Scheda allergeni' and z.Equivalent = '" + articolo + "'";
                        break;

                    case "SchedaSicurezza":
                        checkDoc = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+a.pkid+'' as webDoc from dbo.specLegacySpecJoin z inner join dbo.SDMSupportingDocument d on z.fkSpecID = d.fkEntity inner join dbo.SDMTypes t on d.fkDocumentType = t.pkid inner join dbo.SDMAttachment s on s.fkOwner = d.fkOriginator and s.fkEntity = d.pkid inner join dbo.DrlAttachmentReference r on r.fkOwner = s.pkid inner join dbo.DrlAttachment a on r.DrlAttachmentId = a.pkid inner join dbo.SDMStatus st on d.fkStatus = st.pkid and st.Category = 'GSM' where d.InactiveDate >= getdate() and st.Name = 'Approved' and t.Name = 'Scheda di Sicurezza' and z.Equivalent = '" + articolo + "'";
                        break;

                    case "CertificatoConformita":
                        checkDoc = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+a.pkid+'' as webDoc from dbo.specLegacySpecJoin z inner join dbo.SDMSupportingDocument d on z.fkSpecID = d.fkEntity inner join dbo.SDMTypes t on d.fkDocumentType = t.pkid inner join dbo.SDMAttachment s on s.fkOwner = d.fkOriginator and s.fkEntity = d.pkid inner join dbo.DrlAttachmentReference r on r.fkOwner = s.pkid inner join dbo.DrlAttachment a on r.DrlAttachmentId = a.pkid inner join dbo.SDMStatus st on d.fkStatus = st.pkid and st.Category = 'GSM' where d.InactiveDate >= getdate() and st.Name = 'Approved' and t.Name = 'Certificato di conformità' and z.Equivalent = '" + articolo + "'";
                        break;

                    case "EtichettaPezzo":
                        checkDoc = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+a.pkid+'' as webDoc from dbo.specLegacySpecJoin z inner join dbo.SDMSupportingDocument d on z.fkSpecID = d.fkEntity inner join dbo.SDMTypes t on d.fkDocumentType = t.pkid inner join dbo.SDMAttachment s on s.fkOwner = d.fkOriginator and s.fkEntity = d.pkid inner join dbo.DrlAttachmentReference r on r.fkOwner = s.pkid inner join dbo.DrlAttachment a on r.DrlAttachmentId = a.pkid inner join dbo.SDMStatus st on d.fkStatus = st.pkid and st.Category = 'GSM' where d.InactiveDate >= getdate() and st.Name = 'Approved' and t.Name = 'Etichetta pezzo' and z.Equivalent = '" + articolo + "'";
                        break;

                    case "EtichettaCartone":
                        checkDoc = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+a.pkid+'' as webDoc from dbo.specLegacySpecJoin z inner join dbo.SDMSupportingDocument d on z.fkSpecID = d.fkEntity inner join dbo.SDMTypes t on d.fkDocumentType = t.pkid inner join dbo.SDMAttachment s on s.fkOwner = d.fkOriginator and s.fkEntity = d.pkid inner join dbo.DrlAttachmentReference r on r.fkOwner = s.pkid inner join dbo.DrlAttachment a on r.DrlAttachmentId = a.pkid inner join dbo.SDMStatus st on d.fkStatus = st.pkid and st.Category = 'GSM' where d.InactiveDate >= getdate() and st.Name = 'Approved' and t.Name = 'Etichetta cartone' and z.Equivalent = '" + articolo + "'";
                        break;

                    case "EtichettaPallet":
                        checkDoc = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+a.pkid+'' as webDoc from dbo.specLegacySpecJoin z inner join dbo.SDMSupportingDocument d on z.fkSpecID = d.fkEntity inner join dbo.SDMTypes t on d.fkDocumentType = t.pkid inner join dbo.SDMAttachment s on s.fkOwner = d.fkOriginator and s.fkEntity = d.pkid inner join dbo.DrlAttachmentReference r on r.fkOwner = s.pkid inner join dbo.DrlAttachment a on r.DrlAttachmentId = a.pkid inner join dbo.SDMStatus st on d.fkStatus = st.pkid and st.Category = 'GSM' where d.InactiveDate >= getdate() and st.Name = 'Approved' and t.Name = 'Etichetta pallet' and z.Equivalent = '" + articolo + "'";
                        break;

                    case "SpecifichePallettizzazione":
                        checkDoc = "select 'C:/AGILE_HOME/PLM4P_HOME/XDocuments/Drl/'+a.pkid+'' as webDoc from dbo.specLegacySpecJoin z inner join dbo.SDMSupportingDocument d on z.fkSpecID = d.fkEntity inner join dbo.SDMTypes t on d.fkDocumentType = t.pkid inner join dbo.SDMAttachment s on s.fkOwner = d.fkOriginator and s.fkEntity = d.pkid inner join dbo.DrlAttachmentReference r on r.fkOwner = s.pkid inner join dbo.DrlAttachment a on r.DrlAttachmentId = a.pkid inner join dbo.SDMStatus st on d.fkStatus = st.pkid and st.Category = 'GSM' where d.InactiveDate >= getdate() and st.Name = 'Approved' and t.Name = 'Specifiche Palletizzazione' and z.Equivalent = '" + articolo + "'";
                        break;

                    default:
                        Response.Write("Tipo documento errato!");
                        break;
                }
                
                SqlCommand cmd = new SqlCommand(checkDoc, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    String url = (String)reader.GetValue(0);

                    Response.ContentType = "Application/pdf";
                    Response.TransmitFile(url);
                }
                reader.Close();
                cmd.Dispose();
                conn.Close();
            }

            else
                return;
        }

    }
}