﻿<%@ Page Title="" Language="C#" MasterPageFile="~/temp.master" AutoEventWireup="true" CodeFile="BillProcessingManagementReport.aspx.cs" Inherits="BillProcessingManagementReport" %>
<%@ Reference Page="~/AccidentReport.aspx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script type="text/javascript">
        $(function () {
            $('#<%= ddldistrict.ClientID %>').select2({
                disable_search_threshold: 5, search_contains: true, minimumResultsForSearch: 2, 
                placeholder: "Select an option"
            });
        });
        function Validations()
        {
            var ddlDistrict = $('#<%= ddldistrict.ClientID %> option:selected').text().toLowerCase();
            if (ddlDistrict === '--select--') {
              return  alert("Please select District");
            }
            return true;
        }
    </script>
    <table align="center">
        <tr>
            <td>
                <asp:Label ID="lblBillProcessingManagementReport" style="font-size: 20px; color: brown" runat="server" Text="Bill Processing Management Report"></asp:Label>
            </td>
        </tr>
    </table>
    <table align="center" style="margin-top: 10px" >
        <tr>

            <td>
                Select District<asp:Label ID="lbldistrict" runat="server" Text="Select&nbsp;District" style="color: red">*</asp:Label>
            </td>

            <td>
                <asp:DropDownList ID="ddldistrict" runat="server" style="width: 150px"></asp:DropDownList>
            </td>
            </tr>
        </table>
    <table align="center">
        <tr>
            <td>
                <asp:Button runat="server" Text="ShowReport" ID="btnShowReport" OnClick="btnsubmit_Click" OnClientClick="if(!Validations())return false;" CssClass="form-submit-button"></asp:Button>
            </td>
            <td>
                <asp:Button runat="server" Text="ExportExcel" OnClick="btntoExcel_Click" CssClass="form-reset-button"></asp:Button>

            </td>
        </tr>
    </table>
    <br/>
    <div align="center">
        <asp:Panel ID="Panel2" runat="server" Style="margin-left: 2px;">
            <asp:GridView ID="Grdtyre" runat="server" BorderWidth="1px" BorderColor="brown"></asp:GridView>
        </asp:Panel>
    </div>
</asp:Content>

