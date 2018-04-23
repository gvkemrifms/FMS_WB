﻿<%@ Page Title="" Language="C#" MasterPageFile="~/temp.master" AutoEventWireup="true" CodeFile="AccidentReport.aspx.cs" Inherits="AccidentReport" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">
        $(function() {
            $('#<%= ddldistrict.ClientID %>').chosen();
            $('#<%= ddlvehicle.ClientID %>').chosen();
        });
        function Validations() {
            $('#<%= ddldistrict.ClientID %>').chosen();
            var ddlDistrict = $('#<%= ddldistrict.ClientID %> option:selected').text().toLowerCase();
            if (ddlDistrict === '--select--') {
                return alert("Please select District");
            }
            $('#<%= ddlvehicle.ClientID %>').chosen();
            var ddlVehicle = $('#<%= ddlvehicle.ClientID %> option:selected').text().toLowerCase();
            if (ddlVehicle === '--select--') {
                return alert("Please select Vehicle");
            }
            var txtFirstDate = $('#<%= txtfrmDate.ClientID %>').val();
            var txtToDate = $('#<%= txttodate.ClientID %>').val();
            if (txtFirstDate === "") {
                return alert('From Date is Mandatory');
            }
            if (txtToDate === "") {
                return alert("End Date is Mandatory");
            }
            var fromDate = (txtFirstDate).replace(/\D/g, '/');
            var toDate = (txtToDate).replace(/\D/g, '/');
            var ordFromDate = new Date(fromDate);
            var ordToDate = new Date(toDate);
            var currentDate = new Date();
            if (ordFromDate > currentDate) {
                return alert("From date should not be greater than today's date");
            }
            if (ordToDate < ordFromDate) {
                alert("Please select valid date range");
            }
            return true;
        }
    </script>
    <br/>
    <table>
        <tr>
            <td>
                <asp:Label ID="lblaccidentreport" Style="font-size: 20px; color: brown" runat="server" Text="Accident&nbsp;Report"></asp:Label>
            </td>
        </tr>
    </table>
    <br/>

    <table style="width: 100px; margin-left: 175px;">
        <tr>
          
            <td >
             
                    Select District <span style="color: Red;">*</span>
              
            </td>
            <td>
                <asp:DropDownList ID="ddldistrict" runat="server" Style="width: 150px;display:inline-block;white-space:pre;margin:30px;padding-right: 150px;" AutoPostBack="true" OnSelectedIndexChanged="ddldistrict_SelectedIndexChanged"></asp:DropDownList>
            </td>
            <td>

            </td>
            </tr>
        <tr>
            <td >
            Select Vehicle <span style="color: Red;display:inline-block; width: 10px;">*</span>
            </td>

            <td>
                <asp:DropDownList ID="ddlvehicle"  runat="server" Style="width: 150px;padding-right:50px;"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td >
                From Date <span style="color: Red">*</span>
            </td>

            <td>
                <asp:TextBox ID="txtfrmDate" placeholder="From Date" class="search_3" runat="server" onkeypress="return false" oncut="return false;" onpaste="return false;"></asp:TextBox>
            </td>
            <td>
                <cc1:CalendarExtender ID="CalendarExtender2" runat="server" Format="MM/dd/yyyy" TargetControlID="txtfrmDate" Enabled="true" CssClass="cal_Theme1"></cc1:CalendarExtender>
            </td>
            </tr>
        <tr>
            <td>
                To date <span style="color: Red;display:inline-block;">*</span>
            </td>
            <td>
                <asp:TextBox ID="txttodate" placeholder="To Date"  class="search_3"  runat="server"  onkeypress="return false" oncut="return false;" onpaste="return false;"></asp:TextBox>
            </td>
            <td>
                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" Format="MM/dd/yyyy"  TargetControlID="txttodate" Enabled="true" CssClass="cal_Theme1"></cc1:CalendarExtender>
            </td>
        </tr>

        <tr>
            <td>
                <asp:Button runat="server"  id="btnShowReport"  class="form-submit-button" Text="ShowReport" OnClick="btnsubmit_Click" ClientIDMode="static" EnableViewState="True"  OnClientClick="if(!Validations()) return false;"></asp:Button>
            </td>

            <td>
                <asp:Button runat="server" Text="ExportExcel" class="form-reset-button" OnClick="btntoExcel_Click"></asp:Button>
            </td>
            <td>
                <asp:HiddenField ID="HiddenFileldVariable" runat="server"/>
            </td>
        </tr>

    </table>
    <asp:Panel ID="Panel2" runat="server" Style="margin-left: 2px;">
        <asp:GridView ID="Grddetails" runat="server"></asp:GridView>
    </asp:Panel>
</asp:Content>









