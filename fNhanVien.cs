using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JazzCoffe
{
    public partial class fNhanVien : Form
    {
        QuanLyCafeEntities2 db = new QuanLyCafeEntities2();
        public fNhanVien()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void lbSearchName_Click(object sender, EventArgs e)
        {

        }

        private void fNhanVien_Load(object sender, EventArgs e)
        {
            cbChucVu.Items.Add("Quản trị viên");
            cbChucVu.Items.Add("Nhân viên");
            cbChucVu.SelectedIndex = 0; 
            LoadData();
        }

        private void LoadData()
        {
            
                var dsNhanVien = db.NhanViens
                    .Select(nv => new
                    {
                        nv.MaNV,
                        nv.TenNV,
                        nv.MatKhau,
                        nv.SDT,
                        nv.Quyen,
                        nv.LuongCoBanTheoGio,
                    })
                    .ToList();

                dtgvNhanVien.DataSource = dsNhanVien;
            dtgvNhanVien.Columns["LuongCoBanTheoGio"].DefaultCellStyle.Format = "#,##0 VNĐ";
            dtgvNhanVien.Columns["LuongCoBanTheoGio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



        }


        private void dtgvNhanVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dtgvNhanVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dtgvNhanVien.Rows[e.RowIndex].Cells.Count > 0)
            {
                DataGridViewRow row = dtgvNhanVien.Rows[e.RowIndex];

                txtMaNV.Text = row.Cells["MaNV"]?.Value?.ToString() ?? string.Empty;
                txtTenNV.Text = row.Cells["TenNV"]?.Value?.ToString() ?? string.Empty;
                txtNVPassword.Text = row.Cells["MatKhau"]?.Value?.ToString() ?? string.Empty;
                txtSDT.Text = row.Cells["SDT"]?.Value?.ToString() ?? string.Empty;
               
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2")); 
                }
                return sb.ToString();
            }
        }

        private void thêmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string maNV = txtMaNV.Text.Trim();
                string tenNV = txtTenNV.Text.Trim();
                string sdt = txtSDT.Text.Trim();
                string quyen = cbChucVu.Text;
                decimal luong = 0;
                decimal.TryParse(txtLuongCoBan.Text, out luong);

                string matKhauMaHoa = HashPassword("123");

                NhanVien nv = new NhanVien()
                {
                    MaNV = maNV,
                    TenNV = tenNV,
                    MatKhau = matKhauMaHoa,
                    SDT = sdt,
                    Quyen = quyen,
                    LuongCoBanTheoGio = luong
                };

                db.NhanViens.Add(nv);
                db.SaveChanges();

                LoadData();
                MessageBox.Show("Thêm nhân viên thành công! Mật khẩu mặc định là 123 (đã mã hóa).");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm: " + ex.Message);
            }
        }

        private void sửaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string maNV = txtMaNV.Text.Trim();

            if (string.IsNullOrWhiteSpace(txtMaNV.Text) ||
                string.IsNullOrWhiteSpace(txtTenNV.Text) ||
                string.IsNullOrWhiteSpace(txtNVPassword.Text) ||
                string.IsNullOrWhiteSpace(txtSDT.Text) ||
                string.IsNullOrWhiteSpace(txtLuongCoBan.Text) ||
                string.IsNullOrWhiteSpace(cbChucVu.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin để sửa.");
                return;
            }

            if (!decimal.TryParse(txtLuongCoBan.Text, out decimal luongCoBan))
            {
                MessageBox.Show("Lương cơ bản phải là số hợp lệ.");
                return;
            }

            var nv = db.NhanViens.FirstOrDefault(n => n.MaNV == maNV);
            if (nv == null)
            {
                MessageBox.Show("Không tìm thấy nhân viên để sửa.");
                return;
            }

            nv.TenNV = txtTenNV.Text.Trim();
            nv.MatKhau = txtNVPassword.Text.Trim();
            nv.SDT = txtSDT.Text.Trim();
            nv.Quyen = cbChucVu.Text;
            nv.LuongCoBanTheoGio = luongCoBan;

            try
            {
                db.SaveChanges();
                LoadData();
                MessageBox.Show("Đã sửa thông tin nhân viên thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa: " + ex.InnerException?.InnerException?.Message);
            }
        }

        private void xóaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string maNV = txtMaNV.Text.Trim();

            if (string.IsNullOrWhiteSpace(maNV))
            {
                MessageBox.Show("Vui lòng nhập hoặc chọn mã nhân viên cần xóa.");
                return;
            }

            var nv = db.NhanViens.FirstOrDefault(n => n.MaNV == maNV);
            if (nv == null)
            {
                MessageBox.Show("Không tìm thấy nhân viên để xóa.");
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa nhân viên '{nv.TenNV}' không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
                return;

            try
            {
                db.NhanViens.Remove(nv);
                db.SaveChanges();
                LoadData();

                MessageBox.Show("Đã xóa nhân viên thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa nhân viên này. Có thể nhân viên đang được tham chiếu ở bảng khác.\n\nChi tiết lỗi: "
                    + ex.InnerException?.InnerException?.Message);
            }
        }

        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {

            txtMaNV.Clear();
            txtTenNV.Clear();
            txtNVPassword.Clear();
            txtSDT.Clear();
            txtLuongCoBan.Clear();
            txtSearchNameNV.Clear();

            LoadData();

            txtTenNV.Focus();
        }

        private void btSearchNameNV_Click(object sender, EventArgs e)
        {
            string keyword = txtSearchNameNV.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("Vui lòng nhập tên nhân viên cần tìm.");
                return;
            }

            var result = db.NhanViens
                .Where(nv => nv.TenNV.ToLower().Contains(keyword))
                .Select(nv => new
                {
                    nv.MaNV,
                    nv.TenNV,
                    nv.MatKhau,
                    nv.SDT,
                    nv.LuongCoBanTheoGio,
                    nv.Quyen
                })
                .ToList();

            dtgvNhanVien.DataSource = result;

            if (result.Count == 0)
            {
                MessageBox.Show("Không tìm thấy nhân viên nào phù hợp với từ khóa đã nhập.");
            }
        }

    }
}

