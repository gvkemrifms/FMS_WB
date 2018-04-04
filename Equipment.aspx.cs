﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using GvkFMSAPP.BLL;
using GvkFMSAPP.BLL.Admin;

public partial class Equipment : Page
{
    private readonly DistrictVehicleMapping _distvehmapp = new DistrictVehicleMapping();
    public IFleetMaster ObjFleetMaster = new FMSFleetMaster();


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["User_Name"] == null) Response.Redirect("Error.aspx");

        if (!IsPostBack)
        {
            BtnSave.Attributes.Add("onclick", "return validation()");
            GetVehicles();
            var numbers = new List<int> {1, 2, 3, 4, 5, 6};
            foreach (var number in numbers)
            {
                var ds = ObjFleetMaster.GetSelectAllMedicalEquipmentByEquipmentTypeID(number);
                switch (number)
                {
                    case 1:
                        grdviewMedicalEqupment.DataSource = ds;
                        grdviewMedicalEqupment.DataBind();
                        break;
                    case 2:
                        grdviewMedicalDisposables.DataSource = ds;
                        grdviewMedicalDisposables.DataBind();
                        break;
                    case 3:
                        grdviewExtricationTools.DataSource = ds;
                        grdviewExtricationTools.DataBind();
                        break;
                    case 4:
                        grdviewCOmmunicationTechnology.DataSource = ds;
                        grdviewCOmmunicationTechnology.DataBind();
                        break;
                    case 5:
                        grdviewMedicines.DataSource = ds;
                        grdviewMedicines.DataBind();
                        break;
                    case 6:
                        grdviewNoMedicalSupplies.DataSource = ds;
                        grdviewNoMedicalSupplies.DataBind();
                        break;
                }
            }
        }
    }

    public void GetVehicles()
    {
        var ds = _distvehmapp.GetVehicles();
        if (ds != null) AccidentReport.FillDropDownHelperMethodWithDataSet(ds, "VehicleNumber", "VehicleID", ddlistVehicleNumber);
    }

    protected void BtnSave_Click(object sender, EventArgs e)
    {
        var equipmentArray = new ArrayList();

        foreach (GridViewRow row in grdviewMedicalEqupment.Rows)
        {
            if (!((CheckBox) row.FindControl("chkMedicalEquipment")).Checked) continue;
            var lblMedicalEquipmentName = (Label) row.FindControl("LblMedicalEquipmentId");
            equipmentArray.Add(lblMedicalEquipmentName.Text);
        }

        foreach (GridViewRow row in grdviewMedicalDisposables.Rows)
        {
            if (!((CheckBox) row.FindControl("chkMedicalDisposables")).Checked) continue;
            var lblDisposableName = (Label) row.FindControl("LblDisposableId");
            equipmentArray.Add(lblDisposableName.Text);
        }

        foreach (GridViewRow row in grdviewExtricationTools.Rows)
        {
            if (!((CheckBox) row.FindControl("chkExtricationTools")).Checked) continue;
            var lblExtricationName = (Label) row.FindControl("LblExtricationId");
            equipmentArray.Add(lblExtricationName.Text);
        }

        foreach (GridViewRow row in grdviewCOmmunicationTechnology.Rows)
        {
            if (!((CheckBox) row.FindControl("chkCommunicationTechnology")).Checked) continue;
            var lblCommunicationName = (Label) row.FindControl("LblCommunicationId");
            equipmentArray.Add(lblCommunicationName.Text);
        }

        foreach (GridViewRow row in grdviewMedicines.Rows)
        {
            if (!((CheckBox) row.FindControl("chkMedicines")).Checked) continue;
            var lblMedicineName = (Label) row.FindControl("LblMedicineId");
            equipmentArray.Add(lblMedicineName.Text);
        }

        foreach (GridViewRow row in grdviewNoMedicalSupplies.Rows)
        {
            if (!((CheckBox) row.FindControl("chkNoMedicalSupplies")).Checked) continue;
            var lblNoMedicalName = (Label) row.FindControl("LblNoMedicalId");
            equipmentArray.Add(lblNoMedicalName.Text);
        }

        if (equipmentArray.Count <= 0)
        {
            Show("Please Select The Equipments");
        }
        else
        {
            ObjFleetMaster.IDeleteVehicleEquipmentMapping(int.Parse(ddlistVehicleNumber.SelectedItem.Value));

            foreach (string equipmentArrayId in equipmentArray) ObjFleetMaster.IInsertVehicleEquipmentMapping(int.Parse(equipmentArrayId), int.Parse(ddlistVehicleNumber.SelectedItem.Value));

            Show("Vehicle Mapped to Equipment Successfully");
            Clearcontrols();
        }
    }

    public void Show(string message)
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('" + message + "');", true);
    }

    protected void ddlistVehicleNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (ddlistVehicleNumber.SelectedIndex)
        {
            case 0:
                return;
        }

        var mapping = new ArrayList();

        var ds = ObjFleetMaster.GetSelectAllVehicleEquipmentByVehicleId(Convert.ToInt32(ddlistVehicleNumber.SelectedItem.Value));

        foreach (DataRow dr in ds.Tables[0].Rows) mapping.Add(dr[1].ToString());

        foreach (GridViewRow row in grdviewMedicalEqupment.Rows)
        {
            var chk = (CheckBox) row.FindControl("chkMedicalEquipment");

            var lblMedicalEquipmentName = (Label) row.FindControl("LblMedicalEquipmentId");

            chk.Checked = mapping.Contains(lblMedicalEquipmentName.Text);
        }


        foreach (GridViewRow row in grdviewMedicalDisposables.Rows)
        {
            var chk = (CheckBox) row.FindControl("chkMedicalDisposables");

            var lblDisposableName = (Label) row.FindControl("LblDisposableId");

            chk.Checked = mapping.Contains(lblDisposableName.Text);
        }

        foreach (GridViewRow row in grdviewExtricationTools.Rows)
        {
            var chk = (CheckBox) row.FindControl("chkExtricationTools");

            var lblExtricationName = (Label) row.FindControl("LblExtricationId");

            chk.Checked = mapping.Contains(lblExtricationName.Text);
        }

        foreach (GridViewRow row in grdviewCOmmunicationTechnology.Rows)
        {
            var chk = (CheckBox) row.FindControl("chkCommunicationTechnology");

            var lblCommunicationName = (Label) row.FindControl("LblCommunicationId");

            chk.Checked = mapping.Contains(lblCommunicationName.Text);
        }

        foreach (GridViewRow row in grdviewMedicines.Rows)
        {
            var chk = (CheckBox) row.FindControl("chkMedicines");

            var lblMedicineId = (Label) row.FindControl("LblMedicineId");

            chk.Checked = mapping.Contains(lblMedicineId.Text);
        }

        foreach (GridViewRow row in grdviewNoMedicalSupplies.Rows)
        {
            var chk = (CheckBox) row.FindControl("chkNoMedicalSupplies");

            var lblNoMedicalId = (Label) row.FindControl("LblNoMedicalId");

            chk.Checked = mapping.Contains(lblNoMedicalId.Text);
        }
    }

    public void Clearcontrols()
    {
        ddlistVehicleNumber.SelectedIndex = 0;

        foreach (GridViewRow row in grdviewMedicalEqupment.Rows)
        {
            var chk = (CheckBox) row.FindControl("chkMedicalEquipment");
            chk.Checked = false;
        }

        foreach (GridViewRow row in grdviewMedicalDisposables.Rows)
        {
            var chk = (CheckBox) row.FindControl("chkMedicalDisposables");
            chk.Checked = false;
        }

        foreach (GridViewRow row in grdviewExtricationTools.Rows)
        {
            var chk = (CheckBox) row.FindControl("chkExtricationTools");
            chk.Checked = false;
        }

        foreach (GridViewRow row in grdviewCOmmunicationTechnology.Rows)
        {
            var chk = (CheckBox) row.FindControl("chkCommunicationTechnology");
            chk.Checked = false;
        }

        foreach (GridViewRow row in grdviewMedicines.Rows)
        {
            var chk = (CheckBox) row.FindControl("chkMedicines");
            chk.Checked = false;
        }

        foreach (GridViewRow row in grdviewNoMedicalSupplies.Rows)
        {
            var chk = (CheckBox) row.FindControl("chkNoMedicalSupplies");
            chk.Checked = false;
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Clearcontrols();
    }
}