﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using GvkFMSAPP.BLL;
using GvkFMSAPP.PL;

public partial class AgencyDetails : Page
{
    public IFleetMaster ObjFleetMaster = new FMSFleetMaster();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["User_Name"] == null) Response.Redirect("Error.aspx");
        if (!IsPostBack)
        {
            grvAgencyDetails.Columns[0].Visible = false;
            grvAgencyDetails.Columns[3].Visible = false;
            FillStates();
            FillDistricts(0);
            ddlDistrict.Enabled = false;
            FillGridAgencyDetails();
            btnSaveAgencyDetails.Attributes.Add("onclick", "javascript:return  validationAgencyDetails()");
            txtAddress.Attributes.Add("onkeypress", "javascript:return  remark(this,event)");
            txtAgencyName.Attributes.Add("onkeypress", "javascript:return  OnlyAlphabets(this,event)");
            txtContactNo.Attributes.Add("onkeypress", "javascript:return  isNumberKey(this,event)");
            txtTin.Attributes.Add("onkeypress", "javascript:return  isNumberKey(this,event)");
            txtPanNo.Attributes.Add("onkeypress", "javascript:return  OnlyAlphaNumeric(this,event)");
            //Permissions
            var dsPerms = (DataSet) Session["PermissionsDS"];
            dsPerms.Tables[0].DefaultView.RowFilter = "Url='" + Page.Request.Url.Segments[Page.Request.Url.Segments.Length - 1] + "'";
            var p = new PagePermissions(dsPerms, dsPerms.Tables[0].DefaultView[0]["Url"].ToString(), dsPerms.Tables[0].DefaultView[0]["Title"].ToString());
            if (p.Modify != true) return;
            grvAgencyDetails.Visible = true;
            grvAgencyDetails.Columns[5].Visible = true;
            grvAgencyDetails.Columns[6].Visible = false;
        }
    }

    private void FillGridAgencyDetails()
    {
        var ds = ObjFleetMaster.IFillGridAgencyDetails();
        grvAgencyDetails.DataSource = ds;
        grvAgencyDetails.DataBind();
    }

    private void FillStates()
    {
        var ds = ObjFleetMaster.IFillStates();
        AccidentReport.FillDropDownHelperMethodWithDataSet(ds, "sc_lname", "sc_scid", ddlState);
        ddlState.Items[0].Value = "0";
        ddlState.SelectedIndex = 0;
        ddlDistrict.Enabled = true;
    }

    protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillDistricts(Convert.ToInt32(ddlState.SelectedValue));
    }

    private void FillDistricts(int stateId)
    {
        var ds = ObjFleetMaster.IFillDistricts(stateId);
        AccidentReport.FillDropDownHelperMethodWithDataSet(ds, "district_name", "district_id", ddlDistrict);
        ddlDistrict.Items[0].Value = "0";
        ddlDistrict.SelectedIndex = 0;
        ddlDistrict.Enabled = true;
    }

    protected void ddlDistrict_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    protected void grvAgencyDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        FillStates();
        btnSaveAgencyDetails.Text = "Update";
        var index = e.NewEditIndex;
        var lblId = (Label) grvAgencyDetails.Rows[index].FindControl("lblId");
        var cmd = new SqlCommand();

        //int AgencyID = Convert.ToInt16(grvAgencyDetails.Rows[index].Cells[0].Text);
        hidAgencyId.Value = lblId.Text;
        int agencyId = Convert.ToInt16(hidAgencyId.Value);
        var ds = ObjFleetMaster.IEditAgencyDetails(agencyId);
        txtEdit.Text = hidAgencyId.Value;
        txtAgencyName.Text = ds.Tables[0].Rows[0]["AgencyName"].ToString();
        ddlState.SelectedValue = ds.Tables[0].Rows[0]["StateID"].ToString();
        int sid = Convert.ToInt16(ddlState.SelectedValue);
        FillDistricts(sid);
        ddlDistrict.SelectedValue = ds.Tables[0].Rows[0]["DistrictID"].ToString();
        int did = Convert.ToInt16(ddlDistrict.SelectedValue);
        txtAddress.Text = ds.Tables[0].Rows[0]["Address"].ToString();
        txtContactNo.Text = ds.Tables[0].Rows[0]["ContactNum"].ToString();
        txtPanNo.Text = ds.Tables[0].Rows[0]["PANNum"].ToString();
        txtTin.Text = ds.Tables[0].Rows[0]["TIN"].ToString();
    }

    protected void grvAgencyDetails_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        {
            Clearfields();
            var result = 0;
            var index = e.RowIndex;
            var lblId = (Label) grvAgencyDetails.Rows[index].FindControl("lblId");
            var cmd = new SqlCommand();
            var ds = new DataSet();
            hidAgencyId.Value = lblId.Text;
            int agencyId = Convert.ToInt16(hidAgencyId.Value);
            result = ObjFleetMaster.IDeleteAgencyDetails(agencyId);
            Show(result == 1 ? "Agency Details Deleted Successfully" : "Agency Details Deletion Failure");
            FillGridAgencyDetails();
        }
    }

    protected void grvAgencyDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grvAgencyDetails.PageIndex = e.NewPageIndex;
        var ds = ObjFleetMaster.IFillGridAgencyDetails();
        grvAgencyDetails.DataSource = ds;
        grvAgencyDetails.DataBind();
    }

    protected void btnSaveAgencyDetails_Click(object sender, EventArgs e)
    {
        {
            switch (btnSaveAgencyDetails.Text)
            {
                case "Save":
                {
                    var cmd = new SqlCommand();
                    var ds = ObjFleetMaster.IFillGridAgencyDetails();
                    if (ds.Tables[0].Select("AgencyName='" + txtAgencyName.Text + "'").Length <= 0)
                        InsertAgencyDetails(Convert.ToString(txtAgencyName.Text), Convert.ToInt32(ddlState.SelectedValue), Convert.ToInt32(ddlDistrict.SelectedValue), Convert.ToInt32(0), Convert.ToInt32(0), Convert.ToInt64(txtContactNo.Text), Convert.ToString(txtPanNo.Text), Convert.ToInt64(txtTin.Text), Convert.ToString(txtAddress.Text));
                    else
                        Show("Agency Name already exists");
                    break;
                }
                default:
                {
                    var cmd = new SqlCommand();
                    var ds = ObjFleetMaster.IFillGridAgencyDetails();
                    if (ds.Tables[0].Select("AgencyName='" + txtAgencyName.Text + "' And AgencyID<>'" + txtEdit.Text + "'").Length <= 0)
                        UpdateAgencyDetails(Convert.ToInt32(txtEdit.Text), Convert.ToString(txtAgencyName.Text), Convert.ToInt32(ddlState.SelectedValue), Convert.ToInt32(ddlDistrict.SelectedValue), Convert.ToInt32(0), Convert.ToInt32(0), Convert.ToInt64(txtContactNo.Text), Convert.ToString(txtPanNo.Text), Convert.ToInt64(txtTin.Text), Convert.ToString(txtAddress.Text));
                    else
                        Show("Agency Name already exists");
                    break;
                }
            }

            FillGridAgencyDetails();
        }
    }

    private void UpdateAgencyDetails(int agencyId, string agencyName, int stateId, int districtId, int mandalId, int cityId, long contactNum, string panNum, long tin, string address)
    {
        var res = ObjFleetMaster.IUpdateAgencyDetails(agencyId, agencyName, stateId, districtId, mandalId, cityId, contactNum, panNum, tin, address);
        switch (res)
        {
            case 1:
                Show("Agency Details Updated Successfully");
                Clearfields();
                break;
            default:
                Show("Agency Details already exists");
                break;
        }
    }

    private void InsertAgencyDetails(string agencyName, int stateId, int districtId, int mandalId, int cityId, long contactNum, string panNum, long tin, string address)
    {
        var res = ObjFleetMaster.IInsertAgencyDetails(agencyName, stateId, districtId, mandalId, cityId, contactNum, panNum, tin, address);
        switch (res)
        {
            case 1:
                Show("Agency Details Added Successfully");
                Clearfields();
                break;
            default:
                Show("Agency Details already exists");
                break;
        }
    }

    protected void btnResetAgencyDetails_Click(object sender, EventArgs e)
    {
        Clearfields();
    }

    private void Clearfields()
    {
        btnSaveAgencyDetails.Text = "Save";
        txtAddress.Text = "";
        txtAgencyName.Text = "";
        txtContactNo.Text = "";
        txtEdit.Text = "";
        txtPanNo.Text = "";
        txtTin.Text = "";
        ddlDistrict.SelectedIndex = 0;
        ddlDistrict.Enabled = false;
        ddlState.SelectedIndex = 0;
    }

    public void Show(string message)
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('" + message + "');", true);
    }
}