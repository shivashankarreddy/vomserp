using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using DAL;


namespace BAL
{
    public class ExportShipmentDetailsBLL
    {
        #region Export Shipment Details

        /// <summary>
        /// Insert/Update/Delete in ExportShipmentDetails
        /// </summary>
        /// <param name="Flag"></param>
        /// <param name="FnclYrID"></param>
        /// <param name="FnclrYear"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="iscurrentyear"></param>
        /// <param name="CreatedBy"></param>
        /// <returns></returns>
        public int InsertUpdateDeleteExpShipDlts(char Flag, Guid CommercialInvID, string CommercialInvNo, DateTime CommercailInvDate, string ConsigneeName, string BivacPreShiptInspDetails, string SuppCargoDesc, string PerfInvNo, DateTime PerfInvDate, string ModeofShpt, decimal FobValueUSD, decimal FreightIns, decimal CFRCIFValue, string PortofLoading, string PortofDisc, int Noofpkgs, decimal NetWeightinKgs, decimal GrossWeightinKgs, string ShippBillNo, DateTime DateofCargoCartatCFS, DateTime CustsExamStatus, string ContainerNo, DateTime ContainerStuffingDate, string VesselDetailsETAETD, string PartofBLAWB, string PartofECTNURNID, DateTime ECTNReqDate, DateTime ECTNInvRecDate, DateTime ECTNPayDate, DateTime ECTNNoRecDate, DateTime BLPayDate, DateTime BLAppDate, DateTime BLRelDate, DateTime BLRecDateAtHYD, string CommInvDetails, DateTime CerfofOriginFAPCCIDate, string StatusofCommInv, string DOCDetailsReqConsignee, DateTime
BLAWBapprecon, DateTime RFIFDIformreqon, DateTime RFIFDIformrecon, DateTime BIVACPreshipinspreqon, DateTime BIVACPreshipinspcomptdon, string ExpInvcourierdlts, DateTime BLInvRecDate, string ContStuffStat, string ETCNPayStat, string BLPayStat, string BLRelStat, DateTime AVCoCConsigneeon, string Remarks, Guid CompanyID, Guid CreatedBy, DateTime CreatedDate, Guid ModifiedBy, DateTime ModifiedDate, Boolean IsActive)
        {
            List<string> parms = new List<string> { "@Flag", "@CommercialInvID", "@CommercialInvNo", "@CommercailInvDate", "@ConsigneeName", "@BivacPreShiptInspDetails", "@SuppCargoDesc", "@PerfInvNo", "@PerfInvDate", "@ModeofShpt", "@FobValueUSD", "@FreightIns", "@CFRCIFValue", "@PortofLoading", "@PortofDisc", "@Noofpkgs", "@NetWeightinKgs", "@GrossWeightinKgs", "@ShippBillNo", "@DateofCargoCartatCFS", "@CustsExamStatus", "@ContainerNo", "@ContainerStuffingDate", "@VesselDetailsETAETD", "@PartofBLAWB", "@PartofECTNURNID", "@ECTNReqDate", "@ECTNInvRecDate", "@ECTNPayDate", "@ECTNNoRecDate", "@BLPayDate", "@BLAppDate", "@BLRelDate", "@BLRecDateAtHYD", "@CommInvDetails", "@CerfofOriginFAPCCIDate", "@StatusofCommInv", "@DOCDetailsReqConsignee", "@BLAWBapprecon", "@RFIFDIformreqon", "@RFIFDIformrecon", "@BIVACPreshipinspreqon", "@BIVACPreshipinspcomptdon", "@ExpInvcourierdlts", "@BLInvRecDate", "@ContStuffStat", "@ETCNPayStat", "@BLPayStat", "@BLRelStat", "@AVCoCConsigneeon", "@Remarks", "@CompanyID", "@CreatedBy", "@CreatedDate", "@ModifiedBy", "@ModifiedDate", "@IsActive" };
            ArrayList al = new ArrayList();
            al.Add(Flag);//Flag
            al.Add(CommercialInvID);//ID
            al.Add(CommercialInvNo);
            al.Add(CommercailInvDate);
            al.Add(ConsigneeName);
            al.Add(BivacPreShiptInspDetails);
            al.Add(SuppCargoDesc);
            al.Add(PerfInvNo);
            al.Add(PerfInvDate);
            al.Add(ModeofShpt);
            al.Add(FobValueUSD);
            al.Add(FreightIns);
            al.Add(CFRCIFValue);
            al.Add(PortofLoading);
            al.Add(PortofDisc);
            al.Add(Noofpkgs);
            al.Add(NetWeightinKgs);
            al.Add(GrossWeightinKgs);
            al.Add(ShippBillNo);
            al.Add(DateofCargoCartatCFS);
            al.Add(CustsExamStatus);
            al.Add(ContainerNo);
            al.Add(ContainerStuffingDate);
            al.Add(VesselDetailsETAETD);
            al.Add(PartofBLAWB);
            al.Add(PartofECTNURNID);
            al.Add(ECTNReqDate);
            al.Add(ECTNInvRecDate);
            al.Add(ECTNPayDate);
            al.Add(ECTNNoRecDate);
            al.Add(BLPayDate);
            al.Add(BLAppDate);
            al.Add(BLRelDate);
            al.Add(BLRecDateAtHYD);
            al.Add(CommInvDetails);
            al.Add(CerfofOriginFAPCCIDate);
            al.Add(StatusofCommInv);
            al.Add(DOCDetailsReqConsignee);
            al.Add(BLAWBapprecon);
            al.Add(RFIFDIformreqon);
            al.Add(RFIFDIformrecon);
            al.Add(BIVACPreshipinspreqon);
            al.Add(BIVACPreshipinspcomptdon);
            al.Add(ExpInvcourierdlts);
            al.Add(BLInvRecDate);
            al.Add(ContStuffStat);
            al.Add(ETCNPayStat);
            al.Add(BLPayStat);
            al.Add(BLRelStat);
            al.Add(AVCoCConsigneeon);
            al.Add(Remarks);
            al.Add(CompanyID);
            al.Add(CreatedBy);
            al.Add(CreatedDate);
            al.Add(ModifiedBy);
            al.Add(ModifiedDate);
            al.Add(IsActive);
            return ExportShipmentDetailsDAL.InsertUPdateDelteExpShipDlts(parms, al);
        }
        public DataSet GetCommInvData(char Flag)
        {
            List<string> parms = new List<string> { "@Flag", "@CommercialInvID", "@CommercialInvNo", "@CommercailInvDate", "@ConsigneeName", "@BivacPreShiptInspDetails", "@SuppCargoDesc", "@PerfInvNo", "@PerfInvDate", "@ModeofShpt", "@FobValueUSD", "@FreightIns", "@CFRCIFValue", "@PortofLoading", "@PortofDisc", "@Noofpkgs", "@NetWeightinKgs", "@GrossWeightinKgs", "@ShippBillNo", "@DateofCargoCartatCFS", "@CustsExamStatus", "@ContainerNo", "@ContainerStuffingDate", "@VesselDetailsETAETD", "@PartofBLAWB", "@PartofECTNURNID", "@ECTNReqDate", "@ECTNInvRecDate", "@ECTNPayDate", "@ECTNNoRecDate", "@BLPayDate", "@BLAppDate", "@BLRelDate", "@BLRecDateAtHYD", "@CommInvDetails", "@CerfofOriginFAPCCIDate", "@StatusofCommInv", "@DOCDetailsReqConsignee", "@BLAWBapprecon", "@RFIFDIformreqon", "@RFIFDIformrecon", "@BIVACPreshipinspreqon", "@BIVACPreshipinspcomptdon", "@ExpInvcourierdlts", "@BLInvRecDate", "@ContStuffStat", "@ETCNPayStat", "@BLPayStat", "@BLRelStat", "@AVCoCConsigneeon", "@Remarks", "@CompanyID", "@CreatedBy", "@CreatedDate", "@ModifiedBy", "@ModifiedDate", "@IsActive" };
            ArrayList al = new ArrayList();
            al.Add(Flag);//Flag
            al.Add(Guid.Empty);//ID
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add(0.00);
            al.Add(0.00);
            al.Add(0.00);
            al.Add("");
            al.Add("");
            al.Add(0);
            al.Add(0.00);
            al.Add(0.00);
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            return ExportShipmentDetailsDAL.GetCommInv(parms, al);
        }
        public DataSet GetDataSet1(char Flag, Guid CommercialInvID)
        {
            List<string> parms = new List<string> { "@Flag", "@CommercialInvID", "@CommercialInvNo","@CommercailInvDate", "@ConsigneeName", "@BivacPreShiptInspDetails", "@SuppCargoDesc",
                "@PerfInvNo", "@PerfInvDate", "@ModeofShpt", "@FobValueUSD", "@FreightIns", "@CFRCIFValue", "@PortofLoading", "@PortofDisc", "@Noofpkgs", "@NetWeightinKgs", "@GrossWeightinKgs", 
                "@ShippBillNo", "@DateofCargoCartatCFS", "@CustsExamStatus", "@ContainerNo", "@ContainerStuffingDate", 
                "@VesselDetailsETAETD", "@PartofBLAWB", "@PartofECTNURNID", "@ECTNReqDate", "@ECTNInvRecDate", "@ECTNPayDate", "@ECTNNoRecDate", 
                "@BLPayDate", "@BLAppDate", "@BLRelDate", "@BLRecDateAtHYD", "@CommInvDetails", "@CerfofOriginFAPCCIDate", "@StatusofCommInv",
                "@DOCDetailsReqConsignee", "@BLAWBapprecon", "@RFIFDIformreqon", "@RFIFDIformrecon", "@BIVACPreshipinspreqon", 
                "@BIVACPreshipinspcomptdon", "@ExpInvcourierdlts", "@BLInvRecDate", "@ContStuffStat", "@ETCNPayStat", "@BLPayStat", "@BLRelStat", "@AVCoCConsigneeon","@Remarks" };
            ArrayList al = new ArrayList();
            al.Add(Flag);//Flag
            al.Add(CommercialInvID);//CommercialInvID
            al.Add("");//CommercialInvDate
            al.Add("");//CommercialInvNo
            al.Add("");//ConsigneeName
            al.Add("");//BivacPreShiptInspDetails
            al.Add("");//SuppCargoDesc
            al.Add("");//PerfInvNo
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//PerfInvDate
            al.Add("");//ModeofShpt
            al.Add(0.00);//FobValueUSD
            al.Add(0.00);//FreightIns
            al.Add(0.00);//CFRCIFValue
            al.Add("");//PortofLoading
            al.Add("");//PortofDisc
            al.Add(0);//Noofpkgs
            al.Add(0.00);//NetWeightinKgs
            al.Add(0.00);//GrossWeightinKgs
            al.Add("");//ShippBillNo
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//DateofCargoCartatCFS
            al.Add("");//CustsExamStatus
            al.Add("");//ContainerNo
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ContainerStuffingDate
            al.Add("");//VesselDetailsETAETD
            al.Add("");//PartofBLAWB
            al.Add("");//PartofECTNURNID
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ECTNReqDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ECTNInvRecDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ECTNPayDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ECTNNoRecDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//BLPayDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//BLAppDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//BLRelDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//BLRecDateAtHYD
            al.Add("");//CommInvDetails
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//CerfofOriginFAPCCIDate
            al.Add("");//StatusofCommInv
            al.Add("");//DOCDetailsReqConsignee
            al.Add("");//BLAWBapprecon
            al.Add("");//RFIFDIformreqon
            al.Add("");//RFIFDIformrecon
            al.Add("");//BIVACPreshipinspreqon
            al.Add("");//BIVACPreshipinspcomptdon
            al.Add("");//ExpInvcourierdlts
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))); //BLInvRecDate
            al.Add("");//ContStuffStat
            al.Add("");//ETCNPayStat
            al.Add("");//BLPayStat
            al.Add("");//BLRelStat
            al.Add("");//Remarks
            al.Add("");
            return ExportShipmentDetailsDAL.GetDataSet1(parms, al);
        }
        public DataSet GetExportDetailsNumber(char Flag)
        {
            List<string> parms = new List<string> { "@Flag", "@ExpShipDtlsID", "@CommercialInvID", "@CommercialInvNo", "@CommercailInvDate", "@ConsigneeName", "@BivacPreShiptInspDetails", "@SuppCargoDesc", "@PerfInvNo", "@PerfInvDate", "@ModeofShpt", "@FobValueUSD", "@FreightIns", "@CFRCIFValue", "@PortofLoading", "@PortofDisc", "@Noofpkgs", "@NetWeightinKgs", "@GrossWeightinKgs", "@ShippBillNo", "@DateofCargoCartatCFS", "@CustsExamStatus", "@ContainerNo", "@ContainerStuffingDate", "@VesselDetailsETAETD", "@PartofBLAWB", "@PartofECTNURNID", "@ECTNReqDate", "@ECTNInvRecDate", "@ECTNPayDate", "@ECTNNoRecDate", "@BLPayDate", "@BLAppDate", "@BLRelDate", "@BLRecDateAtHYD", "@CommInvDetails", "@CerfofOriginFAPCCIDate", "@StatusofCommInv", "@DOCDetailsReqConsignee", "@BLAWBapprecon", "@RFIFDIformreqon", "@RFIFDIformrecon", "@BIVACPreshipinspreqon", "@BIVACPreshipinspcomptdon", "@ExpInvcourierdlts", "@BLInvRecDate", "@ContStuffStat", "@ETCNPayStat", "@BLPayStat", "@BLRelStat", "@AVCoCConsigneeon", "@Remarks", "@CompanyID", "@CreatedBy", "@CreatedDate", "@ModifiedBy", "@ModifiedDate", "@IsActive" };
            ArrayList al = new ArrayList();
            al.Add(Flag);//Flag
            al.Add(Guid.Empty);//ID
            al.Add(Guid.Empty);//ID
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add(0.00);
            al.Add(0.00);
            al.Add(0.00);
            al.Add("");
            al.Add("");
            al.Add(0);
            al.Add(0.00);
            al.Add(0.00);
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            al.Add("");
            return ExportShipmentDetailsDAL.GetCommInv(parms, al);
        }
        public DataSet GetDetailsExportExcel(char Flag, Guid ExpShipDtlsID)
        {
            List<string> parms = new List<string> { "@Flag", "@ExpShipDtlsID", "@CommercialInvNo", "@ConsigneeName", "@BivacPreShiptInspDetails", "@SuppCargoDesc",
                "@PerfInvNo", "@PerfInvDate", "@ModeofShpt", "@FobValueUSD", "@FreightIns", "@CFRCIFValue", "@PortofLoading", "@PortofDisc", "@Noofpkgs", "@NetWeightinKgs", "@GrossWeightinKgs", 
                "@ShippBillNo", "@DateofCargoCartatCFS", "@CustsExamStatus", "@ContainerNo", "@ContainerStuffingDate", 
                "@VesselDetailsETAETD", "@PartofBLAWB", "@PartofECTNURNID", "@ECTNReqDate", "@ECTNInvRecDate", "@ECTNPayDate", "@ECTNNoRecDate", 
                "@BLPayDate", "@BLAppDate", "@BLRelDate", "@BLRecDateAtHYD", "@CommInvDetails", "@CerfofOriginFAPCCIDate", "@StatusofCommInv",
                "@DOCDetailsReqConsignee", "@BLAWBapprecon", "@RFIFDIformreqon", "@RFIFDIformrecon", "@BIVACPreshipinspreqon", 
                "@BIVACPreshipinspcomptdon", "@ExpInvcourierdlts", "@BLInvRecDate", "@ContStuffStat", "@ETCNPayStat", "@BLPayStat", "@BLRelStat", "@Remarks", "@CreatedBy", "@CreatedDate", "@ModifiedBy", "@ModifiedDate", "@IsActive"};
            ArrayList al = new ArrayList();
            al.Add(Flag);//Flag
            al.Add(ExpShipDtlsID);//CommercialInvID
            al.Add("");//CommercialInvNo
            al.Add("");//ConsigneeName
            al.Add("");//BivacPreShiptInspDetails
            al.Add("");//SuppCargoDesc
            al.Add("");//PerfInvNo
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//PerfInvDate
            al.Add("");//ModeofShpt
            al.Add(0.00);//FobValueUSD
            al.Add(0.00);//FreightIns
            al.Add(0.00);//CFRCIFValue
            al.Add("");//PortofLoading
            al.Add("");//PortofDisc
            al.Add(0);//Noofpkgs
            al.Add(0.00);//NetWeightinKgs
            al.Add(0.00);//GrossWeightinKgs
            al.Add("");//ShippBillNo
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//DateofCargoCartatCFS
            al.Add("");//CustsExamStatus
            al.Add("");//ContainerNo
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ContainerStuffingDate
            al.Add("");//VesselDetailsETAETD
            al.Add("");//PartofBLAWB
            al.Add("");//PartofECTNURNID
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ECTNReqDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ECTNInvRecDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ECTNPayDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ECTNNoRecDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//BLPayDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//BLAppDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//BLRelDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//BLRecDateAtHYD
            al.Add("");//CommInvDetails
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//CerfofOriginFAPCCIDate
            al.Add("");//StatusofCommInv
            al.Add("");//DOCDetailsReqConsignee
            al.Add("");//BLAWBapprecon
            al.Add("");//RFIFDIformreqon
            al.Add("");//RFIFDIformrecon
            al.Add("");//BIVACPreshipinspreqon
            al.Add("");//BIVACPreshipinspcomptdon
            al.Add("");//ExpInvcourierdlts
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))); //BLInvRecDate
            al.Add("");//ContStuffStat
            al.Add("");//ETCNPayStat
            al.Add("");//BLPayStat
            al.Add("");//BLRelStat
            al.Add("");//Remarks
            al.Add(Guid.Empty);//CreatedBy
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//CreatedDate
            al.Add(Guid.Empty);//ModifiedBy
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ModifiedDate
            al.Add(true);//is IsActive
            return ExportShipmentDetailsDAL.GetDataSet1(parms, al);
        }
        public DataSet GetDataSetDate(char Flag, DateTime FromDate, DateTime ToDate)
        {
            List<string> parms = new List<string> { "@Flag", "@ExpShipDtlsID","@FromDate","@ToDate", "@CommercialInvNo", "@ConsigneeName", "@BivacPreShiptInspDetails", "@SuppCargoDesc",
                "@PerfInvNo", "@PerfInvDate", "@ModeofShpt", "@FobValueUSD", "@FreightIns", "@CFRCIFValue", "@PortofLoading", "@PortofDisc", "@Noofpkgs", "@NetWeightinKgs", "@GrossWeightinKgs", 
                "@ShippBillNo", "@DateofCargoCartatCFS", "@CustsExamStatus", "@ContainerNo", "@ContainerStuffingDate", 
                "@VesselDetailsETAETD", "@PartofBLAWB", "@PartofECTNURNID", "@ECTNReqDate", "@ECTNInvRecDate", "@ECTNPayDate", "@ECTNNoRecDate", 
                "@BLPayDate", "@BLAppDate", "@BLRelDate", "@BLRecDateAtHYD", "@CommInvDetails", "@CerfofOriginFAPCCIDate", "@StatusofCommInv",
                "@DOCDetailsReqConsignee", "@BLAWBapprecon", "@RFIFDIformreqon", "@RFIFDIformrecon", "@BIVACPreshipinspreqon", 
                "@BIVACPreshipinspcomptdon", "@ExpInvcourierdlts", "@BLInvRecDate", "@ContStuffStat", "@ETCNPayStat", "@BLPayStat", "@BLRelStat", "@Remarks", "@CreatedBy", "@CreatedDate", "@ModifiedBy", "@ModifiedDate", "@IsActive"};
            ArrayList al = new ArrayList();
            al.Add(Flag);//Flag
            al.Add(Guid.Empty);//CommercialInvID
            al.Add(FromDate);
            al.Add(ToDate);
            al.Add("");//CommercialInvNo
            al.Add("");//ConsigneeName
            al.Add("");//BivacPreShiptInspDetails
            al.Add("");//SuppCargoDesc
            al.Add("");//PerfInvNo
            //al.Add(pinvDt);//PerfInvDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//PerfInvDate
            al.Add("");//ModeofShpt
            al.Add(0.00);//FobValueUSD
            al.Add(0.00);//FreightIns
            al.Add(0.00);//CFRCIFValue
            al.Add("");//PortofLoading
            al.Add("");//PortofDisc
            al.Add(0);//Noofpkgs
            al.Add(0.00);//NetWeightinKgs
            al.Add(0.00);//GrossWeightinKgs
            al.Add("");//ShippBillNo
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//DateofCargoCartatCFS
            al.Add("");//CustsExamStatus
            al.Add("");//ContainerNo
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ContainerStuffingDate
            al.Add("");//VesselDetailsETAETD
            al.Add("");//PartofBLAWB
            al.Add("");//PartofECTNURNID
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ECTNReqDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ECTNInvRecDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ECTNPayDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ECTNNoRecDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//BLPayDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//BLAppDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//BLRelDate
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//BLRecDateAtHYD
            al.Add("");//CommInvDetails
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//CerfofOriginFAPCCIDate
            al.Add("");//StatusofCommInv
            al.Add("");//DOCDetailsReqConsignee
            al.Add("");//BLAWBapprecon
            al.Add("");//RFIFDIformreqon
            al.Add("");//RFIFDIformrecon
            al.Add("");//BIVACPreshipinspreqon
            al.Add("");//BIVACPreshipinspcomptdon
            al.Add("");//ExpInvcourierdlts
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))); //BLInvRecDate
            al.Add("");//ContStuffStat
            al.Add("");//ETCNPayStat
            al.Add("");//BLPayStat
            al.Add("");//BLRelStat
            al.Add("");//Remarks
            al.Add(Guid.Empty);//CreatedBy
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//CreatedDate
            al.Add(Guid.Empty);//ModifiedBy
            al.Add(CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")));//ModifiedDate
            al.Add(true);//is IsActive
            return ExportShipmentDetailsDAL.GetDataSetDate(parms, al);
        }

    }
        #endregion
}
