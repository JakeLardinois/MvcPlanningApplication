﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.ObjectModel;
using System.Text;
using MvcPlanningApplication.Models.DataTablesMVC;
using System.Linq.Dynamic;
using log4net;


namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthOrdersRepository
    {
        private static readonly ILog Logger = LogHelper.GetLogger();


        public IList<HaworthOrder> GetOrders(out int searchRecordCount, JQueryDataTablesModel DataTablesModel, bool isDownloadReport = false)
        {
            ReadOnlyCollection<SortedColumn> sortedColumns = DataTablesModel.GetSortedColumns();
            IEnumerable<HaworthOrder> orders;
            DateTime dtmTemp;
            int intTemp;
            string[] objResults;
            string strEmptyString = "EMPTY";
            StringBuilder objStrBldr = new StringBuilder();
            


            var objHaworthOrderSearch = new HaworthOrderSearch();
            for (int intCounter = 0; intCounter < DataTablesModel.iColumns; intCounter++)
            {

                if (DataTablesModel.bSearchable_[intCounter] == true && !string.IsNullOrEmpty(DataTablesModel.sSearch_[intCounter]))
                {
                    /*For some reason when I implemented resizable movable columns and would then move the columns in the application, the application would send tilde's in the 'checkbox' column types sSearch field which was wierd
                     since the checkbox column types are delimited by the pipe | character and the 'range' column types are delimited by the tilde...  The resolution that I came up with was to check if the only value passed in sSearch
                     was a tilde and if it was then skip the loop so that the respective VendorRequestSearch field was left null.*/
                    if (DataTablesModel.sSearch_[intCounter].Equals("~"))
                        continue;

                    /*Notice that i had to use mDataProp2_ due to datatables multi-column filtering not placing sSearch into proper array position when columns are reordered; See VendorRequestsController.cs Search method for details...*/
                    switch (DataTablesModel.mDataProp2_[intCounter])
                    {
                        case "Characteristics":
                            objHaworthOrderSearch.Characteristics = DataTablesModel.sSearch_[intCounter].Split('|');//results returned from a checklist are delimited by the pipe char
                            break;
                        case "ChangeDate":
                            objResults = DataTablesModel.sSearch_[intCounter].Split('~');//results returned from a daterange are delimited by the tilde char
                            objHaworthOrderSearch.ChangeDateGT = DateTime.TryParse(objResults[0], out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            objHaworthOrderSearch.ChangeDateLT = DateTime.TryParse(objResults[1], out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                        case "OrderNumber":
                            objStrBldr.Clear();
                            objStrBldr.Append(DataTablesModel.sSearch_[intCounter]);
                            objHaworthOrderSearch.OrderNumber = string.IsNullOrEmpty(objStrBldr.ToString()) ? strEmptyString : DataTablesModel.sSearch_[intCounter];
                            break;
                        case "ItemNumber":
                            objStrBldr.Clear();
                            objStrBldr.Append(DataTablesModel.sSearch_[intCounter]);
                            objHaworthOrderSearch.ItemNumber = string.IsNullOrEmpty(objStrBldr.ToString()) ? strEmptyString : DataTablesModel.sSearch_[intCounter];
                            break;
                        case "Description":
                            objStrBldr.Clear();
                            objStrBldr.Append(DataTablesModel.sSearch_[intCounter]);
                            objHaworthOrderSearch.Description = string.IsNullOrEmpty(objStrBldr.ToString()) ? strEmptyString : DataTablesModel.sSearch_[intCounter];
                            break;
                        case "Description2":
                            objStrBldr.Clear();
                            objStrBldr.Append(DataTablesModel.sSearch_[intCounter]);
                            objHaworthOrderSearch.Description2 = string.IsNullOrEmpty(objStrBldr.ToString()) ? strEmptyString : DataTablesModel.sSearch_[intCounter];
                            break;
                        case "ColorCode":
                            objStrBldr.Clear();
                            objStrBldr.Append(DataTablesModel.sSearch_[intCounter]);
                            objHaworthOrderSearch.ColorCode = string.IsNullOrEmpty(objStrBldr.ToString()) ? strEmptyString : DataTablesModel.sSearch_[intCounter];
                            break;
                        case "ColorPattern":
                            objStrBldr.Clear();
                            objStrBldr.Append(DataTablesModel.sSearch_[intCounter]);
                            objHaworthOrderSearch.ColorPattern = string.IsNullOrEmpty(objStrBldr.ToString()) ? strEmptyString : DataTablesModel.sSearch_[intCounter];
                            break;
                        case "ColorDescription":
                            objStrBldr.Clear();
                            objStrBldr.Append(DataTablesModel.sSearch_[intCounter]);
                            objHaworthOrderSearch.ColorDescription = string.IsNullOrEmpty(objStrBldr.ToString()) ? strEmptyString : DataTablesModel.sSearch_[intCounter];
                            break;
                        case "StatusCode":
                            objHaworthOrderSearch.StatusCode = DataTablesModel.sSearch_[intCounter].Split('|');//results returned from a checklist are delimited by the pipe char
                            break;
                        case "DockDate":
                            objResults = DataTablesModel.sSearch_[intCounter].Split('~');//results returned from a daterange are delimited by the tilde char
                            objHaworthOrderSearch.DockDateGT = DateTime.TryParse(objResults[0], out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            objHaworthOrderSearch.DockDateLT = DateTime.TryParse(objResults[1], out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                        case "ImportDateTime":
                            objResults = DataTablesModel.sSearch_[intCounter].Split('~');//results returned from a daterange are delimited by the tilde char
                            objHaworthOrderSearch.ImportDateTimeGT = DateTime.TryParse(objResults[0], out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            objHaworthOrderSearch.ImportDateTimeLT = DateTime.TryParse(objResults[1], out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                        default:
                            break;
                    }
                }
            }


            using (var db = new PlanningApplicationDb())
            {
                /*The Below was created because the Entity Framework had a problem doing a filter of a list with a list because of the difficulty it had using deferred execution and the corresponding sql creation*/
                var StatusCodeList = objHaworthOrderSearch.StatusCode == null ? new[] { strEmptyString } : objHaworthOrderSearch.StatusCode.ToArray<string>();
                var CharacteristicsList = objHaworthOrderSearch.Characteristics == null ? new[] { strEmptyString } : objHaworthOrderSearch.Characteristics.ToArray<string>();

                /*.Include("Characteristics") forces the Characteristics list to be populated ('eager' or 'greedy') since it is virtual and therefore 'lazy' by default. This wasn't an issue until I leveraged the
                 * 'Using' statement for my dbcontext; not including it resulted in a 'The ObjectContext instance has been disposed and can no longer be used for operations that require a connection' Error in my 
                 * Controller class ('HaworthController')*/
                orders = db.HaworthOrders.Include("Characteristics") 
                    .Where(c => CharacteristicsList.Contains(strEmptyString) || CharacteristicsList.Intersect(c.Characteristics.Select(n => n.CharacteristicKey)).Any())
                    .Where(c => c.ChangeDate >= objHaworthOrderSearch.ChangeDateGT || objHaworthOrderSearch.ChangeDateGT == DateTime.MinValue)
                    .Where(c => c.ChangeDate <= objHaworthOrderSearch.ChangeDateLT || objHaworthOrderSearch.ChangeDateLT == DateTime.MinValue)
                    .Where(c => string.IsNullOrEmpty(objHaworthOrderSearch.OrderNumber) || c.OrderNumber.ToUpper().Contains(objHaworthOrderSearch.OrderNumber.ToUpper()))
                    .Where(c => string.IsNullOrEmpty(objHaworthOrderSearch.ItemNumber) || c.ItemNumber.ToUpper().Contains(objHaworthOrderSearch.ItemNumber.ToUpper()))
                    .Where(c => string.IsNullOrEmpty(objHaworthOrderSearch.Description) || c.PartInformation.Description.ToUpper().Contains(objHaworthOrderSearch.Description.ToUpper()))
                    .Where(c => string.IsNullOrEmpty(objHaworthOrderSearch.Description2) || c.PartInformation.Description2.ToUpper().Contains(objHaworthOrderSearch.Description2.ToUpper()))
                    .Where(c => string.IsNullOrEmpty(objHaworthOrderSearch.ColorCode) || c.PartInformation.ColorCode.ToUpper().Contains(objHaworthOrderSearch.ColorCode.ToUpper()))
                    .Where(c => string.IsNullOrEmpty(objHaworthOrderSearch.ColorPattern) || c.PartInformation.ColorPattern.ToUpper().Contains(objHaworthOrderSearch.ColorPattern.ToUpper()))
                    .Where(c => string.IsNullOrEmpty(objHaworthOrderSearch.ColorDescription) || c.PartInformation.ColorDescription.ToUpper().Contains(objHaworthOrderSearch.ColorDescription.ToUpper()))
                    .Where(c => StatusCodeList.Contains(strEmptyString) || StatusCodeList.Contains(c.StatusCode))
                    .Where(c => c.DockDate >= objHaworthOrderSearch.DockDateGT || objHaworthOrderSearch.DockDateGT == DateTime.MinValue)
                    .Where(c => c.DockDate <= objHaworthOrderSearch.DockDateLT || objHaworthOrderSearch.DockDateLT == DateTime.MinValue)
                    .Where(c => c.ImportDateTime >= objHaworthOrderSearch.ImportDateTimeGT || objHaworthOrderSearch.ImportDateTimeGT == DateTime.MinValue)
                    .Where(c => c.ImportDateTime <= objHaworthOrderSearch.ImportDateTimeLT || objHaworthOrderSearch.ImportDateTimeLT == DateTime.MinValue)
                    .OrderBy(sortedColumns[0].PropertyName + " " + sortedColumns[0].Direction) //Uses Dynamic Linq to have sorting occur in the query
                    .ToList();


                //needed this to get the proper pagination values. by adding it here, i was hoping to optomize performance and still leverage deferred execution with the above queries
                // and the take values below...
                searchRecordCount = orders.Count();

                IEnumerable<HaworthOrder> obj;
                if (isDownloadReport)
                    obj = orders
                        .ToList();
                else
                    obj = orders
                        .Skip(DataTablesModel.iDisplayStart)
                        .Take(DataTablesModel.iDisplayLength)
                        .ToList();


                return obj.ToList();

            }
        }

    }
}