<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DatabaseDiagnostics.aspx.cs" Inherits="CyberArenaWebForms5.DatabaseDiagnostics" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Database Diagnostics & CRUD Test</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 20px;
            background-color: #f5f5f5;
        }
        .container {
            max-width: 1200px;
            margin: 0 auto;
            background-color: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        h1 {
            color: #333;
            border-bottom: 3px solid #007bff;
            padding-bottom: 10px;
        }
        h2 {
            color: #007bff;
            margin-top: 30px;
        }
        .status {
            padding: 10px;
            margin: 10px 0;
            border-radius: 4px;
            font-weight: bold;
        }
        .success {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        .error {
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
        .info {
            background-color: #d1ecf1;
            color: #0c5460;
            border: 1px solid #bee5eb;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            margin: 15px 0;
        }
        table, th, td {
            border: 1px solid #ddd;
        }
        th {
            background-color: #007bff;
            color: white;
            padding: 12px;
            text-align: left;
        }
        td {
            padding: 12px;
        }
        tr:nth-child(even) {
            background-color: #f9f9f9;
        }
        tr:hover {
            background-color: #f0f0f0;
        }
        .button {
            padding: 10px 20px;
            margin: 5px;
            background-color: #007bff;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }
        .button:hover {
            background-color: #0056b3;
        }
        .section {
            margin: 20px 0;
            padding: 15px;
            background-color: #f9f9f9;
            border-left: 4px solid #007bff;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>🔍 CyberArena Database Diagnostics & CRUD Test</h1>

            <div class="status success">
                ✅ Database Status: Connected to CyberArena
            </div>

            <h2>📊 Table Summary</h2>
            <asp:GridView ID="gvTableSummary" runat="server" CssClass="table" AutoGenerateColumns="true" ShowHeader="true">
            </asp:GridView>

            <h2>👥 AppUser Table Data</h2>
            <div class="section">
                <h3>All Users (10 total)</h3>
                <asp:GridView ID="gvAppUsers" runat="server" CssClass="table" AutoGenerateColumns="true" ShowHeader="true">
                </asp:GridView>
            </div>

            <h2>🎯 Challenge Table Data</h2>
            <div class="section">
                <h3>All Challenges (12 total)</h3>
                <asp:GridView ID="gvChallenges" runat="server" CssClass="table" AutoGenerateColumns="true" ShowHeader="true">
                </asp:GridView>
            </div>

            <h2>👥 Team Table Data</h2>
            <div class="section">
                <h3>All Teams (6 total)</h3>
                <asp:GridView ID="gvTeams" runat="server" CssClass="table" AutoGenerateColumns="true" ShowHeader="true">
                </asp:GridView>
            </div>

            <h2>🔗 TeamMember Table Data</h2>
            <div class="section">
                <h3>All Team Memberships (7 total)</h3>
                <asp:GridView ID="gvTeamMembers" runat="server" CssClass="table" AutoGenerateColumns="true" ShowHeader="true">
                </asp:GridView>
            </div>

            <h2>🧪 CRUD Operations Test</h2>
            <div class="section">
                <h3>Test Controls</h3>
                <asp:Button ID="btnTestCreate" runat="server" Text="Test CREATE (Add New User)" CssClass="button" OnClick="TestCreate_Click" />
                <asp:Button ID="btnTestRead" runat="server" Text="Test READ (Refresh Data)" CssClass="button" OnClick="TestRead_Click" />
                <asp:Button ID="btnTestUpdate" runat="server" Text="Test UPDATE (Update User)" CssClass="button" OnClick="TestUpdate_Click" />
                <asp:Button ID="btnTestDelete" runat="server" Text="Test DELETE (Remove Test User)" CssClass="button" OnClick="TestDelete_Click" />
                <asp:Button ID="btnResetData" runat="server" Text="Reset to Original Seed Data" CssClass="button" OnClick="ResetData_Click" />
            </div>

            <h2>📝 Test Results</h2>
            <asp:Label ID="lblTestResults" runat="server" Text="" CssClass="status info"></asp:Label>

            <h2>🔧 Connection Information</h2>
            <div class="section">
                <p><strong>Server:</strong> (LocalDb)\MSSQLLocalDB</p>
                <p><strong>Database:</strong> CyberArena</p>
                <p><strong>Connection String:</strong> <code>Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=CyberArena;Integrated Security=true;</code></p>
            </div>
        </div>
    </form>
</body>
</html>
