using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace DAL
{
    public class ExportShipmentDetailsDAL
    {
        #region  Export Shipment Details DAL
        /// <summary>
        /// This is Used to Insert/Update/Delete Data Into Table ExportShipmentDetails 
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="al"></param>
        /// <returns></returns>
        public static int InsertUPdateDelteExpShipDlts(List<string> parms, ArrayList al)//function to execute sp for Insert,Update,Delete of a record
        {
            return DAL.ExecuteSP("SP_ExportShipmentDetails", parms, al);
        }
        public static DataSet GetCommInv(List<string> parms, ArrayList al)//function to get data from ExportShipmentDetails
        {
            return DAL.GetDataSet("SP_ExportShipmentDetails", parms, al);
        }
        public static DataSet GetDataSet1(List<string> parms, ArrayList al)
        {
            return DAL.GetDataSet("SP_ExportShipmentDetails", parms, al);
        }
        public static DataSet GetDataSetDate(List<string> parms, ArrayList al)
        {
            return DAL.GetDataSet("SP_ExportShipmentDetails", parms, al);
        }
        #endregion
    }
}
