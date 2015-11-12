using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml;
using System.Net;
using System.IO;
using System.Text;
using System.Xml.Linq;
using log4net;


namespace MvcPlanningApplication.Models
{
    public class HaworthOrders : List<HaworthOrder>
    {
        private static readonly ILog Logger = LogHelper.GetLogger();
        public StringBuilder mStrBldrXML { get; set; }
        //public XmlDocument XMLDoc { get; set; }
        public NetworkCredential NetworkCredentials { get; set; }
        private Uri URI { get; set; }
        private bool mIsFTP { get; set; }


        public HaworthOrders(string FileNameAndLocation)
            : base()
        {
            XmlDocument XMLDoc = new System.Xml.XmlDocument();

            XMLDoc.Load(FileNameAndLocation);
            Populate(XMLDoc);
        }

        public HaworthOrders(Uri HaworthURL, bool isFTP = false)
            : base()
        {
            XmlDocument XMLDoc;

            URI = HaworthURL;
            mIsFTP = isFTP;

            if (mIsFTP)
            {
                List<string> FtpFiles = new List<string>();
                FtpWebRequest request;
                FtpWebResponse response;
                Stream responseStream;
                StreamReader reader;


                NetworkCredentials = new NetworkCredential("WTFabInc", "***REMOVED***");

                request = (FtpWebRequest)WebRequest.Create(URI.AbsoluteUri);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = NetworkCredentials;
                response = (FtpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                reader = new StreamReader(responseStream);
                while (!reader.EndOfStream)
                    FtpFiles.Add(reader.ReadLine());
                reader.Close();
                responseStream.Close();
                response.Close();

                mStrBldrXML = new StringBuilder();
                foreach (var strFile in FtpFiles)
                {
                    XMLDoc = new System.Xml.XmlDocument();
                    request = (FtpWebRequest)WebRequest.Create(URI.AbsoluteUri + "//" + strFile);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.Credentials = NetworkCredentials;

                    response = (FtpWebResponse)request.GetResponse();

                    responseStream = response.GetResponseStream();

                    //var test = new XmlTextReader(responseStream);
                    XMLDoc.Load(responseStream);
                    mStrBldrXML.AppendLine(XMLDoc.InnerXml
                        .Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", string.Empty)
                        .Replace("<HaworthOrders>", string.Empty)
                        .Replace("</HaworthOrders>", string.Empty)
                        .Replace("<FileName>", "<File Name=\"")
                        .Replace("</FileName>", "\">")
                        /*.Replace("</RevisionLevel>", "</RevisionLevel>" + //I insert the Characteristics XML into the spot after the Revision Tag...
                        "<Characteristics>" +
			                "<ArmStyle>NO_ARM</ArmStyle>" +
			                "<SeatMaterial>PLST</SeatMaterial>" +
			                "<BackJacket>NO_BKJCK</BackJacket>" +
			                "<FireCode>NO_COD Surface Treatment</FireCode>" +
			                "<FrontSide>NO_TREAT</FrontSide>" +
			                "<Seat1Color>TR_FJ</Seat1Color>" +
			                "<Seat2Color>NO_COL</Seat2Color>" +
			                "<Back1ColorInside>TR_FJ</Back1ColorInside>" +
			                "<FrameColor>KR_V</FrameColor>" +
                            "<TrimColor>NO_COL</TrimColor>" +
		                "</Characteristics>")*/
                        + "</File>");
                    responseStream.Close();
                    response.Close();

                    PopulateFromFTP(XMLDoc, strFile);

                    //this.Last().FileName = strFile;
                }

                MoveCompletedOrderFiles();
                RemoveDuplicates();
            }
            else
            {
                mStrBldrXML = new StringBuilder();
                XMLDoc = new System.Xml.XmlDocument();
                XMLDoc.Load(HaworthURL.AbsoluteUri);
                mStrBldrXML.AppendLine(XMLDoc.InnerXml);
                Populate(XMLDoc);
            }
        }

        //Removes any of the orders that are duplicates since updates to an order result in a new order file.
        private void RemoveDuplicates()
        {
            var objQuery = this.GroupBy(o => o.OrderNumber)
                .Where(g => g.Count() > 1)
                .Select(a => new { OrderNumber = a.Key })
                .ToList();

            foreach (var objOrder in objQuery)
            {
                var ordersToRemove = this
                    .Where(o => o.OrderNumber.Equals(objOrder.OrderNumber))
                    .OrderBy(o => o.ChangeDate)
                    .ToList();

                for (var intCounter = 0; intCounter < ordersToRemove.Count() - 1; ++intCounter)
                    this.Remove(ordersToRemove[intCounter]);
            }
        }

        private void MoveCompletedOrderFiles()
        {
            FtpWebRequest request;
            FtpWebResponse response;
            List<HaworthOrder> objOrdersToRemove;


            objOrdersToRemove = new List<HaworthOrder>();
            foreach (var objOrder in this)
            {
                if (objOrder.StatusCode.Trim().ToUpper().Equals("5 - CLOSED"))
                {
                    request = (FtpWebRequest)WebRequest.Create(URI.AbsoluteUri + "//" + objOrder.FileName);
                    request.Method = WebRequestMethods.Ftp.Rename;
                    request.Credentials = NetworkCredentials;
                    request.RenameTo = "../In/" + objOrder.FileName;
                    response = (FtpWebResponse)request.GetResponse();

                    //objOrdersToRemove.Add(objOrder);
                    objOrdersToRemove
                        .AddRange(this.Where(o => o.OrderNumber.Equals(objOrder.OrderNumber))); //remove ALL the order files with the corresponding "5 - CLOSED" order number from the ftp directory
                }
            }

            foreach (var objOrderToRemove in objOrdersToRemove)
                this.Remove(objOrderToRemove);
        }

        private void PopulateFromFTP(XmlDocument XMLDoc, string File)
        {
            XmlNodeList objXMLNodes;
            HaworthOrder objOrder;
            DateTime dtmTemp;
            double dblTemp;
            string strFileName = string.Empty;


            objXMLNodes = XMLDoc.SelectNodes("HaworthOrders");

            foreach (XmlNode objXMLNode in objXMLNodes[0])
            {
                objOrder = new HaworthOrder();

                if (objXMLNode.Name.Equals("FileName"))
                {
                    strFileName = objXMLNode.InnerText;
                    //Logger.Debug("Filename is: " + strFileName);
                    continue;
                }

                objOrder.Type = objXMLNode.Attributes["type"].Value;
                objOrder.FileName = strFileName;
                objOrder.File = File;
                foreach (XmlNode objChildNode in objXMLNode.ChildNodes)
                {
                    switch (objChildNode.Name)
                    {
                        case "OrderNumber":
                            objOrder.OrderNumber = objChildNode.InnerText;
                            break;
                        case "VariantName":
                            objOrder.VariantName = objChildNode.InnerText;
                            break;
                        case "OrgCode":
                            objOrder.OrgCode = objChildNode.InnerText;
                            break;
                        case "OrgName":
                            objOrder.OrgName = objChildNode.InnerText;
                            break;
                        case "StatusCode":
                            objOrder.StatusCode = objChildNode.InnerText;
                            break;
                        case "ChangeDate":
                            objOrder.ChangeDate = DateTime.TryParse(objChildNode.InnerText, out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                        case "BuyerID":
                            objOrder.BuyerID = objChildNode.InnerText;
                            break;
                        case "PlannerID":
                            objOrder.PlannerID = objChildNode.InnerText;
                            break;
                        case "DockDate":
                            objOrder.DockDate = DateTime.TryParse(objChildNode.InnerText, out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                        case "SupplierNumber":
                            objOrder.SupplierNumber = objChildNode.InnerText;
                            break;
                        case "SupplierName":
                            objOrder.SupplierNumber = objChildNode.InnerText;
                            break;
                        case "PartInformation":
                            objOrder.PartInformation = new HaworthPartInformation();
                            foreach (XmlNode objChildNode2 in objChildNode.ChildNodes)
                            {
                                switch (objChildNode2.Name)
                                {
                                    case "Description":
                                        objOrder.PartInformation.Description = objChildNode2.InnerText;
                                        break;
                                    case "Description2":
                                        objOrder.PartInformation.Description2 = objChildNode2.InnerText;
                                        break;
                                    case "ColorCode":
                                        objOrder.PartInformation.ColorCode = objChildNode2.InnerText;
                                        break;
                                    case "ColorPattern":
                                        objOrder.PartInformation.ColorPattern = objChildNode2.InnerText;
                                        break;
                                    case "ColorDescription":
                                        objOrder.PartInformation.ColorDescription = objChildNode2.InnerText;
                                        break;
                                    case "EngineeringDrawingNumber":
                                        objOrder.PartInformation.EngineeringDrawingNumber = objChildNode2.InnerText;
                                        break;
                                    case "RevisionLevel":
                                        objOrder.PartInformation.RevisionLevel = objChildNode2.InnerText;
                                        break;
                                }
                            }
                            break;
                        case "ItemNumber":
                            objOrder.ItemNumber = objChildNode.InnerText;
                            break;
                        case "RequiredQty":
                            objOrder.RequiredQty = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "ReceivedQty":
                            objOrder.ReceivedQty = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "UOM":
                            objOrder.UnitOfMeasure = objChildNode.InnerText;
                            break;
                        case "UnitPrice":
                            objOrder.UnitPrice = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "PartWeight":
                            objOrder.PartWeight = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0; ;
                            break;
                        case "CartonWeight":
                            objOrder.CartonWeight = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "CartonLength":
                            objOrder.CartonLength = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "CartonWidth":
                            objOrder.CartonWidth = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "CartonHeight":
                            objOrder.CartonHeight = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "DeliveryInformation":
                            objOrder.DeliveryInformation = new HaworthDeliveryInformation();
                            foreach (XmlNode objChildNode2 in objChildNode.ChildNodes)
                            {
                                switch (objChildNode2.Name)
                                {
                                    case "PlantCode":
                                        objOrder.DeliveryInformation.PlantCode = objChildNode2.InnerText;
                                        break;
                                    case "PlantName":
                                        objOrder.DeliveryInformation.PlantName = objChildNode2.InnerText;
                                        break;
                                    case "PlantName2":
                                        objOrder.DeliveryInformation.PlantName2 = objChildNode2.InnerText;
                                        break;
                                    case "Address":
                                        objOrder.DeliveryInformation.Address = objChildNode2.InnerText;
                                        break;
                                    case "Address2":
                                        objOrder.DeliveryInformation.Address2 = objChildNode2.InnerText;
                                        break;
                                    case "City":
                                        objOrder.DeliveryInformation.City = objChildNode2.InnerText;
                                        break;
                                    case "State":
                                        objOrder.DeliveryInformation.State = objChildNode2.InnerText;
                                        break;
                                    case "ZipCode":
                                        objOrder.DeliveryInformation.ZipCode = objChildNode2.InnerText;
                                        break;
                                    case "Country":
                                        objOrder.DeliveryInformation.Country = objChildNode2.InnerText;
                                        break;
                                }
                            }
                            break;
                        case "LeadTime":
                            objOrder.LeadTime = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "ShippingInstructions":
                            objOrder.ShippingInstructions = objChildNode.InnerText;
                            break;
                        case "PurchText":
                            objOrder.PurchText = objChildNode.InnerText;
                            break;
                        case "PurchText2":
                            objOrder.PurchText2 = objChildNode.InnerText;
                            break;
                        case "TransType":
                            objOrder.TransType = objChildNode.InnerText;
                            break;
                        case "MaintenanceDateTime":
                            objOrder.MaintenanceDateTime = DateTime.TryParse(objChildNode.InnerText, out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                        case "ImportDateTime":
                            objOrder.ImportDateTime = DateTime.TryParse(objChildNode.InnerText, out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                    }
                }

                this.Add(objOrder);
            }
        }

        private void Populate(XmlDocument XMLDoc)
        {
            XmlNodeList objXMLNodes;
            HaworthOrder objOrder;
            DateTime dtmTemp;
            double dblTemp;


            objXMLNodes = XMLDoc.SelectNodes("HaworthOrders");

            foreach (XmlNode objXMLNode in objXMLNodes[0])
            {
                objOrder = new HaworthOrder();

                objOrder.Type = objXMLNode.Attributes["type"].Value;
                foreach (XmlNode objChildNode in objXMLNode.ChildNodes)
                {
                    switch (objChildNode.Name)
                    {
                        case "OrderNumber":
                            objOrder.OrderNumber = objChildNode.InnerText;
                            break;
                        case "VariantName":
                            objOrder.VariantName = objChildNode.InnerText;
                            break;
                        case "OrgCode":
                            objOrder.OrgCode = objChildNode.InnerText;
                            break;
                        case "OrgName":
                            objOrder.OrgName = objChildNode.InnerText;
                            break;
                        case "StatusCode":
                            objOrder.StatusCode = objChildNode.InnerText;
                            break;
                        case "ChangeDate":
                            objOrder.ChangeDate = DateTime.TryParse(objChildNode.InnerText, out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                        case "BuyerID":
                            objOrder.BuyerID = objChildNode.InnerText;
                            break;
                        case "PlannerID":
                            objOrder.PlannerID = objChildNode.InnerText;
                            break;
                        case "DockDate":
                            objOrder.DockDate = DateTime.TryParse(objChildNode.InnerText, out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                        case "SupplierNumber":
                            objOrder.SupplierNumber = objChildNode.InnerText;
                            break;
                        case "PartInformation":
                            objOrder.PartInformation = new HaworthPartInformation();
                            foreach (XmlNode objChildNode2 in objChildNode.ChildNodes)
                            {
                                switch (objChildNode2.Name)
                                {
                                    case "Description":
                                        objOrder.PartInformation.Description = objChildNode2.InnerText;
                                        break;
                                    case "Description2":
                                        objOrder.PartInformation.Description2 = objChildNode2.InnerText;
                                        break;
                                    case "ColorCode":
                                        objOrder.PartInformation.ColorCode = objChildNode2.InnerText;
                                        break;
                                    case "ColorPattern":
                                        objOrder.PartInformation.ColorPattern = objChildNode2.InnerText;
                                        break;
                                    case "ColorDescription":
                                        objOrder.PartInformation.ColorDescription = objChildNode2.InnerText;
                                        break;
                                    case "EngineeringDrawingNumber":
                                        objOrder.PartInformation.EngineeringDrawingNumber = objChildNode2.InnerText;
                                        break;
                                    case "RevisionLevel":
                                        objOrder.PartInformation.RevisionLevel = objChildNode2.InnerText;
                                        break;
                                }
                            }
                            break;
                        case "ItemNumber":
                            objOrder.ItemNumber = objChildNode.InnerText;
                            break;
                        case "RequiredQty":
                            objOrder.RequiredQty = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "ReceivedQty":
                            objOrder.ReceivedQty = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "UOM":
                            objOrder.UnitOfMeasure = objChildNode.InnerText;
                            break;
                        case "UnitPrice":
                            objOrder.UnitPrice = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "PartWeight":
                            objOrder.PartWeight = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0; ;
                            break;
                        case "CartonWeight":
                            objOrder.CartonWeight = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "CartonLength":
                            objOrder.CartonLength = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "CartonWidth":
                            objOrder.CartonWidth = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "CartonHeight":
                            objOrder.CartonHeight = double.TryParse(objChildNode.InnerText, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case "DeliveryInformation":
                            objOrder.DeliveryInformation = new HaworthDeliveryInformation();
                            foreach (XmlNode objChildNode2 in objChildNode.ChildNodes)
                            {
                                switch (objChildNode2.Name)
                                {
                                    case "PlantCode":
                                        objOrder.DeliveryInformation.PlantCode = objChildNode2.InnerText;
                                        break;
                                    case "PlantName":
                                        objOrder.DeliveryInformation.PlantName = objChildNode2.InnerText;
                                        break;
                                    case "Address":
                                        objOrder.DeliveryInformation.Address = objChildNode2.InnerText;
                                        break;
                                    case "Address2":
                                        objOrder.DeliveryInformation.Address2 = objChildNode2.InnerText;
                                        break;
                                    case "City":
                                        objOrder.DeliveryInformation.City = objChildNode2.InnerText;
                                        break;
                                    case "State":
                                        objOrder.DeliveryInformation.State = objChildNode2.InnerText;
                                        break;
                                    case "ZipCode":
                                        objOrder.DeliveryInformation.ZipCode = objChildNode2.InnerText;
                                        break;
                                    case "Country":
                                        objOrder.DeliveryInformation.Country = objChildNode2.InnerText;
                                        break;
                                }
                            }
                            break;
                        case "TransType":
                            objOrder.TransType = objChildNode.InnerText;
                            break;
                        case "MaintenanceDateTime":
                            objOrder.MaintenanceDateTime = DateTime.TryParse(objChildNode.InnerText, out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                        case "ImportDateTime":
                            objOrder.ImportDateTime = DateTime.TryParse(objChildNode.InnerText, out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                    }
                }

                this.Add(objOrder);
            }
        }

        public void Archive(string FileNameAndLocation)
        {
            XDocument objXDocument;
            var objXMLTextWriter = new XmlTextWriter(FileNameAndLocation, null);
            objXMLTextWriter.Formatting = Formatting.Indented;


            if (mIsFTP)
                objXDocument = XDocument.Parse(mStrBldrXML
                    .Insert(0, "<HaworthOrders>" + Environment.NewLine)
                    //.Insert(0, "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine)
                    .AppendLine("</HaworthOrders>").ToString());
            else
                objXDocument = XDocument.Parse(mStrBldrXML.ToString());

            objXDocument.Save(objXMLTextWriter);
            objXMLTextWriter.Close();
        }
    }
}