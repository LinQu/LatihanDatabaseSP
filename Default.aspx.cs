using System;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Web.UI.WebControls;

namespace LatihanDatabase
{
    public partial class Default : System.Web.UI.Page
    {
        public string TempID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadData(txtPencarian.Text);
                getListProvinsi();
            }
            gridData.Width = Unit.Percentage(100);
        }

        protected void loadData(string query)
        {
            try
            {
                SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                conn.Open();
                //VIEW DATA WITH STORE PRECODURE
                SqlCommand cmd = new SqlCommand("viewKar", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@query", query);
                cmd.Parameters.AddWithValue("@prov", query);
                

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gridData.DataSource = dt;
                gridData.DataBind();
                conn.Close();
                

               

                gridData.DataSource = dt;
                gridData.DataBind();
                clearForm();
                panelViewData.Visible = true;
                panelManupulasiData.Visible = false;
                btnKirim.Visible = true;
                btnUpdate.Visible = false;
                conn.Close();
            }
            catch { }
        }

        protected void getListProvinsi()
        {
            try
            {
                SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                conn.Open();

                SqlCommand command =
                    new SqlCommand("select pro_id, pro_nama from lat_msprovinsi order by pro_nama asc;", conn);

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());

                ddProvinsi.Items.Clear();
                ddProvinsi.DataSource = dt;
                ddProvinsi.DataValueField = "pro_nama";
                ddProvinsi.DataTextField = "pro_nama";
                ddProvinsi.DataBind();
                ddProvinsi.Items.Insert(0, new ListItem("-- Pilih Provinsi --", ""));
                ddProvinsi.AppendDataBoundItems = false;

                conn.Close();
            }
            catch { }
        }

        protected void btnPencarian_Click(object sender, EventArgs e)
        {
            loadData(txtPencarian.Text);
        }

        protected void linkTambahBaru_Click(object sender, EventArgs e)
        {
            clearForm();
            panelViewData.Visible = false;
            panelManupulasiData.Visible = true;
            literalTitle.Text = "Form Tambah Data Karyawan";
        }

        protected void btnKirim_Click(object sender, EventArgs e)
        {
            if (hiddenID.Text.Equals(""))
                createDataKaryawan();
            else
                updateDataKaryawan(hiddenID.Text);
            panelViewData.Visible = true;
            panelManupulasiData.Visible = false;
        }

        protected void createDataKaryawan()
        {
            try
            {
                SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                conn.Open();

                //inser data use store procedure
                SqlCommand command = new SqlCommand("insertKar", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@kry_npk", txtNPK.Text);
                command.Parameters.AddWithValue("@kry_nama", txtNama.Text);
                command.Parameters.AddWithValue("@kry_prov", ddProvinsi.Text);

                command.ExecuteNonQuery();
                


                conn.Close();

                loadData(txtPencarian.Text);
            }
            catch { }
        }

        protected void deleteDataKaryawan(string id)
        {
            try
            {
                SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                conn.Open();

                //delete data with store procedure
                SqlCommand command = new SqlCommand("deleteKar", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
                

                conn.Close();

                loadData(txtPencarian.Text);
            }
            catch { }
        }

        protected void updateDataKaryawan(string id)
        {
            try
            {
                SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                conn.Open();

                //update data with store procedure
                SqlCommand command = new SqlCommand("updateKar", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@kry_npk", txtNPK.Text);
                command.Parameters.AddWithValue("@kry_nama", txtNama.Text);
                command.Parameters.AddWithValue("@kry_prov", ddProvinsi.SelectedValue);

                command.ExecuteNonQuery();
                



                conn.Close();

                loadData(txtPencarian.Text);
            }
            catch { }
        }

        protected void clearForm()
        {
            hiddenID.Text = "";
            txtNPK.Text = "";
            txtNama.Text = "";
            ddProvinsi.SelectedIndex = 0;
            txtPencarian.Text = "";
        }

        protected void gridData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridData.PageIndex = e.NewPageIndex;
            loadData(txtPencarian.Text);
        }

        protected void gridData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Page")
            {
               string TempID = gridData.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();

                if (e.CommandName == "Hapus")
                {
                    deleteDataKaryawan(TempID);
                }
                else if (e.CommandName == "Ubah")
                {
                    hiddenID.Text = TempID;
                    panelViewData.Visible = false;
                    panelManupulasiData.Visible = true;
                    literalTitle.Text = "Form Ubah Data Karyawan";

                    try
                    {
                        SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                        conn.Open();

                        SqlCommand command = new SqlCommand("select kry_npk, kry_nama, kry_provinsi from lat_mskaryawan where kry_id = '" + TempID + "';", conn);

                        DataTable dt = new DataTable();
                        dt.Load(command.ExecuteReader());

                        txtNPK.Text = dt.Rows[0][0].ToString();
                        txtNama.Text = dt.Rows[0][1].ToString();
                        ddProvinsi.SelectedValue = dt.Rows[0][2].ToString();

                        btnKirim.Visible = false;
                        btnUpdate.Visible = true;
                        

                        conn.Close();
                    }
                    catch { }
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            updateDataKaryawan(hiddenID.Text);
            loadData(txtPencarian.Text);
        }
    }
}