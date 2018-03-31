﻿using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using GvkFMSAPP.BLL;
using GvkFMSAPP.PL;

public partial class FuelEntry : Page
{
    public IFuelManagement ObjFuelEntry = new FuelManagement();
    double _kmplInt = 0;
    double _mSkmplInt = 0;
    bool _flag;
    string _bunkname;
    readonly GvkFMSAPP.BLL.FMSGeneral _fmsg = new GvkFMSAPP.BLL.FMSGeneral();
    protected void Page_PreInit(Object sender, EventArgs e)
    {

        if (Session["Role_Id"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        else
        {
            if (Session["Role_Id"].ToString() == "120")
            {
                this.MasterPageFile = "~/MasterERO.master";
            }
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

        if (Session["User_Name"] == null)
        {
            Response.Redirect("Error.aspx");
        }

        if (!IsPostBack)
        {

            if (Session["UserdistrictId"] != null)
            {
                _fmsg.UserDistrictId = Convert.ToInt32(Session["UserdistrictId"].ToString());
            }

            linkExisting.Visible = false;
            lnkNew.Visible = true;
            txtBunkName.Visible = true;
            txtBunkName.Enabled = false;
            ddlBunkName.Visible = false;
            FillVehicles();
            FillPayMode();

            //FillGridFuelEntry();
            txtAmount.Attributes.Add("onkeypress", "javascript:return isNumberKey(event)");
            txtBillNumber.Attributes.Add("onkeypress", "javascript:return isNumberKey(event)");
            //txtBunkName.Attributes.Add("onkeypress", "javascript:return OnlyAlphabets(this,event)");

            txtLocation.Attributes.Add("onkeypress", "javascript:return OnlyAlphabets(this,event)");
            txtOdometer.Attributes.Add("onkeypress", "javascript:return isNumberKey(event)");
            //txtQuantity.Attributes.Add("onkeypress", "javascript:return isNumberKey(this,event)");
            //txtUnitPrice.Attributes.Add("onkeypress", "javascript:return isDecimalNumberKey(event)");
            txtLocation.Attributes.Add("onkeypress", "javascript:return OnlyAlphabets(this,event)");
            txtPilotID.Attributes.Add("onkeypress", "javascript:return isNumberKey(event)");
            txtPilotName.Attributes.Add("onkeypress", "javascript:return OnlyAlphabets(this,event)");

            DataSet dsPerms = (DataSet)Session["PermissionsDS"];
            dsPerms.Tables[0].DefaultView.RowFilter = "Url='" + Page.Request.Url.Segments[Page.Request.Url.Segments.Length - 1] + "'";
            PagePermissions p = new PagePermissions(dsPerms, dsPerms.Tables[0].DefaultView[0]["Url"].ToString(), dsPerms.Tables[0].DefaultView[0]["Title"].ToString());

            if (p.Add)
            {

            }
            if (p.Modify)
            {
            }
            if (p.View)
            {
            }
            if (p.Approve)
            {
            }

        }
    }

    private void FillDistricts()
    {
        int districtId = -1;

        if (Session["UserdistrictId"] != null)
        {
            districtId = Convert.ToInt32(Session["UserdistrictId"].ToString());
        }

        var ds = ObjFuelEntry.IFillVehiclesWithMappedCards(districtId);
        ddlDistrict.DataSource = ds.Tables[0];
        ddlDistrict.DataValueField = "VehicleID";
        ddlDistrict.DataTextField = "VehicleNumber";
        ddlDistrict.DataBind();
        ddlDistrict.Items.Insert(0, "--Select--");
        ddlDistrict.Items[0].Value = "0";
        ddlDistrict.SelectedIndex = 0;

        ListItem itemToRemove = ddlDistrict.Items.FindByValue(ddlVehicleNumber.SelectedValue);
        if (itemToRemove != null)
        {
            ddlDistrict.Items.Remove(itemToRemove);
        }


        ddlDistrict.Enabled = true;
    }

    protected void ddlDistrict_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillCardNumber(Convert.ToInt32(ddlDistrict.SelectedValue));
        ddlPetroCardNumber.Enabled = false;
    }
    //Shiva...GetVehicleNumber() method
    private void FillVehicles()
    {
        if (Session["UserdistrictId"] != null)
        {
        }
        DataSet ds = _fmsg.GetVehicleNumber();
        //ds = objFuelEntry.IFillVehicles(districtID);
        ddlVehicleNumber.DataSource = ds.Tables[0];
        ddlVehicleNumber.DataValueField = "VehicleID";
        ddlVehicleNumber.DataTextField = "VehicleNumber";
        ddlVehicleNumber.DataBind();
        ddlVehicleNumber.Items.Insert(0, "--Select--");
        ddlVehicleNumber.Items[0].Value = "0";
        ddlVehicleNumber.SelectedIndex = 0;
        ddlVehicleNumber.Enabled = true;

    }


    private void FillDistrictLocation()
    {
        _fmsg.vehicle = (ddlVehicleNumber.SelectedItem.ToString());
        DataSet dsDistrict = _fmsg.GetDistrictLoc();
        lblDistrict.Text = dsDistrict.Tables[0].Rows[0]["District"].ToString();
        lblLocation.Text = dsDistrict.Tables[0].Rows[0]["BaseLocation"].ToString();

    }

    private void FillServiceStn()
    {
        _fmsg.ServiceStn = lblDistrict.Text;
        DataSet dsServiceStn = _fmsg.GetServiceStns();
        ddlBunkName.DataSource = dsServiceStn.Tables[0];
        ddlBunkName.DataValueField = "Id";
        ddlBunkName.DataTextField = "ServiceStnName";
        ddlBunkName.DataBind();
        ddlBunkName.Items.Insert(0, "--Select--");
        ddlBunkName.Items[0].Value = "0";
        ddlBunkName.SelectedIndex = 0;

        ddlBunkName.Enabled = true;
    }

    private void FillServiceStnVeh()
    {
        _fmsg.ServiceStnVeh = Convert.ToInt32(ddlVehicleNumber.SelectedValue);
        DataSet dsServiceStn = _fmsg.GetServiceStnsVeh();
        if (dsServiceStn.Tables[0].Rows.Count != 0)
        {
            txtBunkName.Text = Convert.ToString(dsServiceStn.Tables[0].Rows[0][1]);
        }
        else
        {
            txtBunkName.Enabled = true;
        }

    }



    protected void ddlVehicleNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillCardNumber(Convert.ToInt32(ddlVehicleNumber.SelectedValue));
        //FillGridLastFiveTransactions(Convert.ToInt32(ddlVehicleNumber.SelectedValue));
        FillGridFuelEntry(Convert.ToInt32(ddlVehicleNumber.SelectedValue));
        ViewState["VehicleID"] = ddlVehicleNumber.SelectedValue;

        ddlPetroCardNumber.Enabled = false;


        var dsOdo = ObjFuelEntry.ICheckFuelEntryOdo(Convert.ToInt32(ddlVehicleNumber.SelectedValue));
        if (dsOdo.Tables[0].Rows.Count != 0)
        {
            maxOdo.Value = dsOdo.Tables[0].Rows[0]["ODO"].ToString() != string.Empty ? dsOdo.Tables[0].Rows[0]["ODO"].ToString() : "0";
        }
        else
        {
            maxOdo.Value = "0";
        }
        FillDistricts();
        FillDistrictLocation();
        FillServiceStnVeh();
    }

    private void FillPayMode()
    {
        var ds = ObjFuelEntry.IFillPayMode();
        ddlPaymode.DataSource = ds.Tables[0];
        ddlPaymode.DataValueField = "PayModeID";
        ddlPaymode.DataTextField = "PayMode";
        ddlPaymode.DataBind();
        ddlPaymode.Items.Insert(0, "--Select--");
        ddlPaymode.Items[0].Value = "0";
        ddlPaymode.SelectedIndex = 0;

        ddlPaymode.Enabled = true;
    }




    protected void ddlPaymode_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlPaymode.SelectedValue == "1")
        {

            ddlPetroCardNumber.Enabled = true;
            ddlCardSwiped.Enabled = true;

        }

        else
        {
            ddlPetroCardNumber.Enabled = false;
            ddlAgency.Enabled = false;
            ddlCardSwiped.SelectedIndex = 1;
            ddlCardSwiped.Enabled = false;
        }
    }




    private void FillCardNumber(int vehicleId)
    {
        DataSet ds = new DataSet();
        ds = ObjFuelEntry.IFillCardNumber(vehicleId);
        if (ds.Tables[0].Rows.Count == 0)
        {
            string strFmsScript = "No cards mapped to this Vehicle";
            Show(strFmsScript);
            ddlPaymode.SelectedIndex = 2;
            ddlPaymode.Enabled = false;
            ddlCardSwiped.SelectedIndex = 1;
            ddlCardSwiped.Enabled = false;
            ddlPetroCardNumber.SelectedIndex = -1;

        }
        else
        {
            ddlPetroCardNumber.DataSource = ds.Tables[0];
            ddlPetroCardNumber.DataValueField = "PetroCardIssueID";
            ddlPetroCardNumber.DataTextField = "PetroCardNum";
            ddlPetroCardNumber.DataBind();
            ddlPetroCardNumber.Items.Insert(0, "--Select--");
            if (ds.Tables[0].Rows.Count > 0)
            {

            }
            ddlPaymode.Enabled = true;
        }
    }




    protected void ddlPetroCardNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillFuelAgency(Convert.ToInt32(ddlPetroCardNumber.SelectedValue));
    }

    private void FillFuelAgency(int petroCardIssueId)
    {
        var ds = ObjFuelEntry.IFillFuelAgency(petroCardIssueId);
        ddlAgency.DataSource = ds.Tables[0];
        ddlAgency.DataValueField = "AgencyID";
        ddlAgency.DataTextField = "AgencyName";
        ddlAgency.DataBind();
        ddlAgency.Items.Insert(0, "--Select--");
        if (ds.Tables[0].Rows.Count > 0)
        {
            //   ddlAgency.SelectedIndex = 1;
            //   ddlCardSwiped.SelectedIndex = 2;
        }
        ddlAgency.Enabled = true;
    }

    protected void Save_Click(object sender, EventArgs e)
    {
        var dsOdo = ObjFuelEntry.ICheckFuelEntryOdo(Convert.ToInt32(ddlVehicleNumber.SelectedValue));
        if (dsOdo.Tables[0].Rows.Count != 0)
        {
            if (dsOdo.Tables[0].Rows[0]["ODO"].ToString() != string.Empty)
            {
                maxOdo.Value = dsOdo.Tables[0].Rows[0]["ODO"].ToString();
                ViewState["maxodometer"] = dsOdo.Tables[0].Rows[0]["ODO"].ToString();
            }
            else
            {
                maxOdo.Value = "0";
            }
        }
        else
        {
            maxOdo.Value = "0";
        }
        // Show(txtFuelEntryDate.Text);
        DateTime entrydate = DateTime.ParseExact(txtFuelEntryDate.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture);

        if (entrydate > DateTime.Now)
        {
            Show("Fuel entry date should not be greater than current date ");
            return;
        }

        Save.Enabled = false;
        GvkFMSAPP.BLL.FMSGeneral fmsGeneral = new GvkFMSAPP.BLL.FMSGeneral();

        DataSet ds = fmsGeneral.GetRegistrationDate(int.Parse(ddlVehicleNumber.SelectedItem.Value));
        Save.Enabled = true;
        if (ds.Tables[0].Rows.Count == 0)
        {
            Show("Fuel Entry cannot be done as vehicle is not yet Registered");
        }
        else
        {
            if (txtOdometer.Text.Trim() != string.Empty)
            {
                if (Convert.ToInt32(ViewState["maxodometer"].ToString()) != 0)
                {
                    int maxno = Convert.ToInt32(ViewState["maxodometer"].ToString()) + 1000;
                    if (maxno <= Convert.ToInt32(txtOdometer.Text) || Convert.ToInt32(txtOdometer.Text) <= Convert.ToInt32(ViewState["maxodometer"].ToString()))
                    {

                        Show("Odo value between  " + ViewState["maxodometer"].ToString() + " And " + maxno);
                        txtOdometer.Text = "";
                        txtOdometer.Focus();
                        return;
                    }
                }


            }
            else
            {
                Show("Enter Odo value");
                return;
            }
            // Show(ds.Tables[0].Rows[0]["RegDate"].ToString());
            DateTime dtofRegistration = DateTime.ParseExact(ds.Tables[0].Rows[0]["RegDate"].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
            DateTime fuelEntry = DateTime.ParseExact(txtFuelEntryDate.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            if (dtofRegistration > fuelEntry)
            {
                Show("Fuel entry date should be greater than date of registration ");
                return;
            }

            var dtpreviousentryDate = getpreviousODO(int.Parse(ddlVehicleNumber.SelectedItem.Value));
            if (dtpreviousentryDate.Rows.Count > 0)
            {
                if (dtpreviousentryDate.Rows[0]["maxentry"].ToString() != "")
                {
                    DateTime dtprvrefill = Convert.ToDateTime(dtpreviousentryDate.Rows[0]["maxentry"].ToString());
                    if (dtprvrefill > DateTime.ParseExact(txtFuelEntryDate.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture))
                    {
                        Show("Fuel entry date must be greater than previous fuel entry date");
                        return;
                    }
                }
            }
            //Shiva end
            Save.Enabled = false;
            if (Save.Text == "Save" && ddlPetroCardNumber.Enabled)
            {
                _bunkname = ddlBunkName.Visible ? ddlBunkName.SelectedItem.Text : txtBunkName.Text;

                InsFuelEntry(Convert.ToInt32(Session["UserdistrictId"].ToString()),
                    Convert.ToInt32(ddlVehicleNumber.SelectedValue), Convert.ToInt32(ddlDistrict.SelectedValue),
                    fuelEntry, Convert.ToInt64(txtBillNumber.Text), Convert.ToInt64(txtOdometer.Text), _bunkname,
                    Convert.ToInt32(ddlPaymode.SelectedValue), Convert.ToDecimal(txtQuantity.Text),
                    Convert.ToInt64(ddlPetroCardNumber.SelectedValue), Convert.ToDecimal(txtUnitPrice.Text),
                    Convert.ToInt32(ddlAgency.SelectedValue), Convert.ToString(txtLocation.Text),
                    Convert.ToInt32(Session["User_Id"].ToString()), DateTime.Now, 1, Convert.ToDecimal(txtAmount.Text),
                    Convert.ToInt32(txtPilotID.Text), Convert.ToString(txtPilotName.Text),
                    Convert.ToInt32(ddlCardSwiped.SelectedValue), Convert.ToString(txtRemarks.Text));

                FillGridFuelEntry(Convert.ToInt32(ViewState["VehicleID"]));
            }
            else if (Save.Text == "Save" && ddlPetroCardNumber.Enabled == false)
            {
                _bunkname = ddlBunkName.Visible ? ddlBunkName.SelectedItem.Text : txtBunkName.Text;

                InsFuelEntry1(Convert.ToInt32(Session["UserdistrictId"].ToString()),
                    Convert.ToInt32(ddlVehicleNumber.SelectedValue), fuelEntry, Convert.ToInt64(txtBillNumber.Text),
                    Convert.ToInt64(txtOdometer.Text), _bunkname, Convert.ToInt32(ddlPaymode.SelectedValue),
                    Convert.ToDecimal(txtQuantity.Text), Convert.ToDecimal(txtUnitPrice.Text),
                    Convert.ToString(txtLocation.Text), Convert.ToInt32(Session["User_Id"].ToString()), DateTime.Now, 1,
                    Convert.ToDecimal(txtAmount.Text), Convert.ToInt32(txtPilotID.Text),
                    Convert.ToString(txtPilotName.Text), Convert.ToInt32(ddlCardSwiped.SelectedValue),
                    Convert.ToString(txtRemarks.Text));

                FillGridFuelEntry(Convert.ToInt32(ViewState["VehicleID"]));
            }
            else if (Save.Text == "Update" && ddlPetroCardNumber.Enabled)
            {
                _bunkname = ddlBunkName.Visible ? ddlBunkName.SelectedItem.Text : txtBunkName.Text;
                UpdFuelEntry(Convert.ToInt32(txtEdit.Text), Convert.ToInt32(Session["UserdistrictId"].ToString()),
                    Convert.ToInt32(ddlVehicleNumber.SelectedValue), Convert.ToInt32(ddlDistrict.SelectedValue),
                    fuelEntry, Convert.ToInt64(txtBillNumber.Text), Convert.ToInt64(txtOdometer.Text), _bunkname,
                    Convert.ToInt32(ddlPaymode.SelectedValue), Convert.ToDecimal(txtQuantity.Text),
                    Convert.ToInt64(ddlPetroCardNumber.SelectedValue), Convert.ToDecimal(txtUnitPrice.Text),
                    Convert.ToInt32(ddlAgency.SelectedValue), Convert.ToString(txtLocation.Text),
                    Convert.ToDecimal(txtAmount.Text), Convert.ToInt32(txtPilotID.Text),
                    Convert.ToString(txtPilotName.Text), Convert.ToInt32(ddlCardSwiped.SelectedValue),
                    Convert.ToString(txtRemarks.Text));
                FillGridFuelEntry(Convert.ToInt32(ViewState["VehicleID"]));
            }
            else
            {
                _bunkname = ddlBunkName.Visible ? ddlBunkName.SelectedItem.Text : txtBunkName.Text;
                UpdFuelEntry1(Convert.ToInt32(txtEdit.Text), Convert.ToInt32(Session["UserdistrictId"].ToString()),
                    Convert.ToInt32(ddlVehicleNumber.SelectedValue), fuelEntry, Convert.ToInt64(txtBillNumber.Text),
                    Convert.ToInt64(txtOdometer.Text), _bunkname, Convert.ToInt32(ddlPaymode.SelectedValue),
                    Convert.ToDecimal(txtQuantity.Text), Convert.ToDecimal(txtUnitPrice.Text),
                    Convert.ToString(txtLocation.Text), Convert.ToDecimal(txtAmount.Text),
                    Convert.ToInt32(txtPilotID.Text), Convert.ToString(txtPilotName.Text),
                    Convert.ToInt32(ddlCardSwiped.SelectedValue), Convert.ToString(txtRemarks.Text));
                FillGridFuelEntry(Convert.ToInt32(ViewState["VehicleID"]));
            }
        }

    }
    private DataTable getpreviousODO(int vehicleId)
    {
        DataTable dtVehData;
        string connetionString = null;
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataSet ds = new DataSet();
        connetionString = ConfigurationManager.AppSettings["Str"];
        using (var connection = new SqlConnection(connetionString))
        {
            try
            {
                connection.Open();
                adapter.SelectCommand = new SqlCommand("select max(entrydate) maxentry from T_FMS_FuelEntryDetails where vehicleid = '" + vehicleId + "' and status = 1", connection);
                adapter.Fill(ds);
                connection.Close();
                dtVehData = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex.GetBaseException();
            }

        }

        return dtVehData;


    }
    private void ShowKmpl()
    {
       var ds = ObjFuelEntry.GetKMPL(Convert.ToInt32(ddlVehicleNumber.SelectedValue));
        if (ds.Tables[0].Rows.Count > 0)
        {
            switch (ds.Tables[0].Rows[0]["KMPL"].ToString())
            {
                case "":
                    _flag = false;
                    _kmplInt = 0;
                    break;
                default:
                    string kmpl = ds.Tables[0].Rows[0]["KMPL"].ToString();
                    _kmplInt = Convert.ToDouble(kmpl);
                    _flag = true;
                    break;
            }
        }

        else
        {
            _flag = false;
            _kmplInt = 0;
        }

    }

    private void ShowMasterKmpl()
    {
    
        var ds = ObjFuelEntry.GetMasterKMPL(Convert.ToInt32(ddlVehicleNumber.SelectedValue));
        if (ds.Tables[0].Rows.Count > 0)
        {
            switch (ds.Tables[0].Rows[0]["KMPL"].ToString())
            {
                case "":
                    _flag = false;
                    _mSkmplInt = 0;
                    break;
                default:
                    string masterkmpl = (ds.Tables[0].Rows[0]["KMPL"].ToString());
                    _mSkmplInt = Convert.ToDouble(masterkmpl);
                    _flag = true;
                    break;
            }
        }

        else
        {
            _flag = false;
            _kmplInt = 0;
        }
        //ClearFields();
    }

    private void UpdFuelEntry1(int fuelEntryId, int districtId, int vehicleId, DateTime entryDate, long billNumber, long odometer, string bunkName, int paymode, decimal quantity, decimal unitPrice, string location, decimal amount, int pilotId, string pilotName, int cardSwipedStatus, string remarks)
    {

        int res = ObjFuelEntry.IUpdFuelEntry1(fuelEntryId, districtId, vehicleId, entryDate, billNumber, odometer, bunkName, paymode, quantity, unitPrice, location, amount, pilotId, pilotName, cardSwipedStatus, remarks);

        ShowKmpl();
        ShowMasterKmpl();
        string strFmsScript;
        switch (res)
        {
            case 1:
                if (Math.Abs(Math.Abs(_kmplInt)) <= 0 && _flag == false)
                {
                    strFmsScript = "Fuel Entry Inserted and KMPL is NA since no past Fuel Entry Records are found for this vehicle";
                    Show(strFmsScript);
                }
                else if (_kmplInt < 8)
                {
                    strFmsScript = "Fuel Entry Inserted and KMPL = " + _kmplInt + "\\nBenchMark KMPL =" + _mSkmplInt;
                    Show(strFmsScript);
                }
                else
                {
                    strFmsScript = "Fuel Entry Inserted and KMPL = " + _kmplInt + "\\nBenchMark KMPL =" + _mSkmplInt;
                    Show(strFmsScript);
                }

                break;
            default:
                strFmsScript = "Failure";
                Show(strFmsScript);
                break;
        }

        ClearFields();
    }
    private void InsFuelEntry(int districtId, int vehicleId, int borrowedVehicle, DateTime entryDate, long billNumber, long odometer, string bunkName, int paymode, decimal quantity, long petroCardNumber, decimal unitPrice, int agencyId, string location, int createdBy, DateTime createdDate, int status, decimal amount, int pilotId, string pilotName, int cardSwipedStatus, string remarks)
    {
        var dsOdo = ObjFuelEntry.ICheckFuelEntryOdo(Convert.ToInt32(ddlVehicleNumber.SelectedValue));

        int maxodo = Convert.ToInt32(dsOdo.Tables[0].Rows[0]["ODO"].ToString());

        if (maxodo < odometer)
        {
            var dsres = ObjFuelEntry.IInsFuelEntry(districtId, vehicleId, borrowedVehicle, entryDate, billNumber, odometer, bunkName, paymode, quantity, petroCardNumber, unitPrice, agencyId, location, createdBy, createdDate, status, amount, pilotId, pilotName, cardSwipedStatus, remarks);
            ShowKmpl();
            ShowMasterKmpl();
            if (dsres.Tables[0].Rows.Count > 0)
            {
                string resid = dsres.Tables[0].Rows[0][0].ToString();
                if (Math.Abs(_kmplInt) <= 0 && _flag == false)
                {
                    string strFmsScript = "Fuel Entry Inserted and KMPL is NA since no past Fuel Entry Records are found for this vehicle";
                    Show(strFmsScript);
                }
                else if (_kmplInt < 8)
                {
                    string strFmsScript = "Fuel Entry Inserted and KMPL = " + _kmplInt + "\\nBenchMark KMPL =" + _mSkmplInt + "\\nTransaction Id = " + resid;
                    Show(strFmsScript);
                }
                else
                {
                    string strFmsScript = "Fuel Entry Inserted and KMPL = " + _kmplInt + "\\nBenchMark KMPL =" + _mSkmplInt + "\\nTransaction Id = " + resid;
                    Show(strFmsScript);
                }

            }
            else
            {
                string strFmsScript = "Failure";
                Show(strFmsScript);
            }
            ClearFields();

        }

        else
        {
            string strFmsScript = "Odometer Reading can't be less than the Previous Odometer Reading";
            Show(strFmsScript);

        }

    }
    private void InsFuelEntry1(int districtId, int vehicleId, DateTime entryDate, long billNumber, long odometer, string bunkName, int paymode, decimal quantity, decimal unitPrice, string location, int createdBy, DateTime createdDate, int status, decimal amount, int pilotId, string pilotName, int cardSwipedStatus, string remarks)
    {

       var ds = ObjFuelEntry.IInsFuelEntry1(districtId, vehicleId, entryDate, billNumber, odometer, bunkName, paymode, quantity, unitPrice, location, createdBy, createdDate, status, amount, pilotId, pilotName, cardSwipedStatus, remarks);

        ShowKmpl();
        ShowMasterKmpl();
        if (ds.Tables[0].Rows.Count > 0)
        {
            string resid = ds.Tables[0].Rows[0][0].ToString();
            if (Math.Abs(_kmplInt)>=0 && _flag == false)
            {
                string strFmsScript = "Fuel Entry Inserted and KMPL is NA since no past Fuel Entry Records are found for this vehicle";
                Show(strFmsScript);
            }
            else if (_kmplInt < 8)
            {
                string strFmsScript = "Fuel Entry Inserted and KMPL = " + _kmplInt + "\\nBenchMark KMPL =" + _mSkmplInt + "\\nTransaction Id = " + resid;
                Show(strFmsScript);
            }
            else
            {
                string strFmsScript = "Fuel Entry Inserted and KMPL = " + _kmplInt + "\\nBenchMark KMPL =" + _mSkmplInt + "\\nTransaction Id = " + resid;
                Show(strFmsScript);
            }

        }
        else
        {
            string strFmsScript = "Failure";
            Show(strFmsScript);
        }
        ClearFields();

    }
    private void UpdFuelEntry(int fuelEntryId, int districtId, int vehicleId, int borrowedVehicle, DateTime entryDate, long billNumber, long odometer, string bunkName, int paymode, decimal quantity, long petroCardNumber, decimal unitPrice, int agencyId, string location, decimal amount, int pilotId, string pilotName, int cardSwipedStatus, string remarks)
    {
        int res = ObjFuelEntry.IUpdFuelEntry(fuelEntryId, districtId, vehicleId, borrowedVehicle, entryDate, billNumber, odometer, bunkName, paymode, quantity, petroCardNumber, unitPrice, agencyId, location, amount, pilotId, pilotName, cardSwipedStatus, remarks);

        ShowKmpl();
        ShowMasterKmpl();
        if (res == 1)
        {
            if (Math.Abs(_kmplInt) <=0 && _flag == false)
            {
                string strFmsScript = "Fuel Entry Inserted and KMPL is NA since no past Fuel Entry Records are found for this vehicle";
                Show(strFmsScript);
            }
            else if (_kmplInt < 8)
            {
                string strFmsScript = "Fuel Entry Inserted and KMPL = " + _kmplInt + "\\nBenchMark KMPL =" + _mSkmplInt;
                Show(strFmsScript);
            }
            else
            {
                string strFmsScript = "Fuel Entry Inserted and KMPL = " + _kmplInt + "\\nBenchMark KMPL =" + _mSkmplInt;
                Show(strFmsScript);
            }

        }
        else
        {
            string strFmsScript = "Failure";
            Show(strFmsScript);
        }
        ClearFields();
    }



    private void FillGridFuelEntry(int vehicleId)
    {
        gvFuelEntry.Visible = true;

        if (Session["UserdistrictId"] != null)
        {
           var districtId = Convert.ToInt32(Session["UserdistrictId"].ToString());
        }

       
        var ds = ObjFuelEntry.IFillGridFuelEntry(vehicleId);
        if (ds != null && ds.Tables.Count > 0)
        {
            gvFuelEntry.DataSource = ds;
            gvFuelEntry.DataBind();
            ViewState["maxodometer"] = ds.Tables[0].Rows[0]["odo"].ToString();
        }
        else
        {
            ViewState["maxodometer"] = 0;
        }

    }

    protected void gvFuelEntry_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvFuelEntry.PageIndex = e.NewPageIndex;

        if (Session["UserdistrictId"] != null)
        {
            var districtId = Convert.ToInt32(Session["UserdistrictId"].ToString());
        }
       var ds = ObjFuelEntry.IFillGridFuelEntry(Convert.ToInt32(ddlVehicleNumber.SelectedValue));
        gvFuelEntry.DataSource = ds;
        gvFuelEntry.DataBind();
    }

    protected void gvFuelEntry_RowEditing(object sender, GridViewEditEventArgs e)
    {



    }

    protected void gvFuelEntry_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

    }

    protected void Reset_Click(object sender, EventArgs e)
    {
        ClearFields();
    }

    private void ClearFields()
    {
        txtAmount.Text = string.Empty; ;

        txtBillNumber.Text = string.Empty;
        if (ddlBunkName.Visible)
        {
            ddlBunkName.Items.Clear();
        }
        else
        {
            txtBunkName.Text = string.Empty; 
        }
        txtEdit.Text = string.Empty;
        txtFuelEntryDate.Text = string.Empty;
        txtLocation.Text = string.Empty;
        txtOdometer.Text = string.Empty;
        txtQuantity.Text = string.Empty;
        txtSegmentID.Text = string.Empty;
        txtUnitPrice.Text = string.Empty;
        txtPilotID.Text = string.Empty;
        txtPilotName.Text = string.Empty;
        DataSet ds = new DataSet();
        if (ddlAgency.Items.Count != 0)
        {
            ddlAgency.SelectedIndex = -1;
        }
        ddlPaymode.SelectedIndex = 0;

        if (ddlAgency.Items.Count != 0)
        {
            ddlPetroCardNumber.SelectedIndex = -1;
        }

        if (ddlAgency.Items.Count != 0)
        {
            ddlVehicleNumber.SelectedIndex = -1;
        }

        txtRemarks.Text = "";


        ddlAgency.Enabled = true;
        ddlAgency.Items.Clear();
        ddlPaymode.Enabled = true;
        ddlPetroCardNumber.Enabled = true;
        ddlPetroCardNumber.Items.Clear();
        ddlVehicleNumber.Enabled = true;
        ddlDistrict.Enabled = true;
        ddlDistrict.Items.Clear();
        ddlCardSwiped.SelectedIndex = -1;
        Save.Text = "Save";
        ddlVehicleNumber.SelectedIndex = 0;
        txtOdometer.Enabled = true;
        ddlCardSwiped.SelectedIndex = -1;
        ddlCardSwiped.Enabled = true;
        gvLastTransactions.Visible = false;
        gvFuelEntry.Visible = false;
        lblDistrict.Visible = false;
        lblLocation.Visible = false;
        txtBunkName.Visible = true;
        txtBunkName.Text = "";
        txtBunkName.Enabled = false;
        ddlBunkName.Visible = false;
        Save.Enabled = true;

    }

    public void Show(string message)
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('" + message + "');", true);
    }

    protected void gvFuelEntry_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditFuel")
        {

            Save.Text = "Update";



            int id = Convert.ToInt32(e.CommandArgument.ToString());
           var ds = ObjFuelEntry.IEditFuelEntryDetails(id);
            FillCardNumber(Convert.ToInt32(ds.Tables[0].Rows[0]["VehicleID"].ToString()));
            ddlPetroCardNumber.Enabled = false;
            txtEdit.Text = Convert.ToString(id);

            ddlPaymode.ClearSelection();
            ddlPaymode.Items.FindByValue(ds.Tables[0].Rows[0]["Paymode"].ToString()).Selected = true;

            ddlVehicleNumber.ClearSelection();
            ddlVehicleNumber.Items.FindByValue(ds.Tables[0].Rows[0]["VehicleID"].ToString()).Selected = true;
            ddlCardSwiped.ClearSelection();
            ddlCardSwiped.Items.FindByValue(ds.Tables[0].Rows[0]["CardSwipedStatus"].ToString()).Selected = true;
            ddlCardSwiped.Enabled = false;


            FillDistricts();

            maxOdo.Value = "0";

            txtFuelEntryDate.Text = ds.Tables[0].Rows[0]["EntryDate"].ToString();
            txtBillNumber.Text = ds.Tables[0].Rows[0]["BillNumber"].ToString();
            txtOdometer.Text = ds.Tables[0].Rows[0]["Odometer"].ToString();
            //  txtOdometer.Enabled = false;
            txtBunkName.Text = ds.Tables[0].Rows[0]["BunkName"].ToString();
            string[] coBGrade = (Convert.ToString(ds.Tables[0].Rows[0]["Quantity"].ToString())).Split('.');
            txtQuantity.Text = coBGrade[0] + '.' + coBGrade[1].Substring(0, 2);
            txtLocation.Text = ds.Tables[0].Rows[0]["Location"].ToString();
            string[] cGrade = (Convert.ToString(ds.Tables[0].Rows[0]["UnitPrice"].ToString())).Split('.');
            txtUnitPrice.Text = cGrade[0] + '.' + cGrade[1].Substring(0, 2);
            string[] coAGrade = (Convert.ToString(ds.Tables[0].Rows[0]["Amount"].ToString())).Split('.');
            txtAmount.Text = coAGrade[0] + '.' + coAGrade[1].Substring(0, 2);
            txtPilotID.Text = ds.Tables[0].Rows[0]["Pilot"].ToString();
            txtPilotName.Text = ds.Tables[0].Rows[0]["PilotName"].ToString();
            txtRemarks.Text = ds.Tables[0].Rows[0]["RemarksFuel"].ToString();
            if (ds.Tables[0].Rows[0]["PetroCardNumber"].ToString() != string.Empty)
            {
                switch (Convert.ToInt32(ds.Tables[0].Rows[0]["BorrowedVehicleID"].ToString()))
                {
                    case 0:
                    {
                        int vid = Convert.ToInt32(ds.Tables[0].Rows[0]["VehicleID"].ToString());
                        ddlPetroCardNumber.ClearSelection();
                        FillCardNumber(vid);
                        ddlPetroCardNumber.Items.FindByValue(ds.Tables[0].Rows[0]["PetroCardNumber"].ToString()).Selected = true;
                        int pid = Convert.ToInt32(ds.Tables[0].Rows[0]["PetroCardNumber"].ToString());
                        ddlAgency.ClearSelection();
                        FillFuelAgency(pid);
                        ddlAgency.Items.FindByValue(ds.Tables[0].Rows[0]["AgencyID"].ToString()).Selected = true;

                        break;
                    }
                    default:
                    {
                        int vid = Convert.ToInt32(ds.Tables[0].Rows[0]["BorrowedVehicleID"].ToString());
                        ddlDistrict.ClearSelection();
                        ddlDistrict.Items.FindByValue(ds.Tables[0].Rows[0]["BorrowedVehicleID"].ToString()).Selected = true;
                        ddlPetroCardNumber.ClearSelection();
                        FillCardNumber(vid);
                        ddlPetroCardNumber.Items.FindByValue(ds.Tables[0].Rows[0]["PetroCardNumber"].ToString()).Selected = true;
                        int pid = Convert.ToInt32(ds.Tables[0].Rows[0]["PetroCardNumber"].ToString());
                        ddlAgency.ClearSelection();
                        FillFuelAgency(pid);
                        ddlAgency.Items.FindByValue(ds.Tables[0].Rows[0]["AgencyID"].ToString()).Selected = true;
                        break;
                    }
                }
            }


            else 
            {
                FillFuelAgency(0);
                int vid = Convert.ToInt32(ds.Tables[0].Rows[0]["VehicleID"].ToString());
                 ObjFuelEntry.IFillCardNumber(vid);

                 ObjFuelEntry.IFillAgencyWoDistrictID();

            }
            ddlAgency.Enabled = false;
            ddlPaymode.Enabled = false;
            ddlPetroCardNumber.Enabled = false;
            ddlVehicleNumber.Enabled = false;
            ddlDistrict.Enabled = false;


        }
        else if (e.CommandName == "DeleteFuel")
        {
        

            int id = Convert.ToInt32(e.CommandArgument.ToString());

           var result = ObjFuelEntry.IDeleteFuelEntry(id);
            switch (result)
            {
                case 1:
                {
                    string strFmsScript = "Fuel Entry Deactivated";
                    Show(strFmsScript);
                    break;
                }
                default:
                {
                    string strFmsScript = "failure";
                    Show(strFmsScript);
                    break;
                }
            }

            ClearFields();
            FillGridFuelEntry(Convert.ToInt32(ViewState["VehicleID"]));
        }
        
    }
 
    protected void lnkNew_Click(object sender, EventArgs e)
    {
        txtBunkName.Visible = false;
        ddlBunkName.Visible = true;
        linkExisting.Visible = true;
        lnkNew.Visible = false;
        FillServiceStn();
    }

    protected void linkExisting_Click(object sender, EventArgs e)
    {
        txtBunkName.Visible = true;
        ddlBunkName.Visible = false;
        linkExisting.Visible = false;
        lnkNew.Visible = true;
        FillServiceStnVeh();
    }



}